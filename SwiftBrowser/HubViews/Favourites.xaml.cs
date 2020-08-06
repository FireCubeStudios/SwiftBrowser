using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;
using SwiftBrowser.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Favourites : Page
    {
        private List<FavouritesJSON> FavouritesList;
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public Favourites()
        {
            InitializeComponent();
            LoadFavorites();
        }

        public static WebView WebWeb { get; set; }
        public static bool BoolWeb { get; set; }

        private async void LoadFavorites()
        {
            FavouritesList = new List<FavouritesJSON>();
            var filepath = @"Assets\Favorites.json";
            var folder = Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filepath); // error here
            var JSONData = "e";
            try
            {
                if ((bool) localSettings.Values["FirstFavRun"])
                {
                    localSettings.Values["FirstFavRun"] = false;
                    var sfile = await localFolder.CreateFileAsync("Favorites.json",
                        CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(sfile, JSONData);
                    JSONData = await FileIO.ReadTextAsync(file);
                }
                else
                {
                    localSettings.Values["FirstFavRun"] = false;
                    var ssfile = await localFolder.GetFileAsync("Favorites.json");
                    JSONData = await FileIO.ReadTextAsync(ssfile);
                }
            }
            catch
            {
                localSettings.Values["FirstFavRun"] = false;
                var sssfile =
                    await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sssfile, JSONData);
                JSONData = await FileIO.ReadTextAsync(file);
            }

            localSettings.Values["FirstFavRun"] = false;
            var sampleFile =
                await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, JSONData);
            var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            foreach (var item in FavouritesListJSON.Websites)
                FavouritesList.Add(new FavouritesJSON
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON
                });
            Favorites.ItemsSource = FavouritesList;
        }

        public async void LoadFav()
        {
            var sampleFile = await localFolder.GetFileAsync("Favorites.json");
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            FavouritesList.Clear();
            var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            foreach (var item in FavouritesListJSON.Websites)
                FavouritesList.Add(new FavouritesJSON
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON
                });
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
                var Url = UrlBox.Text.Replace("https://", "");
                var ArgsUri = new Uri(UrlBox.Text);
                var host = ArgsUri.Host;
                string Header;
                if (string.IsNullOrEmpty(NameBox.Text))
                    Header = UrlBox.Text;
                else
                    Header = NameBox.Text;
                var sampleFile = await localFolder.GetFileAsync("Favorites.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                FavouritesListJSON.Websites.Add(new FavouritesJSON
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = UrlBox.Text,
                    HeaderJSON = Header
                });
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                LoadFav();
            }
            catch
            {
            }
        }

        private async void DeleteFav(object sender, RoutedEventArgs e)
        {
            var sampleFile = await localFolder.GetFileAsync("Favorites.json");
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            //  List<Favourites> OpenList = Favorites.ItemsSource as List<Favourites>;
            //  OpenList.Remove(SenderPost);
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            //i had a better way to do it before but accidently permanently deleted it and cant remember how. This method is shorter and simpler
            var FoundItem = FavouritesListJSON.Websites.Find(x => x.UrlJSON == SenderPost.UrlJSON);
            FavouritesListJSON.Websites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadFav();
        }

        private async void AddFav(object sender, RoutedEventArgs e)
        {
            if (BoolWeb)
            {
                var sampleFile = await localFolder.GetFileAsync("Favorites.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                var ArgsUri = new Uri(WebWeb.Source.ToString());
                var host = ArgsUri.Host;
                var x = "";
                try
                {
                    x = await WebWeb.InvokeScriptAsync("eval", new[] {"document.title;"});
                }
                catch
                {
                    x = WebWeb.Source.ToString();
                }

                FavouritesListJSON.Websites.Add(new FavouritesJSON
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = WebWeb.Source.ToString(),
                    HeaderJSON = x
                });
                ;
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                LoadFav();
            }
            else
            {
            }
        }

        private void OpenFav(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void Favorites_ItemClick(object sender, ItemClickEventArgs e)
        {
            var SenderPost = e.ClickedItem as FavouritesJSON;
            WebWeb.Navigate(new Uri(SenderPost.UrlJSON));
        }

        public class FavouritesClass
        {
            public List<FavouritesJSON> Websites { get; set; }
        }

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
    }
}
