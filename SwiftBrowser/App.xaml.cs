using System;                 
using SwiftBrowser.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;
using SwiftBrowser.Assets;
using Microsoft.Gaming.XboxGameBar;
using SwiftBrowser.Views;
using Windows.UI.Xaml.Controls;
using SwiftBrowser.HubViews;

namespace SwiftBrowser
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private XboxGameBarWidget widgetBrowser = null;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            DataAccess.InitializeDatabase();
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

        private void Widget1Window_Closed(object sender, Windows.UI.Core.CoreWindowEventArgs e)

        {

            widgetBrowser = null;

            Window.Current.Closed -= Widget1Window_Closed;

        }
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            /*XboxGameBarWidgetActivatedEventArgs widgetArgs = null;

            if (args.Kind == ActivationKind.Protocol)

            {

                var protocolArgs = args as IProtocolActivatedEventArgs;

                string scheme = protocolArgs.Uri.Scheme;

                if (scheme.Equals("ms-gamebarwidget"))

                {

                    widgetArgs = args as XboxGameBarWidgetActivatedEventArgs;

                }

            }

            if (widgetArgs != null)
            {
                if (widgetArgs.IsLaunchActivation)
                {
                    var rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                    // Create Game Bar widget object which bootstraps the connection with Game Bar
                    widgetBrowser = new XboxGameBarWidget(

                        widgetArgs,
                        Window.Current.CoreWindow,
                        rootFrame);
                    rootFrame.Navigate(typeof(Downloads));
                    Window.Current.Closed += Widget1Window_Closed;
                    Window.Current.Activate();
                }
                else
                {*/
                          if (args.Kind == ActivationKind.Protocol)
                    {
                        ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                        localSettings.Values["SourceToGo"] = eventArgs.Uri.AbsoluteUri.ToString();
                        // The received URI is eventArgs.Uri.AbsoluteUri
                    }
              
              //  }
                await ActivationService.ActivateAsync(args);
         //   }

        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.TabViewPage));
        }
    }
}
