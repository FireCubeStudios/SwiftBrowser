using Newtonsoft.Json;
using SwiftBrowser.HubViews;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        public static string NavString{get; set;}
        public HubPage()
        {
            this.InitializeComponent();
            switch(NavString)
                {
                case "F":
                    HubNav.SelectedItem = Fav;
                    contentFrame.Navigate(typeof(ExtensionsStore));
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

        private void HubNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Content.ToString() == "Favorites")
            {
                contentFrame.Navigate(typeof(Favourites));
            }
            else if(args.InvokedItemContainer.Content.ToString() == "History")
            {
                contentFrame.Navigate(typeof(History));
            }
            else if (args.InvokedItemContainer.Content.ToString() == "Offline")
            {
                contentFrame.Navigate(typeof(OfflinePage));
            }
            else if (args.InvokedItemContainer.Content.ToString() == "Passwords")
            {
                contentFrame.Navigate(typeof(PasswordMasterDetails));
            }
            else
            {
                contentFrame.Navigate(typeof(Downloads));
            }
        }
    }
}
