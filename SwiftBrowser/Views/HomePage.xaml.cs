using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using SwiftBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public static AdaptiveGridView HomeGrid { get; set; }
        public static WebView WebViewControl { get; set; }
        WebView webViewControl;
        public HomePage()
        {
            this.InitializeComponent();
            LoadFavorites();
            LoadQuickPinned();
            HomeGrid = FavouritesGridView;
            webViewControl = WebViewControl;
        }
        List<FavouritesJSON> FavouritesList;
        List<FavouritesJSON> FavouritesListQuick;
        public class FavouritesJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
        public class FavouritesClass
        {
            public List<FavouritesJSON> Websites { get; set; }
        }
    
    private async void LoadFavorites()
    {
            try { 
            FavouritesList = new List<FavouritesJSON>();
            StorageFile sampleFile = await localFolder.GetFileAsync("Favorites.json");
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            foreach (var item in FavouritesListJSON.Websites)
            {
                FavouritesList.Add(new FavouritesJSON()
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON,
                });
            }
            FavouritesGridView.ItemsSource = FavouritesList;
            }
            catch
            {
                var JSONData = "e";
                string filepath = @"Assets\Favorites.json";
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await folder.GetFileAsync(filepath); // error here
              //  StorageFile sfile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
              //  await FileIO.WriteTextAsync(sfile, JSONData);
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                StorageFile sampleFile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONData);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    FavouritesList.Add(new FavouritesJSON()
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON,
                    });
                }
                FavouritesGridView.ItemsSource = FavouritesList;
            }
        }
        private async void LoadQuickPinned()
        {
            // using the favorites class
            // TODO: change name of "favorites" class to something else
            try
            {
                FavouritesListQuick = new List<FavouritesJSON>();
                StorageFile sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    FavouritesListQuick.Add(new FavouritesJSON()
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON,
                    });
                }
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
            catch
            {
                var JSONData = "e";
                string filepath = @"Assets\QuickPinWeb.json";
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await folder.GetFileAsync(filepath); 
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
              //  StorageFile sfile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
              //  await FileIO.WriteTextAsync(sfile, JSONData);
                StorageFile sampleFile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONData);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    FavouritesListQuick.Add(new FavouritesJSON()
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON,
                    });
                }
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
        }
        private async void DeleteFavButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            FavouritesJSON FoundItem = FavouritesListJSON.Websites.Find(x => x.UrlJSON == SenderPost.UrlJSON);
            FavouritesListJSON.Websites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            FavouritesClass FavouritesListsJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONDatas);
            foreach (var item in FavouritesListsJSON.Websites)
            {
                FavouritesList.Add(new FavouritesJSON()
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON,
                });
            }
            QuickPinnedGridView.ItemsSource = null;
            QuickPinnedGridView.ItemsSource = FavouritesList;
        }

        private void NewsHomePage_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            WebViewPage.WebviewControl.Navigate(args.Uri);
            WebViewPage.HomeFrameFrame.Visibility = Visibility.Collapsed;
            args.Handled = true;
        }

      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FindName("ContentPivot");
            LoadMore.Visibility = Visibility.Collapsed;

        }

        private void FavStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
        WebViewPage.WebviewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            WebViewPage.WebviewControl.Navigate(new Uri("https://www.ecosia.org/search?q=" + sender.Text));
        }

        private void QGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            WebViewPage.WebviewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String Url = UrlBox.Text.Replace("https://", "");
                Uri ArgsUri = new Uri(UrlBox.Text);
                string host = ArgsUri.Host;
                string Header;
                if (string.IsNullOrEmpty(NameBox.Text) == true)
                {
                    Header = UrlBox.Text;
                }
                else
                {
                    Header = NameBox.Text;
                }
                StorageFile sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                FavouritesListJSON.Websites.Add(new FavouritesJSON()
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = UrlBox.Text,
                    HeaderJSON = Header
                }); ;
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListsJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONDatas);
                foreach (var item in FavouritesListsJSON.Websites)
                {
                    FavouritesList.Add(new FavouritesJSON()
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON,
                    });
                }
                QuickPinnedGridView.ItemsSource = null;
                QuickPinnedGridView.ItemsSource = FavouritesList;
            }
            catch
            {
                return;
            }
        }
    }
}
