using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private bool f;
        private List<FavouritesJSON> FavouritesList;
        private List<FavouritesJSON> FavouritesListQuick;
        public bool isfirst = true;
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private WebView webViewControl;

        public HomePage()
        {
            InitializeComponent();
        }

        public static AdaptiveGridView HomeGrid { get; set; }
        public static WebView WebViewControl { get; set; }

        private void Option2RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Visible;
        }

        private void Option1RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Collapsed;
            ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
            ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
        }

        private async void TextUrl_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text.StartsWith("https://"))
            {
                ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
                ApplicationData.Current.LocalSettings.Values["CustomUrl"] = args.QueryText;
                FindName("WebViewHome");
                WebViewHome.Navigate(new Uri(args.QueryText));
                Home.Visibility = Visibility.Collapsed;
            }
            else
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Not a valid url", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Not a valid url", duration);
                }
            }
        }

        private async void LoadFavorites()
        {
            try
            {
                FavouritesList = new List<FavouritesJSON>();
                var sampleFile = await localFolder.GetFileAsync("Favorites.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                    FavouritesList.Add(new FavouritesJSON
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON
                    });
                FavouritesGridView.ItemsSource = FavouritesList;
            }
            catch
            {
                var JSONData = "e";
                var filepath = @"Assets\Favorites.json";
                var folder = Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(filepath); // error here
                //  StorageFile sfile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                //  await FileIO.WriteTextAsync(sfile, JSONData);
                JSONData = await FileIO.ReadTextAsync(file);
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
                var sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                    FavouritesListQuick.Add(new FavouritesJSON
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON
                    });
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
            catch
            {
                var JSONData = "e";
                var filepath = @"Assets\QuickPinWeb.json";
                var folder = Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(filepath);
                JSONData = await FileIO.ReadTextAsync(file);
                //  StorageFile sfile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                //  await FileIO.WriteTextAsync(sfile, JSONData);
                var sampleFile =
                    await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONData);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                    FavouritesListQuick.Add(new FavouritesJSON
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON
                    });
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
        }

        private async void DeleteFavButton_Click(object sender, RoutedEventArgs e)
        {
            var sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            var FoundItem = FavouritesListJSON.Websites.Find(x => x.UrlJSON == SenderPost.UrlJSON);
            FavouritesListJSON.Websites.Remove(FoundItem);
            var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, SerializedObject);
            var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
            var FavouritesListsJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONDatas);
            FavouritesListQuick.Clear();
            foreach (var item in FavouritesListsJSON.Websites)
                FavouritesListQuick.Add(new FavouritesJSON
                {
                    HeaderJSON = item.HeaderJSON,
                    UrlJSON = item.UrlJSON,
                    FavIconJSON = item.FavIconJSON
                });
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
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            webViewControl.Navigate(new Uri((string) ApplicationData.Current.LocalSettings.Values["SearchEngine"] +
                                            sender.Text));
        }

        private void QGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as FavouritesJSON;
            webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
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
                var sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                FavouritesListJSON.Websites.Add(new FavouritesJSON
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = UrlBox.Text,
                    HeaderJSON = Header
                });
                ;
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await FileIO.WriteTextAsync(sampleFile, SerializedObject);
                var JSONDatas = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListsJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONDatas);
                FavouritesListQuick.Clear();
                foreach (var item in FavouritesListsJSON.Websites)
                    FavouritesListQuick.Add(new FavouritesJSON
                    {
                        HeaderJSON = item.HeaderJSON,
                        UrlJSON = item.UrlJSON,
                        FavIconJSON = item.FavIconJSON
                    });
                QuickPinnedGridView.ItemsSource = null;
                UnloadObject(QuickPinnedGridView);
                FindName("QuickPinnedGridView");
                QuickPinnedGridView.ItemsSource = FavouritesListQuick;
            }
            catch
            {
            }
        }

        private void TICO_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeIcon"] = toggle.IsOn;
            if ((bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"])
                icon.Visibility = Visibility.Visible;
            else
                icon.Visibility = Visibility.Collapsed;
        }

        private void TfAV_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeFav"] = toggle.IsOn;
            if ((bool) ApplicationData.Current.LocalSettings.Values["HomeFav"])
                FavouritesGridView.Visibility = Visibility.Visible;
            else
                FavouritesGridView.Visibility = Visibility.Collapsed;
        }

        private void TqUI_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomePin"] = toggle.IsOn;
            if ((bool) ApplicationData.Current.LocalSettings.Values["HomePin"])
                QuickPinnedGrid.Visibility = Visibility.Visible;
            else
                QuickPinnedGrid.Visibility = Visibility.Collapsed;
        }

        private void TmOR_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeMore"] = toggle.IsOn;
            if ((bool) ApplicationData.Current.LocalSettings.Values["HomeMore"])
                loadcontentmore.Visibility = Visibility.Visible;
            else
                loadcontentmore.Visibility = Visibility.Collapsed;
        }

        private void TSea_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeSearch"] = toggle.IsOn;
            if ((bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"])
                SearchBox.Visibility = Visibility.Visible;
            else
                SearchBox.Visibility = Visibility.Collapsed;
        }

        private void QuickPinnedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var SenderPost = e.ClickedItem as FavouritesJSON;
            webViewControl.Navigate(new Uri(SenderPost.UrlJSON));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("Home");
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            if (isfirst)
            {
                isfirst = false;
                HomeGrid = FavouritesGridView;
                webViewControl = WebViewControl;
                try
                {
                    tICO.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                    if ((bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"])
                        icon.Visibility = Visibility.Visible;
                    else
                        icon.Visibility = Visibility.Collapsed;
                    TfAV.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeFav"];
                    if ((bool) ApplicationData.Current.LocalSettings.Values["HomeFav"])
                        FavouritesGridView.Visibility = Visibility.Visible;
                    else
                        FavouritesGridView.Visibility = Visibility.Collapsed;
                    TqUI.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomePin"];
                    if ((bool) ApplicationData.Current.LocalSettings.Values["HomePin"])
                        QuickPinnedGrid.Visibility = Visibility.Visible;
                    else
                        QuickPinnedGrid.Visibility = Visibility.Collapsed;
                    TmOR.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeMore"];
                    if ((bool) ApplicationData.Current.LocalSettings.Values["HomeMore"])
                        loadcontentmore.Visibility = Visibility.Visible;
                    else
                        loadcontentmore.Visibility = Visibility.Collapsed;
                    TSea.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"];
                    if ((bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"])
                        SearchBox.Visibility = Visibility.Visible;
                    else
                        SearchBox.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
                    ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
                    ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
                    ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
                    ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
                    tICO.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                    QuickPinnedGrid.Visibility = Visibility.Visible;
                    loadcontentmore.Visibility = Visibility.Visible;
                    FavouritesGridView.Visibility = Visibility.Visible;
                    icon.Visibility = Visibility.Visible;
                    TfAV.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeFav"];
                    TqUI.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomePin"];
                    TmOR.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeMore"];
                    TSea.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"];
                    SearchBox.Visibility = Visibility.Visible;
                }

                try
                {
                    if ((bool) ApplicationData.Current.LocalSettings.Values["CustomUrlBool"])
                    {
                        FindName("WebViewHome");
                        WebViewHome.Navigate(
                            new Uri((string) ApplicationData.Current.LocalSettings.Values["CustomUrl"]));
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
                    ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = false;
                    ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
                    LoadFavorites();
                    LoadQuickPinned();
                    UnloadObject(WebViewHome);
                }

                try
                {
                    if ((bool) ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"])
                    {
                        f = true;
                        Imageoption.IsChecked = true;
                        BackGroundimage.Visibility = Visibility.Visible;
                        var bitmapImage =
                            new BitmapImage(); // dimension, so long as one dimension measurement is provided
                        bitmapImage.UriSource =
                            new Uri((string) ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"]);
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
                    ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                    ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
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
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
            BackGroundimage.Visibility = Visibility.Collapsed;
        }

        private async void Imageoption_Checked(object sender, RoutedEventArgs e)
        {
            if (f == false)
            {
                ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
                if ((string) ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] == "")
                {
                    try
                    {
                        var picker = new FileOpenPicker();
                        picker.ViewMode = PickerViewMode.Thumbnail;
                        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".jpeg");
                        picker.FileTypeFilter.Add(".png");

                        var file = await picker.PickSingleFileAsync();
                        var File = await file.CopyAsync(localFolder);
                        BackGroundimage.Visibility = Visibility.Visible;
                        var bitmapImage =
                            new BitmapImage(); // dimension, so long as one dimension measurement is provided
                        bitmapImage.UriSource = new Uri(File.Path);
                        BackGroundimage.Source = bitmapImage;
                        ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = File.Path;
                        var duration = 3000;
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
                        ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                        var duration = 3000;
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
                    var bitmapImage = new BitmapImage(); // dimension, so long as one dimension measurement is provided
                    bitmapImage.UriSource =
                        new Uri((string) ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"]);
                    BackGroundimage.Source = bitmapImage;
                }
            }
        }

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
    }
}
