using Newtonsoft.Json;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfflinePage : Page
    {
        public static OfflinePage Singletonreference { get; set; }
        public OfflinePage()
        {
            this.InitializeComponent();
            Singletonreference = this;
            LoadOfflines();
        }
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static WebView WebWeb { get; set; }
        public static Frame ImageFrame { get; set; }
        public static Boolean BoolWeb { get; set; }
        public class OfflineClass
        {
            public List<OfflineJSON> OfflineWebsites { get; set; }
        }
        List<OfflineJSON> OfflineList;
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
        private async void LoadOfflines()
        {
            OfflineList = new List<OfflineJSON>();
            string filepath = @"Assets\OfflinePages.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath); // error here
            var JSONData = "e";
            try
            {
                if ((bool)localSettings.Values["FirstFavRun"] == true)
                {
                    localSettings.Values["FirstFavRun"] = false;
                    StorageFile sfile = await localFolder.CreateFileAsync("OfflinePages.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(sfile, JSONData);
                    JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                }
                else
                {
                    localSettings.Values["FirstFavRun"] = false;
                    StorageFile ssfile = await localFolder.GetFileAsync("OfflinePages.json");
                    JSONData = await FileIO.ReadTextAsync(ssfile);
                }
            }
            catch
            {
                localSettings.Values["FirstFavRun"] = false;
                StorageFile sssfile = await localFolder.CreateFileAsync("OfflinePages.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sssfile, JSONData);
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
            }
            localSettings.Values["FirstFavRun"] = false;
            StorageFile sampleFile = await localFolder.CreateFileAsync("OfflinePages.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, JSONData);
            OfflineClass OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            foreach (var item in OfflineListJSON.OfflineWebsites)
            {
                OfflineList.Add(new OfflineJSON()
                {
                    HeaderJSON = item.HeaderJSON,
                    ImageUrlJSON = item.ImageUrlJSON,
                    FavIconJSON = item.FavIconJSON,
                });
            }
            Offlines.ItemsSource = OfflineList;

        }
        public async void LoadOffline()
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("OfflinePages.json");
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            OfflineClass OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            foreach (var item in OfflineListJSON.OfflineWebsites)
            {
                OfflineList.Add(new OfflineJSON()
                {
                    HeaderJSON = item.HeaderJSON,
                    ImageUrlJSON = item.ImageUrlJSON,
                    FavIconJSON = item.FavIconJSON,
                });
            }
            Offlines.ItemsSource = null;
            UnloadObject(Offlines);
            FindName("Offlines");
            Offlines.ItemsSource = OfflineList;
            HomePage.HomeGrid.ItemsSource = null;
            HomePage.HomeGrid.ItemsSource = OfflineList;
        }

        private async void DeleteOffline(object sender, RoutedEventArgs e)
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("OfflinePages.json");
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            OfflineJSON SenderPost = DataContext as OfflineJSON;
            //  List<Favourites> OpenList = Offlines.ItemsSource as List<Favourites>;
            //  OpenList.Remove(SenderPost);
            var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            OfflineClass OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            //i had a better way to do it before but accidently permanently deleted it and cant remember how. This method is shorter and simpler
            OfflineJSON FoundItem = OfflineListJSON.OfflineWebsites.Find(x => x.ImageUrlJSON == SenderPost.ImageUrlJSON);
            OfflineListJSON.OfflineWebsites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(OfflineListJSON, Formatting.Indented);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadOffline();
        }

        private async void AddOffline(object sender, RoutedEventArgs e)
        {
            //  if (BoolWeb == true)
          
            var random = new Random();
            int randomnumber = random.Next();
            string rand = randomnumber.ToString();
            WebViewPage webpage = WebWeb.Tag as WebViewPage;
            webpage.CreateRTBOffline(rand);
            //   {
          
        }
        public async void OfflineAddToJSON(string rand)
        {
            StorageFile sampleFile = await localFolder.GetFileAsync("OfflinePages.json");

            var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            OfflineClass OfflineListJSON = JsonConvert.DeserializeObject<OfflineClass>(JSONData);
            Uri ArgsUri = new Uri(WebWeb.Source.ToString());
            string host = ArgsUri.Host;
            string x;
            x = await WebWeb.InvokeScriptAsync("eval", new string[] { "document.title;" });
            OfflineListJSON.OfflineWebsites.Add(new OfflineJSON()
            {
                FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                ImageUrlJSON = rand + ".jpg",
                HeaderJSON = x
            }); ;
            var SerializedObject = JsonConvert.SerializeObject(OfflineListJSON, Formatting.Indented);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            LoadOffline();
        }

        private async void OpenOffline(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            OfflineJSON SenderPost = DataContext as OfflineJSON;
            var m = new MessageDialog(SenderPost.ImageUrlJSON);
            await m.ShowAsync();
            WebViewPage.SingletonReference.Churros();
            ImageFrame.Visibility = Visibility.Visible;
            ImageFrame.Navigate(typeof(OfflineModePage));
            StorageFile sampleFile = await localFolder.GetFileAsync(SenderPost.ImageUrlJSON);
            var ms = new MessageDialog(sampleFile.Path.ToString());
                await ms.ShowAsync();
            OfflineModePage.OiMage.Source = new BitmapImage(new Uri(sampleFile.Path.ToString()));
            //  WebWeb.Navigate(new Uri(SenderPost.ImageUrlJSON));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            OfflineJSON SenderPost = DataContext as OfflineJSON;
            WebWeb.Navigate(new Uri(SenderPost.ImageUrlJSON));
        }
    }
}
