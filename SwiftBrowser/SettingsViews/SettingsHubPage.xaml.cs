using Windows.UI.Xaml.Controls;
using SwiftBrowser.HubViews;
using SwiftBrowser.Views;
using MUXC = Microsoft.UI.Xaml;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHubPage : Page
    {
        public SettingsHubPage()
        {
            InitializeComponent();
            SetHubNav.SelectedItem = Main;
            contentFrame.Navigate(typeof(SettingsPage));
        }

        private void SetHubNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var NavItem = SetHubNav.SelectedItem as NavigationViewItem;
            switch (NavItem.Content.ToString())
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
