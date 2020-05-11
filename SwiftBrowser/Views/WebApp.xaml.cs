using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebApp : Page
    {


        public static string WebViewNavigationString { get; set; }
        public WebView webViewer;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public WebApp()
        {
            this.InitializeComponent();
           WebView webView = new WebView(WebViewExecutionMode.SeparateProcess);
            webView.Name = "webViewApp";
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(bACKGROUND);
            webView.Height = 1000;
            webViewer = webView;
            webView.MinHeight = 300;
            webView.LongRunningScriptDetected += WebView_LongRunningScriptDetected;
            Thickness Margin = webView.Margin;
            Margin.Top = 40;
            webView.Margin = Margin;
            webView.DOMContentLoaded += WebView_DOMContentLoaded;
            webView.IsRightTapEnabled =true;
            webView.NewWindowRequested += WebView_NewWindowRequested;
            ContentGrid.Children.Add(webView);
            try
            {
                webView.Navigate(new Uri(WebViewNavigationString));
            }
            catch
            {
                webView.Navigate(new Uri((string)localSettings.Values["BackupSourceToGo"]));
            }
            localSettings.Values["BackupSourceToGo"] = null;
        }

        private void WebView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
       
        }


        private void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
         
        }

        private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            return;
        }

        private void BackBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(webViewer.CanGoBack == true)
            {
                webViewer.GoBack();
            }
        }

        private void RefreshBarButton_Click(object sender, RoutedEventArgs e)
        {
            webViewer.Refresh();
        }
    }
}
