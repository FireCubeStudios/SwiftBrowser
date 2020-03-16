using Microsoft.UI.Xaml.Controls;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace SwiftBrowser.Helpers
{
    public class WebViewEvents
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private void webView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
          //  TabView er = TabViewPage.TabviewPageControl;
            var newTab = new TabViewItem();
            newTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.World };
            newTab.Header = args.Uri.ToString();

            // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            localSettings.Values["SourceToGo"] = args.Uri.ToString();
            frame.Navigate(typeof(WebViewPage));
          //  er.TabItems.Add(newTab);
            args.Handled = true;
        }

        public void webView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
           /* WebViewPage.InfoDialog.IsOpen = true;
            WebViewPage.InfoDialog.Title = "Long running script";
            WebViewPage.InfoDialog.IsLightDismissEnabled = false;*/
        }
        public void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                applicationView.TryEnterFullScreenMode();
            }
            else if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
            }
        }

        public void webView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            WebViewPage.InfoDialog.IsOpen = true;
            WebViewPage.InfoDialog.Title = "Unsupported uri scheme detected";
            WebViewPage.InfoDialog.IsLightDismissEnabled = false;
        }

        public void webView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            WebViewPage.InfoDialog.IsOpen = true;
            WebViewPage.InfoDialog.Title = "Unsafe content";
            WebViewPage.InfoDialog.Subtitle = "Windows defender detected unsafe content";
            WebViewPage.InfoDialog.IsLightDismissEnabled = false;
            WebViewPage.InfoDialog.ActionButtonContent = "Go anyway";
        }

        public void webView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            WebViewPage.InfoDialog.IsOpen = true;
            WebViewPage.InfoDialog.Title = "Unviewable content";
            WebViewPage.InfoDialog.IsLightDismissEnabled = false;
            WebViewPage.InfoDialog.ActionButtonContent = "Launch content";
            WebViewPage.InfoDialog.ActionButtonClick += LaunchUnviewable;
        }
        public void LaunchUnviewable(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            Windows.Foundation.IAsyncOperation<bool> b =
            Windows.System.Launcher.LaunchUriAsync(WebViewPage.WebviewControl.Source);
        }
    }
}
