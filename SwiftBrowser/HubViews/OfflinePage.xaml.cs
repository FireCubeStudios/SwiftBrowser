using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using SwiftBrowser.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfflinePage : Page
    {
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<OfflineJSON> OfflineList;

        public OfflinePage()
        {
            InitializeComponent();
            Singletonreference = this;
            LoadOfflines();
        }

        public static OfflinePage Singletonreference { get; set; }
        public static WebView WebWeb { get; set; }
        public static Frame ImageFrame { get; set; }
        public static bool BoolWeb { get; set; }

        private async void LoadOfflines()
        {
            OfflineList = new List<OfflineJSON>();
            var filepath = @"Assets\OfflinePages.json";
            var folder = Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filepath); // error here
            var JSONData = "e";
            try
            {
                if ((bool) localSettings.Values["FirstFavRun"])
                {
                    localSettings.Values["FirstFavRun"] = false;
                    var sfile = await localFolder.CreateFileAsync("OfflinePages.json",
                        CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(sfile, JSONData);
                    JSONData = await FileIO.ReadTextAsync(file);
                }
                else
                {
                    localSettings.Values["FirstFavRun"] = false;
                    var ssfile = await localFolder.GetFileAsync("OfflinePages.json");
                    JSONData = await FileIO.ReadTextAsync(ssfile);
                }
            }
            catch
            {
                localSettings.Values["FirstFavRun"] = false;
                var sssfile =
                    await localFolder.CreateFileAsync("OfflinePages.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sssfile, JSONData);
                JSONData = await FileIO.ReadTextAsync(file);
            }

            localSettings.Values["FirstFavRun"] = false;
            var sampleFile =
                await localFolder.CreateFileAsync("OfflinePages.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, JSONData);
            var OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            foreach (var item in OfflineListJSON.OfflineWebsites)
                OfflineList.Add(new OfflineJSON
                {
                    HeaderJSON = item.HeaderJSON,
                    ImageUrlJSON = item.ImageUrlJSON,
                    FavIconJSON = item.FavIconJSON
                });
            Offlines.ItemsSource = OfflineList;
        }

        public async void LoadOffline()
        {
            var sampleFile = await localFolder.GetFileAsync("OfflinePages.json");
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            OfflineList.Clear();
            foreach (var item in OfflineListJSON.OfflineWebsites)
                OfflineList.Add(new OfflineJSON
                {
                    HeaderJSON = item.HeaderJSON,
                    ImageUrlJSON = item.ImageUrlJSON,
                    FavIconJSON = item.FavIconJSON
                });
            Offlines.ItemsSource = null;
            UnloadObject(Offlines);
            FindName("Offlines");
            Offlines.ItemsSource = OfflineList;
            HomePage.HomeGrid.ItemsSource = null;
            HomePage.HomeGrid.ItemsSource = OfflineList;
        }

        private async void DeleteOffline(object sender, RoutedEventArgs e)
        {
            var sampleFile = await localFolder.GetFileAsync("OfflinePages.json");
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as OfflineJSON;
            //  List<Favourites> OpenList = Offlines.ItemsSource as List<Favourites>;
            //  OpenList.Remove(SenderPost);
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            //i had a better way to do it before but accidently permanently deleted it and cant remember how. This method is shorter and simpler
            var FoundItem = OfflineListJSON.OfflineWebsites.Find(x => x.ImageUrlJSON == SenderPost.ImageUrlJSON);
            OfflineListJSON.OfflineWebsites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(OfflineListJSON, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadOffline();
        }

        private void AddOffline(object sender, RoutedEventArgs e)
        {
            //  if (BoolWeb == true)

            var random = new Random();
            var randomnumber = random.Next();
            var rand = randomnumber.ToString();
            var webpage = WebWeb.Tag as WebViewPage;
            webpage.CreateRTBOffline(rand);
            //   {
        }

        public async void OfflineAddToJSON(string rand)
        {
            var sampleFile = await localFolder.GetFileAsync("OfflinePages.json");

            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            var ArgsUri = new Uri(WebWeb.Source.ToString());
            var host = ArgsUri.Host;
            string x;
            x = await WebWeb.InvokeScriptAsync("eval", new[] {"document.title;"});
            OfflineListJSON.OfflineWebsites.Add(new OfflineJSON
            {
                FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                ImageUrlJSON = rand + ".jpg",
                HeaderJSON = x
            });
            ;
            var SerializedObject = JsonConvert.SerializeObject(OfflineListJSON, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadOffline();
        }

        private async void OpenOffline(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as OfflineJSON;
            WebViewPage.SingletonReference.Churros();
            ImageFrame.Visibility = Visibility.Visible;
            ImageFrame.Navigate(typeof(OfflineModePage));
            var sampleFile = await localFolder.GetFileAsync(SenderPost.ImageUrlJSON);
            OfflineModePage.OiMage.Source = new BitmapImage(new Uri(sampleFile.Path));
            //  WebWeb.Navigate(new Uri(SenderPost.ImageUrlJSON));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as OfflineJSON;
            WebWeb.Navigate(new Uri(SenderPost.ImageUrlJSON));
        }

        public class OfflineClass
        {
            public List<OfflineJSON> OfflineWebsites { get; set; }
        }

        /*  public class Favourites
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class OfflineJSON
        {
            public string HeaderJSON { get; set; }
            public string ImageUrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
    }
}
