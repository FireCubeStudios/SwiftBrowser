using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebApp : Page
    {
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public WebView webViewer;

        public WebApp()
        {
            InitializeComponent();
            var webView = new WebView(WebViewExecutionMode.SeparateProcess);
            webView.Name = "webViewApp";
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(bACKGROUND);
            webView.Height = 1000;
            webViewer = webView;
            webView.MinHeight = 300;
            webView.LongRunningScriptDetected += WebView_LongRunningScriptDetected;
            var Margin = webView.Margin;
            Margin.Top = 40;
            webView.Margin = Margin;
            webView.DOMContentLoaded += WebView_DOMContentLoaded;
            webView.IsRightTapEnabled = true;
            webView.NewWindowRequested += WebView_NewWindowRequested;
            ContentGrid.Children.Add(webView);
            try
            {
                webView.Navigate(new Uri(WebViewNavigationString));
            }
            catch
            {
                webView.Navigate(new Uri((string) localSettings.Values["BackupSourceToGo"]));
            }

            localSettings.Values["BackupSourceToGo"] = null;
        }


        public static string WebViewNavigationString { get; set; }

        private void WebView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
        }


        private void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
        }

        private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
        }

        private void BackBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (webViewer.CanGoBack) webViewer.GoBack();
        }

        private void RefreshBarButton_Click(object sender, RoutedEventArgs e)
        {
            webViewer.Refresh();
        }
    }
}
