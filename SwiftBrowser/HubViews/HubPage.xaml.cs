using Windows.UI.Xaml.Controls;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        public HubPage()
        {
            InitializeComponent();
            switch (NavString)
            {
                case "F":
                    HubNav.SelectedItem = Fav;
                    contentFrame.Navigate(typeof(Favourites));
                    break;
                case "H":
                    HubNav.SelectedItem = Hav;
                    contentFrame.Navigate(typeof(History));
                    break;
                case "D":
                    HubNav.SelectedItem = Dav;
                    contentFrame.Navigate(typeof(Downloads));
                    break;
                case "O":
                    HubNav.SelectedItem = Oav;
                    contentFrame.Navigate(typeof(OfflinePage));
                    break;
                case "P":
                    HubNav.SelectedItem = Pav;
                    contentFrame.Navigate(typeof(PasswordMasterDetails));
                    break;
            }
        }

        public static string NavString { get; set; }

        private void HubNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Content.ToString() == "Favorites")
                contentFrame.Navigate(typeof(Favourites));
            else if (args.InvokedItemContainer.Content.ToString() == "History")
                contentFrame.Navigate(typeof(History));
            else if (args.InvokedItemContainer.Content.ToString() == "Offline")
                contentFrame.Navigate(typeof(OfflinePage));
            else if (args.InvokedItemContainer.Content.ToString() == "Passwords")
                contentFrame.Navigate(typeof(PasswordMasterDetails));
            else if (args.InvokedItemContainer.Content.ToString() == "Sync")
                contentFrame.Navigate(typeof(SyncView));
            else
                contentFrame.Navigate(typeof(Downloads));
        }
    }
}
