using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using SwiftBrowser.Helpers;

namespace SwiftBrowser.Services
{
    // A custom event that fires whenever the secondary view is ready to be closed. You should
    // clean up any state (including deregistering for events) then close the window in this handler

    // Whenever the main view is about to interact with the secondary view, it should call
    // StartViewInUse on this object. When finished interacting, it should call StopViewInUse.
    public sealed class ViewLifetimeControl
    {
        private readonly object _lockObj = new object();
        private int _refCount;
        private bool _released;

        // Window for this particular view. Used to register and unregister for events
        private readonly CoreWindow _window;

        private ViewLifetimeControl(CoreWindow newWindow)
        {
            Dispatcher = newWindow.Dispatcher;
            _window = newWindow;
            Id = ApplicationView.GetApplicationViewIdForWindow(_window);
            RegisterForEvents();
        }

        // Necessary to communicate with the window
        public CoreDispatcher Dispatcher { get; }

        // This id is used in all of the ApplicationViewSwitcher and ProjectionManager APIs
        public int Id { get; }

        public string Title { get; set; }

        private event ViewReleasedHandler InternalReleased;

        public event ViewReleasedHandler Released
        {
            add
            {
                var releasedCopy = false;
                lock (_lockObj)
                {
                    releasedCopy = _released;
                    if (!_released) InternalReleased += value;
                }

                if (releasedCopy)
                    throw new InvalidOperationException("ExceptionViewLifeTimeControlViewDisposal".GetLocalizedSwift());
            }

            remove
            {
                lock (_lockObj)
                {
                    InternalReleased -= value;
                }
            }
        }

        public static ViewLifetimeControl CreateForCurrentView()
        {
            return new ViewLifetimeControl(CoreWindow.GetForCurrentThread());
        }

        // Signals that the view is being interacted with by another view,
        // so it shouldn't be closed even if it becomes "consolidated"
        public int StartViewInUse()
        {
            var releasedCopy = false;
            var refCountCopy = 0;

            lock (_lockObj)
            {
                releasedCopy = _released;
                if (!_released) refCountCopy = ++_refCount;
            }

            if (releasedCopy)
                throw new InvalidOperationException("ExceptionViewLifeTimeControlViewDisposal".GetLocalizedSwift());

            return refCountCopy;
        }

        // Should come after any call to StartViewInUse
        // Signals that the another view has finished interacting with the view tracked by this object
        public int StopViewInUse()
        {
            var refCountCopy = 0;
            var releasedCopy = false;

            lock (_lockObj)
            {
                releasedCopy = _released;
                if (!_released)
                {
                    refCountCopy = --_refCount;
                    if (refCountCopy == 0) Dispatcher.RunAsync(CoreDispatcherPriority.Low, FinalizeRelease).AsTask();
                }
            }

            if (releasedCopy)
                throw new InvalidOperationException("ExceptionViewLifeTimeControlViewDisposal".GetLocalizedSwift());

            return refCountCopy;
        }

        private void RegisterForEvents()
        {
            ApplicationView.GetForCurrentView().Consolidated += ViewConsolidated;
        }

        private void UnregisterForEvents()
        {
            ApplicationView.GetForCurrentView().Consolidated -= ViewConsolidated;
        }

        private void ViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs e)
        {
            StopViewInUse();
        }

        private void FinalizeRelease()
        {
            var justReleased = false;
            lock (_lockObj)
            {
                if (_refCount == 0)
                {
                    justReleased = true;
                    _released = true;
                }
            }

            if (justReleased)
            {
                UnregisterForEvents();
                if (InternalReleased == null)
                    // For more information about using Multiple Views, see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/features/multiple-views.md
                    throw new InvalidOperationException("ExceptionViewLifeTimeControlMissingReleasedSubscription"
                        .GetLocalizedSwift());

                InternalReleased.Invoke(this, null);
            }
        }
    }
}
