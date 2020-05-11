using Newtonsoft.Json;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Favourites : Page
    {
        public Favourites()
        {
            InitializeComponent();
            LoadFavorites();
        }
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static WebView WebWeb { get; set; }
        public static Boolean BoolWeb { get; set; }
        public class FavouritesClass
        {
            public List<FavouritesJSON> Websites { get; set; }
        }
        List<FavouritesJSON> FavouritesList;
      /*  public class Favourites
        {
            public string Header { get; set; }
            public string Url { get; set; }
            public string FavIcon { get; set; }
        }*/
        public class FavouritesJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
        private async void LoadFavorites()
        {
            FavouritesList = new List<FavouritesJSON>();
            string filepath = @"Assets\Favorites.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath); // error here
            var JSONData = "e";
            try
            {
                if ((bool)localSettings.Values["FirstFavRun"] == true)
                {
                    localSettings.Values["FirstFavRun"] = false;
                    StorageFile sfile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(sfile, JSONData);
                    JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                }
                else
                {
                    localSettings.Values["FirstFavRun"] = false;
                    StorageFile ssfile = await localFolder.GetFileAsync("Favorites.json");
                    JSONData = await FileIO.ReadTextAsync(ssfile);
                }
            }
            catch
            {
                localSettings.Values["FirstFavRun"] = false;
                StorageFile sssfile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sssfile, JSONData);
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
            }
            localSettings.Values["FirstFavRun"] = false;
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
            Favorites.ItemsSource = FavouritesList;
        }
        public async void LoadFav()
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("Favorites.json");
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            FavouritesList.Clear();
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
            //    Favorites.Items.Clear();
            Favorites.ItemsSource = null;
            UnloadObject(Favorites);
            FindName("Favorites");
            Favorites.ItemsSource = FavouritesList;

         // HomePage.HomeGrid.Items.Clear();
           HomePage.HomeGrid.ItemsSource = FavouritesList;
        }
        private async void AddSiteFav(object sender, RoutedEventArgs e)
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
                StorageFile sampleFile = await localFolder.GetFileAsync("Favorites.json");
                var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                FavouritesListJSON.Websites.Add(new FavouritesJSON()
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = UrlBox.Text,
                    HeaderJSON = Header
                }); 
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                LoadFav();
            }
            catch
            {
                return;
            }
        }

        private async void DeleteFav(object sender, RoutedEventArgs e)
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("Favorites.json");
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            //  List<Favourites> OpenList = Favorites.ItemsSource as List<Favourites>;
            //  OpenList.Remove(SenderPost);
            var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            //i had a better way to do it before but accidently permanently deleted it and cant remember how. This method is shorter and simpler
            FavouritesJSON FoundItem = FavouritesListJSON.Websites.Find(x => x.UrlJSON == SenderPost.UrlJSON);
            FavouritesListJSON.Websites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadFav();
        }

        private async void AddFav(object sender, RoutedEventArgs e)
        {
            if (BoolWeb == true)
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("Favorites.json");
                var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                Uri ArgsUri = new Uri(WebWeb.Source.ToString());
                string host = ArgsUri.Host;
                string x = "";
                try
                {
                    x = await WebWeb.InvokeScriptAsync("eval", new string[] { "document.title;" });
                }
                catch
                {
                    x = WebWeb.Source.ToString();
                }
                FavouritesListJSON.Websites.Add(new FavouritesJSON()
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = WebWeb.Source.ToString(),
                    HeaderJSON = x
                }); ;
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                LoadFav();
            }
            else
            {
                return;
            }
        }

        private void OpenFav(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void Favorites_ItemClick(object sender, ItemClickEventArgs e)
        {
            FavouritesJSON SenderPost = e.ClickedItem as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }
    }
}
