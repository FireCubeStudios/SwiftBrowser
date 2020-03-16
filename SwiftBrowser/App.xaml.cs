using System;

using SwiftBrowser.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SwiftBrowser
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var idOfTappedTile = args.Arguments;
            if(idOfTappedTile.Contains("http") == true) { 
                localSettings.Values["SourceToGo"] = idOfTappedTile.ToString();
            }
            else
            {
                localSettings.Values["SourceToGo"] = null;
            }
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.TabViewPage));
        }
    }
}
