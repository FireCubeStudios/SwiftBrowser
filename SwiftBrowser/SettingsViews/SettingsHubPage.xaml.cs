using SwiftBrowser.HubViews;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHubPage : Page
    {
        public SettingsHubPage()
        {
            InitializeComponent();
            SetHubNav.SelectedItem = Main;
           contentFrame.Navigate(typeof(SettingsPage));
        }

        private void SetHubNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            MUXC.Controls.NavigationViewItem NavItem = SetHubNav.SelectedItem as MUXC.Controls.NavigationViewItem;
            switch(NavItem.Content.ToString())
            {
                case "About":
                    contentFrame.Navigate(typeof(AboutPage));
                    break;
                case "Sync":
                    contentFrame.Navigate(typeof(SyncView));
                    break;
                case "Downloads":
                    contentFrame.Navigate(typeof(Downloads));
                    break;
                case "Home page":
                    contentFrame.Navigate(typeof(HomePageSettings));
                    break;
                case "Permissions":
                    contentFrame.Navigate(typeof(PermissionsSettingsPage));
                    break;
                case "Start-up":
                    contentFrame.Navigate(typeof(StartupSettings));
                    break;
                case "Reset":
                    contentFrame.Navigate(typeof(ResetSettings));
                    break;
                case "Personalization":
                    contentFrame.Navigate(typeof(SettingsPage));
                    break;
                case "Web settings":
                    contentFrame.Navigate(typeof(WebSettings));
                    break;
                case "Privacy and security":
                    contentFrame.Navigate(typeof(PrivacySettings));
                    break;
            }

        }
    }
}
