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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
        public bool isfirst = true;
        public HomePage()
        {
            this.InitializeComponent();
        }
        private void Option2RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Visible;
        }

        private void Option1RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Collapsed;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
        }

        private async void TextUrl_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text.StartsWith("https://") == true)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = args.QueryText;
                FindName("WebViewHome");
                WebViewHome.Navigate(new Uri(args.QueryText));
                Home.Visibility = Visibility.Collapsed;
            }
            else
            {
                int duration = 3000;
                try { 
                TabViewPage.InAppNotificationMain.Show("Not a valid url", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Not a valid url", duration);
                }
            }
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

            FavouritesListQuick = new List<FavouritesJSON>();

            try
            {
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
            FavouritesListQuick.Clear();
            foreach (var item in FavouritesListsJSON.Websites)
            {
                FavouritesListQuick.Add(new FavouritesJSON()
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON,
                });
            }
            QuickPinnedGridView.ItemsSource = null;
            UnloadObject(QuickPinnedGridView);
            FindName("QuickPinnedGridView");
            QuickPinnedGridView.ItemsSource = FavouritesListQuick;
        }

        private void NewsHomePage_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            webViewControl.Navigate(args.Uri);
            WebViewPage.HomeFrameFrame.Visibility = Visibility.Collapsed;
            args.Handled = true;
        }

      
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FindName("ContentPivot");
            try
            {
                SearchBox.Width = Window.Current.Bounds.Width - 100;
                BackGroundimage.Width = Window.Current.Bounds.Width;
                BackGroundimage.Height = Window.Current.Bounds.Height + 800;
            }
            catch
            {
                try
                {
                    FindName("SearchBox");
                    SearchBox.Width = Window.Current.Bounds.Width - 100;
                    BackGroundimage.Width = Window.Current.Bounds.Width;
                    BackGroundimage.Height = Window.Current.Bounds.Height + 800;
                }
                catch
                {
                   
                        
                    
                }
            }
            LoadMore.Visibility = Visibility.Collapsed;

        }

        private void FavStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
          webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
           webViewControl.Navigate(new Uri((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] + sender.Text));
        }

        private void QGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            FavouritesJSON SenderPost = DataContext as FavouritesJSON;
            webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
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
                FavouritesListQuick.Clear();
                foreach (var item in FavouritesListsJSON.Websites)
                {
                    FavouritesListQuick.Add(new FavouritesJSON()
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON,
                    });
                }
                QuickPinnedGridView.ItemsSource = null;
                UnloadObject(QuickPinnedGridView);
                FindName("QuickPinnedGridView");
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
            catch
            {
                return;
            }
        }

        private void TICO_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = toggle.IsOn;
            if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] == true)
            {
                icon.Visibility = Visibility.Visible;
            }
            else
            {
                icon.Visibility = Visibility.Collapsed;
            }
        }

        private void TfAV_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = toggle.IsOn;
            if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] == true)
            {
                FavouritesGridView.Visibility = Visibility.Visible;
            }
            else
            {
                FavouritesGridView.Visibility = Visibility.Collapsed;
            }
        }

        private void TqUI_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = toggle.IsOn;
            if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] == true)
            {
                QuickPinnedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                QuickPinnedGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void TmOR_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = toggle.IsOn;
            if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] == true)
                {
                loadcontentmore.Visibility = Visibility.Visible;
            }
                else
            {
                loadcontentmore.Visibility = Visibility.Collapsed;
            }
            
        }

        private void TSea_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = toggle.IsOn;
            if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] == true)
            {
                SearchBox.Visibility = Visibility.Visible;
            }
            else
            {
                SearchBox.Visibility = Visibility.Collapsed;
            }
        }

        private void QuickPinnedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FavouritesJSON SenderPost = e.ClickedItem as FavouritesJSON;
            webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("Home");
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            if (isfirst == true)
            {
                isfirst = false;
                HomeGrid = FavouritesGridView;
                webViewControl = WebViewControl;
                try
                {
                    tICO.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                    if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] == true)
                    {
                        icon.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        icon.Visibility = Visibility.Collapsed;
                    }
                    TfAV.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"];
                    if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] == true)
                    {
                        FavouritesGridView.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        FavouritesGridView.Visibility = Visibility.Collapsed;
                    }
                    TqUI.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"];
                    if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] == true)
                    {
                        QuickPinnedGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        QuickPinnedGrid.Visibility = Visibility.Collapsed;
                    }
                    TmOR.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"];
                    if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] == true)
                    {
                        loadcontentmore.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        loadcontentmore.Visibility = Visibility.Collapsed;
                    }
                    TSea.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"];
                    if ((Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] == true)
                    {
                        SearchBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SearchBox.Visibility = Visibility.Collapsed;
                    }
                }
                catch
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
                    tICO.IsOn = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                    QuickPinnedGrid.Visibility = Visibility.Visible;
                    loadcontentmore.Visibility = Visibility.Visible;
                    FavouritesGridView.Visibility = Visibility.Visible;
                    icon.Visibility = Visibility.Visible;
                    TfAV.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"];
                    TqUI.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"];
                    TmOR.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"];
                    TSea.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"];
                    SearchBox.Visibility = Visibility.Visible;
                }

                try
                {
                    if ((bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] == true)
                    {
                        FindName("WebViewHome");
                        WebViewHome.Navigate(new Uri((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"]));
                        Option2RadioButton.IsChecked = true;
                        UnloadObject(Home);
                    }
                    else
                    {
                        Option1RadioButton.IsChecked = true;
                        UnloadObject(WebViewHome);
                        LoadFavorites();
                        LoadQuickPinned();
                    }
                }
                catch
                {
                    Option1RadioButton.IsChecked = true;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = false;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
                    LoadFavorites();
                    LoadQuickPinned();
                    UnloadObject(WebViewHome);
                }
               try
               {
                    if ((bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] == true)
                    {
                        f = true;
                        Imageoption.IsChecked = true;
                        BackGroundimage.Visibility = Visibility.Visible;
                        BitmapImage bitmapImage = new BitmapImage();                                            // dimension, so long as one dimension measurement is provided
                        bitmapImage.UriSource = new Uri((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"]);
                        BackGroundimage.Source = bitmapImage;
                        f = false;
                    }
                    else
                    {
                        DefaultacrylicOption.IsChecked = true;
                        BackGroundimage.Visibility = Visibility.Collapsed;
                        DefaultacrylicOption.IsChecked = true;
                    }
                }
                catch
                {
                    DefaultacrylicOption.IsChecked = true;
                    BackGroundimage.Visibility = Visibility.Collapsed;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
                }
            }
        }
        

        private async void SearchBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                SearchBox.Width = Window.Current.Bounds.Width - 100;
                BackGroundimage.Width = Window.Current.Bounds.Width;
                BackGroundimage.Height = Window.Current.Bounds.Height - 100;
            }
            catch
            {
               try
               {
                    FindName("SearchBox");
                    await Task.Delay(300);
                    SearchBox.Width = Window.Current.Bounds.Width - 100;
                    BackGroundimage.Width = Window.Current.Bounds.Width;
                    BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                }
               catch
               {
                    try
                    {
                        FindName("SearchBox");
                        await Task.Delay(300);
                        SearchBox.Width = Window.Current.Bounds.Width - 100;
                        BackGroundimage.Width = Window.Current.Bounds.Width;
                        BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                    }
                    catch
                    {
                        try
                        {
                            FindName("SearchBox");
                            await Task.Delay(300);
                            SearchBox.Width = Window.Current.Bounds.Width - 100;
                            BackGroundimage.Width = Window.Current.Bounds.Width;
                            BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                        }
                        catch
                        {
                            try
                            {
                                FindName("SearchBox");
                                await Task.Delay(300);
                                SearchBox.Width = Window.Current.Bounds.Width - 100;
                                BackGroundimage.Width = Window.Current.Bounds.Width;
                                BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                            }
                            catch
                            {
                                try
                                {
                                    FindName("SearchBox");
                                    await Task.Delay(300);
                                    SearchBox.Width = Window.Current.Bounds.Width - 100;
                                    BackGroundimage.Width = Window.Current.Bounds.Width;
                                    BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                                }
                                catch
                                {
                                    try
                                    {
                                        FindName("SearchBox");
                                        await Task.Delay(300);
                                        SearchBox.Width = Window.Current.Bounds.Width - 100;
                                        BackGroundimage.Width = Window.Current.Bounds.Width;
                                        BackGroundimage.Height = Window.Current.Bounds.Height - 100;
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DefaultacrylicOption_Checked(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
            BackGroundimage.Visibility = Visibility.Collapsed;
        }

        private async void Imageoption_Checked(object sender, RoutedEventArgs e)
        {
            if (f == false)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
                if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] == "")
                {
                    try
                    {
                    var picker = new Windows.Storage.Pickers.FileOpenPicker();
                    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                    picker.FileTypeFilter.Add(".jpg");
                    picker.FileTypeFilter.Add(".jpeg");
                    picker.FileTypeFilter.Add(".png");

                    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                    StorageFile File = await file.CopyAsync(localFolder);
                    BackGroundimage.Visibility = Visibility.Visible;
                    BitmapImage bitmapImage = new BitmapImage();                                            // dimension, so long as one dimension measurement is provided
                    bitmapImage.UriSource = new Uri(File.Path);
                    BackGroundimage.Source = bitmapImage;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = File.Path;
                        int duration = 3000;
                        try
                        {

                            TabViewPage.InAppNotificationMain.Show("Saved", duration);
                        }
                        catch
                        {
                            IncognitoTabView.InAppNotificationMain.Show("Saved", duration);
                        }
                    }
                    catch
                      {
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                        int duration = 3000;
                        try
                        {
                            
                            TabViewPage.InAppNotificationMain.Show("Canceled", duration);
                        }
                        catch
                        {
                            IncognitoTabView.InAppNotificationMain.Show("Canceled", duration);
                        }
                    }
                }
                else
                {
                    BackGroundimage.Visibility = Visibility.Collapsed;
                    BitmapImage bitmapImage = new BitmapImage();                                            // dimension, so long as one dimension measurement is provided
                    bitmapImage.UriSource = new Uri((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"]);
                    BackGroundimage.Source = bitmapImage;
                }
            }
        }
        bool f;
    }
}
