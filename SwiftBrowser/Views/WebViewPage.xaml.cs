using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SwiftBrowser.Views;
using Microsoft.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.UI.Popups;
using SwiftBrowser.Extensions;
using SwiftBrowser.Helpers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Networking.BackgroundTransfer;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;
using Windows.UI.Core;
using Windows.Web;
using Windows.Networking.Sockets;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using Windows.ApplicationModel.DataTransfer;
using SwiftBrowser.HubViews;
using SwiftBrowser.SettingsViews;
using WinUI = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Media;
using System.Drawing;
using Windows.UI.Xaml.Navigation;
using SwiftBrowser_Runtime_Component;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using System.Timers;
using System.Linq;
using Windows.UI.StartScreen;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Graphics.Display;

namespace SwiftBrowser.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private Uri _source;
        DarkMode DarkMode = new DarkMode();
        UserAgents UserAgents = new UserAgents();
        WebViewEvents WebViewEvents = new WebViewEvents();
        public static WebView WebviewControl { get; set; }
        public static TabView MainTab { get; set; }
        public static TabViewItem CurrentMainTab { get; set; }
        MenuFlyout ContextFlyout = new MenuFlyout();
        MenuFlyout ContextFlyoutImage = new MenuFlyout();
        string NewWindowLink;
        public static Button UserAgentbuttonControl { get; set; }
        public WebViewLongRunningScriptDetectedEventArgs e;
        public static TeachingTip InfoDialog { get; set; }
        public WebView webView;
        public static Grid HomeFrameFrame { get; set; }
        bool ShareIMG = false;
        public static Boolean IncognitoModeStatic { get; set; }
        public Boolean IncognitoMode;
        TabViewItem CurrentTab;
        public static string SourceToGo { get; set; }
        TabView TabviewMain;
        public WebViewPage()
        {
            UserAgentbuttonControl = UserAgentbutton;
            InfoDialog = InfoTip;
            InitializeComponent();
            Startup();
        }
        public void Startup()
        {
            if (IncognitoMode == true)
            {
                // await Task.Delay(2000);
                TabviewMain = MainTab;
                // await Task.Delay(5000);
                CurrentTab = CurrentMainTab;

                // GetCurrentTab();
                CurrentMainTab = null;
                MainTab = null;
            }
            else
            {
                TabviewMain = MainTab;
                CurrentTab = CurrentMainTab;

                // GetCurrentTab();
                CurrentMainTab = null;
                MainTab = null;
            }
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Open in new tab- beta" };
            firstItem.Click += FirstItem_Click;
            ContextFlyout.Items.Add(firstItem);
            MenuFlyoutItem WindowItem = new MenuFlyoutItem { Text = "Open in new window - beta" };
            WindowItem.Click += WindowItem_Click;
            ContextFlyout.Items.Add(WindowItem);
            MenuFlyoutItem IncognitoItem = new MenuFlyoutItem { Text = "Open in new incognito - beta" };
            ContextFlyout.Items.Add(IncognitoItem);
            IncognitoItem.Click += IncognitoItem_Click;
            MenuFlyoutItem AppItem = new MenuFlyoutItem { Text = "Open as app - beta" };
            AppItem.Click += AppItem_Click;
            ContextFlyout.Items.Add(AppItem);
            MenuFlyoutItem CopyItem = new MenuFlyoutItem { Text = "Copy link - beta" };
            CopyItem.Click += CopyItem_Click;
            ContextFlyout.Items.Add(CopyItem);
            MenuFlyoutItem CopyIMGItem = new MenuFlyoutItem { Text = "Copy image - beta" };
            CopyIMGItem.Click += CopyIMGItem_Click;
            MenuFlyoutItem SaveIMGItem = new MenuFlyoutItem { Text = "Save image as .png - beta" };
            SaveIMGItem.Click += SaveIMGItem_Click;
            MenuFlyoutItem ShareIMGItem = new MenuFlyoutItem { Text = "Share image - beta" };
            ShareIMGItem.Click += ShareIMGItem_Click;
            MenuFlyoutItem DevItem = new MenuFlyoutItem { Text = "DevTools" };
            ContextFlyout.Items.Add(DevItem);
            ContextFlyoutImage.Items.Add(CopyIMGItem);
            ContextFlyoutImage.Items.Add(SaveIMGItem);
            ContextFlyoutImage.Items.Add(ShareIMGItem);
            ContextFlyoutImage.Items.Add(DevItem);
            Window.Current.SizeChanged += SearchWebBox_SizeChanged;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            webView = new WebView(WebViewExecutionMode.SeparateProcess);
            webView.Name = "webView";
            webView.Height = 1000;
            webView.MinHeight = 300;
            webView.LongRunningScriptDetected += webView_LongRunningScriptDetected;
            webView.NavigationCompleted += OnNavigationCompleted;
            webView.ContentLoading += WebView_ContentLoading;
            Thickness Margin = webView.Margin;
            Margin.Top = 60;
            webView.Margin = Margin;
            webView.NavigationStarting += webView_NavigationStarting;
            //    webView.ScriptNotify += webView_ScriptNotify;
            //   webView.Loaded += WebView_Loaded;
            webView.DOMContentLoaded += WebView_DOMContentLoaded;
            webView.IsRightTapEnabled = true;
            webView.NewWindowRequested += webView_NewWindowRequested;
            webView.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            webView.UnsafeContentWarningDisplaying += webView_UnsafeContentWarningDisplaying;
            webView.UnsupportedUriSchemeIdentified += webView_UnsupportedUriSchemeIdentified;
            webView.UnviewableContentIdentified += webView_UnviewableContentIdentified;
            webView.NavigationFailed += OnNavigationFailed;
            WebviewControl = webView;
          //  webView.Loaded += webView_LoadCompleted;
            //   webView.SeparateProcessLost += WebView_SeparateProcessLost;
            ContentGrid.Children.Add(webView);
            /*  bool E = (bool)localSettings.Values["IncognitoMode"];
              try
              {*/
            if (IncognitoModeStatic == true)
            {
                IncognitoMode = true;
                localSettings.Values["IncognitoMode"] = false;
                if ((string)localSettings.Values["SourceToGo"] != null)
                {
                    FindName("webView");
                    try
                    {
                        webView.Navigate(new Uri((string)localSettings.Values["SourceToGo"]));
                    }
                    catch
                    {
                        webView.Navigate(new Uri((string)localSettings.Values["BackupSourceToGo"]));
                    }
                    localSettings.Values["SourceToGo"] = null;
                    localSettings.Values["BackupSourceToGo"] = null;
                    IsloadingPanel.Visibility = Visibility.Visible;
                    MenuFrameButton.Visibility = Visibility.Collapsed;
                    MenuButton.Visibility = Visibility.Visible;
                    Loading.IsActive = true;
                }
                else
                {
                    localSettings.Values["SourceToGo"] = null;
                    FindName("HomeFrame");
                    FindName("webView");
                    BackButton.IsEnabled = false;
                    ForwardButton.IsEnabled = false;
                    RefreshButton.IsEnabled = false;
                    ExtensionsButton.IsEnabled = false;
                    MenuFrameButton.Visibility = Visibility.Visible;
                    MenuButton.Visibility = Visibility.Collapsed;
                    MenuButton.IsEnabled = false;
                    HomeFrameFrameFrame.BackStack.Clear();
                    GC.Collect();
                    HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                }
            }
            else
            {
                IncognitoMode = false;
                if ((string)localSettings.Values["SourceToGo"] != null)
                {
                    FindName("webView");
                    IsloadingPanel.Visibility = Visibility.Visible;
                    MenuFrameButton.Visibility = Visibility.Collapsed;
                    MenuButton.Visibility = Visibility.Visible;
                    Loading.IsActive = true;
                   try
                   {
                        webView.Navigate(new Uri((string)localSettings.Values["SourceToGo"]));
                    }
                    catch
                    {
                        webView.Navigate(new Uri((string)localSettings.Values["BackupSourceToGo"]));
                    }
                    localSettings.Values["SourceToGo"] = null;
                    localSettings.Values["BackupSourceToGo"] = null;
                }
                else
                {
                    localSettings.Values["SourceToGo"] = null;
                    FindName("HomeFrame");
                    FindName("webView");
                    BackButton.IsEnabled = false;
                    ForwardButton.IsEnabled = false;
                    MenuFrameButton.Visibility = Visibility.Visible;
                    MenuButton.Visibility = Visibility.Collapsed;
                    RefreshButton.IsEnabled = false;
                    ExtensionsButton.IsEnabled = false;
                    MenuButton.IsEnabled = false;
                    HomeFrameFrameFrame.BackStack.Clear();
                    GC.Collect();
                    HomePage.WebViewControl = webView;
                    HomeFrameFrameFrame.Navigate(typeof(HomePage));
                }
            }
            /*  }
              catch
              {
                  IncognitoMode = false;
                  if ((string)localSettings.Values["SourceToGo"] != null)
                  {
                      FindName("webView");
                      webView.Navigate(new Uri((string)localSettings.Values["SourceToGo"]));
                      localSettings.Values["SourceToGo"] = null;
                      IsloadingPanel.Visibility = Visibility.Visible;
                      MenuFrameButton.Visibility = Visibility.Collapsed;
                      MenuButton.Visibility = Visibility.Visible;
                      Loading.IsActive = true;
                  }
                  else
                  {
                      localSettings.Values["SourceToGo"] = null;
                      FindName("HomeFrame");
                      FindName("webView");
                      BackButton.IsEnabled = false;
                      ForwardButton.IsEnabled = false;
                      MenuFrameButton.Visibility = Visibility.Visible;
                      MenuButton.Visibility = Visibility.Collapsed;
                      RefreshButton.IsEnabled = false;
                      ExtensionsButton.IsEnabled = false;
                      MenuButton.IsEnabled = false;
                      HomeFrameFrameFrame.BackStack.Clear();
                      GC.Collect();
                      HomeFrameFrameFrame.Navigate(typeof(HomePage));
                  }
              }*/
            SearchWebBox.Width = Window.Current.Bounds.Width - 300;
            WebviewControl = webView;
            HomeFrameFrame = HomeFrame;
            System.Timers.Timer RightTimer = new System.Timers.Timer();
            RightTimer.Elapsed += new ElapsedEventHandler(WebView_RightTapped);
            RightTimer.Interval = 50;
     RightTimer.Enabled = true;

        }
        private void ShareIMGItem_Click(object sender, RoutedEventArgs e)
        {
            ShareIMG = true;
            DataTransferManager.ShowShareUI();
        }

        private async void SaveIMGItem_Click(object sender, RoutedEventArgs e)
        {
           try
           {
            var savePicker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                savePicker.FileTypeChoices.Add("png image", new List<string>() { ".png" });
                savePicker.SuggestedFileName = "Saved image";        
                StorageFile sFile = await savePicker.PickSaveFileAsync();
                if (sFile != null)
                {
                    CachedFileManager.DeferUpdates(sFile);


                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(); // Create HttpClient
                byte[] bBuffer = await client.GetByteArrayAsync(new Uri(NewWindowLink)); // Download file
                using (Stream sstream = await sFile.OpenStreamForWriteAsync())
                    sstream.Write(bBuffer, 0, bBuffer.Length); // Save


                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(sFile);
                }
            }
            catch
            {
                var m = new MessageDialog("Could not save image");
                await m.ShowAsync();
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
        }
        private async void CopyIMGItem_Click(object sender, RoutedEventArgs e)
        {
           try { 
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            var CopyImage = RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink));
            IRandomAccessStream stream = await CopyImage.OpenReadAsync();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink)));
                Clipboard.SetContent(dataPackage);
            await stream.FlushAsync();
              }
               catch
               {
                   var m = new MessageDialog("could not copy this image. Images embedded in webpages cant be copied yet :(");
                  await m.ShowAsync();
               }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
        }

        private async void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            try
            {
                dataPackage.SetText(NewWindowLink);
            }
            catch
            {
                    var ms = new MessageDialog("Unfortunately we werent able to copy the link");
                    await ms.ShowAsync();
                
            }
            Clipboard.SetContent(dataPackage);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
        }

        private async void IncognitoItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = webView.Source + NewWindowLink;
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(IncognitoTabView));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
        }

        private async void WindowItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = webView.Source + NewWindowLink;
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(TabViewPage));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
        }

        private async void AppItem_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                WebApp.WebViewNavigationString = NewWindowLink;
                frame.Navigate(typeof(WebApp));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
        }

        public async void WebView_RightTapped(object source, ElapsedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async() =>
            { 
                //your code here
                var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                var x = pointerPosition.X - Window.Current.Bounds.X;
                var y = pointerPosition.Y;
                if (String.IsNullOrEmpty(ContextMenu.hrefLink) == false && ContextMenu.hrefLink.StartsWith("http") == true)
             {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ContextFlyout.Hide();
                    });
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ContextFlyoutImage.Hide();
                    });
                    //   TabviewMain.ContextFlyout = ContextFlyout;
                    FlyoutShowOptions ee = new FlyoutShowOptions();
                ee.Position = pointerPosition;
                ContextFlyout.ShowAt(TabviewMain,ee);
                    NewWindowLink = ContextMenu.hrefLink;
                ContextMenu.hrefLink = null;
           }
                if (String.IsNullOrEmpty(ContextMenu.SRC) == false && ContextMenu.SRC.StartsWith("http") == true)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ContextFlyout.Hide();
                    });
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ContextFlyoutImage.Hide();
                    });
                    //  TabviewMain.ContextFlyout = ContextFlyoutImage;
                    FlyoutShowOptions ee = new FlyoutShowOptions();
                    ee.Position = pointerPosition;
                    ContextFlyoutImage.ShowAt(TabviewMain, ee);
                    NewWindowLink = ContextMenu.SRC;
                    ContextMenu.SRC = null;
                }
            }); 
        }


        private async void FirstItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = webView.Source + NewWindowLink;
            NewTabItem_Click(sender, e);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
        }

        private async void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            NavigationTipGrid.Visibility = Visibility.Visible;
            NavigationTip.Text = "Content for " + args.Uri.ToString() + " has finished loading";
            string functionString = @"var anchors = document.querySelectorAll('a');      
     for (var i = 0; i < anchors.length; i += 1) {
           anchors[i].oncontextmenu = function (e) {
        var href = this.getAttribute('href');
 if (window.Context) {
window.Context.setHREFCombination(href);
}
           };
       }";
            // window.external.notify([oX.toString(), oY.toString(), href].toString());
            await webView.InvokeScriptAsync("eval", new string[] { functionString });
            string functionImageString = @"var anchors = document.querySelectorAll('img');      
     for (var i = 0; i < anchors.length; i += 1) {
           anchors[i].oncontextmenu = function (e) {
        var src = this.getAttribute('src');
 if (window.Context) {
window.Context.setSRCCombination(src);
}
           };
       }";
            // window.external.notify([oX.toString(), oY.toString(), href].toString());
            await webView.InvokeScriptAsync("eval", new string[] { functionImageString });
        }

        public static bool IsMobileSiteEnabled = false;
        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        private bool _isShowingFailedMessage;

        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    // IsLoading = false;
                }

                Set(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMesageVisibility;
   
        public Visibility FailedMesageVisibility
        {
            get { return _failedMesageVisibility; }
            set { Set(ref _failedMesageVisibility, value); }
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////zsa
        private async void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            NavigationTip.Text = "Navigation finished";
            NavigationTipGrid.Visibility = Visibility.Collapsed;
            IsloadingPanel.Visibility = Visibility.Collapsed;
            Loading.Visibility = Visibility.Collapsed;
            if (args.IsSuccess == true)
            {
                try
                {
                    string x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
                    CurrentTab.Header = x;
                }
                catch
                {
                    CurrentTab.Header = webView.Source;
                }
                NavigationTipGrid.Visibility = Visibility.Collapsed;
                NavigationTip.Text = "Navigation finishing...";
                Uri ArgsUri = new Uri(webView.Source.ToString());
                string host = ArgsUri.Host;
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.BitmapIconSource() { UriSource = new Uri("http://icons.duckduckgo.com/ip2/" + host + ".ico"), ShowAsMonochrome = false };
                RefreshButtonIcon.Glyph = "\uE72C";
                OnPropertyChanged(nameof(IsBackEnabled));
                OnPropertyChanged(nameof(IsForwardEnabled));
                if (webView.Source.ToString().Contains("https://www.ecosia.org/search?q="))
                {
                    string stre = webView.Source.ToString().Replace("https://www.ecosia.org/search?q=", string.Empty);
                    SearchWebBox.Text = stre;
                }
                else
                {
                    SearchWebBox.Text = webView.Source.ToString();
                }
            }
            else
            {
                if (webView.Source.ToString().Contains("https://www.ecosia.org/search?q=") == false)
                {
                    //CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Globe };
                    //   CurrentTab.Header = args.WebErrorStatus.ToString();
                    /*  ExtensionsButton.IsEnabled = false;
                      MenuButton.IsEnabled = false;
                      MenuFrameButton.Visibility = Visibility.Visible;
                      MenuButton.Visibility = Visibility.Collapsed;
                      RefreshButtonIcon.Glyph = "\uE72C";
                      FindName("InfoFrameGrid");
                      localSettings.Values["ErrorMessage"] = args.WebErrorStatus.ToString();
                      InfoFrame.Navigate(typeof(NavigationErrorFrame));*/
                }
            }
        /*    if (String.IsNullOrEmpty(ContextMenu.hrefLink) == false)
            {
                var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                var x = pointerPosition.X - Window.Current.Bounds.X;
                var y = pointerPosition.Y - Window.Current.Bounds.Y;
                FlyoutShowOptions e = new FlyoutShowOptions();
                e.Position = pointerPosition;
                ContextFlyout.ShowAt(webView, e);
                ContextMenu.hrefLink = null;
            }*/
        }

        private void OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            NavigationTip.Text = "Navigation failed";
            NavigationTipGrid.Visibility = Visibility.Collapsed;
            if (webView.Source.ToString().Contains("https://www.ecosia.org/search?q=") == false)
            {
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Globe };
                CurrentTab.Header = e.WebErrorStatus.ToString();
                /*  ExtensionsButton.IsEnabled = false;
                  MenuButton.IsEnabled = false;
                  MenuFrameButton.Visibility = Visibility.Visible;
                  MenuButton.Visibility = Visibility.Collapsed;
                  RefreshButtonIcon.Glyph = "\uE72C";
                  FindName("InfoFrameGrid");
                  localSettings.Values["ErrorMessage"] = e.WebErrorStatus.ToString();
                  InfoFrame.Navigate(typeof(NavigationErrorFrame));*/
            }
            else
            {
                webView.Navigate(new Uri("https://www.ecosia.org/search?q=" + SearchWebBox.Text));
            }
        }

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            IsShowingFailedMessage = false;

            webView.Refresh();
        }
        public bool IsBackEnabled
        {
            get { return webView.CanGoBack; }
        }

        public bool IsForwardEnabled
        {
            get { return webView.CanGoForward; }
        }

        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoBack == true)
            {
                webView.GoBack();
            }
        }

        private void OnGoForward(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward == true)
            {
                webView.GoForward();
            }
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RefreshButtonIcon.Glyph == "\uE711" && webView.CanGoBack == true)
                {
                    webView.GoBack();
                }
                else if (HomeFrame.Visibility == Visibility.Collapsed)
                {
                    webView.Refresh();
                }
            }
            catch
            {
                webView.Refresh();
            }
        }

        private async void OnOpenInBrowser(object sender, RoutedEventArgs e)
        {
            if (HomeFrame == null)
            {
                await Windows.System.Launcher.LaunchUriAsync(webView.Source);
            }

        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsSuggestionListOpen = false;
            if (sender.Text != "")
            {
                if (sender.Text.Contains("https:") || sender.Text.Contains("http:"))
                {
                    //args.Uri..ToString().Replace("https://", string.Empty)
                    ContextMenu winRTObject = new ContextMenu();
                    webView.AddWebAllowedObject("Context", winRTObject);
                    try {
                    webView.Navigate(new Uri(sender.Text));
                    }
                    catch
                    {
                        webView.Navigate(new Uri("https://www.ecosia.org/search?q=" + sender.Text));
                    }
                }
                else
                {
                    /* try
                     {
                         webView.Navigate(new Uri("https://" + sender.Text));
                     }
                     catch
                     {*/
                    webView.Navigate(new Uri("https://www.ecosia.org/search?q=" + sender.Text));
                    // }
                }
            }
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        private void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            WebViewEvents.webView_ContainsFullScreenElementChanged(sender, args);
        }

        private void webView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            WebViewEvents.webView_UnsupportedUriSchemeIdentified(sender, args);
        }

        private void webView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            WebViewEvents.webView_UnsafeContentWarningDisplaying(sender, args);
        }
        private void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Refresh };
            CurrentTab.Header = "Loading...";
            NavigationTipGrid.Visibility = Visibility.Visible;
            NavigationTip.Text = "Loading content for " + args.Uri.ToString();
        }

        private void WebView_SeparateProcessLost(WebView sender, WebViewSeparateProcessLostEventArgs args)
        {
            FindName("EmergencyFrame");
            Processlostframe.Navigate(typeof(PorcessLost));
            try
            {
                CurrentTab.Header = "Process Lost";
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Delete };
            }
            catch
            {
                return;
            }
            //never realized i spell wrong
        }
        private void webView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            WebViewEvents.webView_UnviewableContentIdentified(sender, args);
        }
        private void LaunchUnviewable(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            WebViewEvents.LaunchUnviewable(sender, args);
        }

        private void InfoTip_CloseButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            webView.GoBack();
        }

        private async void webView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            TabView ParentTab = TabviewMain;
            var newTab = new TabViewItem();
            Uri ArgsUri = new Uri(args.Uri.ToString());
            string host = ArgsUri.Host;
            newTab.IconSource = new Microsoft.UI.Xaml.Controls.BitmapIconSource() { UriSource = new Uri("http://icons.duckduckgo.com/ip2/" + host + ".ico"), ShowAsMonochrome = false }; ;
            string x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
            newTab.Header = args.Uri.ToString();
            // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            if (IncognitoMode == true)
            {
                WebViewPage.IncognitoModeStatic = false;
            }
            else
            {
                WebViewPage.IncognitoModeStatic = true;
            }
            localSettings.Values["SourceToGo"] = args.Uri.ToString();
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = TabviewMain;
            frame.Navigate(typeof(WebViewPage));
            ParentTab.TabItems.Add(newTab);
            ParentTab.SelectedItem = newTab;
            args.Handled = true;
        }

        private void webView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
            WebViewEvents.webView_LongRunningScriptDetected(sender, args);
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////


        private async void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            UnloadObject(HomeFrame);
            ContextMenu winRTObject = new ContextMenu();
            webView.AddWebAllowedObject("Context", winRTObject);
            InkingFrameGrid.Visibility = Visibility.Collapsed;
            BackButton.IsEnabled = true;
            ForwardButton.IsEnabled = true;
            RefreshButton.IsEnabled = true;
            ExtensionsButton.IsEnabled = true;
            MenuButton.IsEnabled = true;
            MenuFrameButton.Visibility = Visibility.Collapsed;
            MenuButton.Visibility = Visibility.Visible;
            NavigationTipGrid.Visibility = Visibility.Visible;
            NavigationTip.Text = "Navigation Starting...";
       

        var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            try
            {
                if (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                {
                    UnloadObject(InfoFrameGrid);
                    IsShowingFailedMessage = false;
                    RefreshButtonIcon.Glyph = "\uE711";
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Refresh };
                    CurrentTab.Header = "Starting...";
                }
            }
            catch
            {
                args.Cancel = true;
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Globe };
                CurrentTab.Header = "No internet";
                ExtensionsButton.IsEnabled = false;
                MenuButton.IsEnabled = false;
                MenuFrameButton.Visibility = Visibility.Visible;
                MenuButton.Visibility = Visibility.Collapsed;
                RefreshButtonIcon.Glyph = "\uE72C";
                FindName("InfoFrameGrid");
                InfoFrame.Navigate(typeof(NoInternetFrame));
            }
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        private void UserAgentbutton_Click(object sender, RoutedEventArgs e)
        {
            UserAgents.RequestUserAgent_Click(webView);
        }

        private async void ClearCachebutton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
        }

        private void ClearCookies_Click(object sender, RoutedEventArgs e)
        {
            // fix uri by getting all websites visited and clearing cookies through each website 
            /*  Uri bob = webView.ur
              HttpBaseProtocolFilter baseFilter = new HttpBaseProtocolFilter();
              foreach (var cookie in baseFilter.CookieManager.GetCookies(bob))
              {
                  baseFilter.CookieManager.DeleteCookie(cookie);
              }*/
        }

        private void DarkModebutton_Click(object sender, RoutedEventArgs e)
        {
            DarkMode.DarkMode_Click(webView);
        }

        private void HubButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            HubPage.NavString = "F";
            HubFrame.Navigate(typeof(HubPage));
            try
            {
                if (HomeFrame.IsLoaded == true)
                {
                    Favourites.BoolWeb = false;
                }
            }
            catch
            {
                Favourites.BoolWeb = true;
            }
        }
        private void HisButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            HubPage.NavString = "H";
            HubFrame.Navigate(typeof(HubPage));
            try
            {
                if (HomeFrame.IsLoaded == true)
                {
                    Favourites.BoolWeb = false;
                }
            }
            catch
            {
                Favourites.BoolWeb = true;
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            HubFrame.Navigate(typeof(SettingsHubPage));
        }

        private void DownlButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            HubPage.NavString = "D";
            HubFrame.Navigate(typeof(HubPage));
            try
            {
                if (HomeFrame.IsLoaded == true)
                {
                    Favourites.BoolWeb = false;
                }
            }
            catch
            {
                Favourites.BoolWeb = true;
            }
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UnloadObject(webView);
            }
            catch
            {
                UnloadObject(InfoFrameGrid);
            }
            SearchWebBox.Text = "";
            BackButton.IsEnabled = false;
            ForwardButton.IsEnabled = false;
            RefreshButton.IsEnabled = false;
            ExtensionsButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            InkingFrameGrid.Visibility = Visibility.Collapsed;
            FindName("HomeFrame");
            HomeFrame.Visibility = Visibility.Visible;
            HomeFrameFrameFrame.BackStack.Clear();
            GC.Collect();
            if (IncognitoMode == false)
            {
                HomePage.WebViewControl = webView;
                HomeFrameFrameFrame.Navigate(typeof(HomePage));
            }
            else
            {
                HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
            }
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
            CurrentTab.Header = "Home Tab";
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        public class SuggestionsClass
        {
            public List<string> suggestions { get; set; }
        }
        List<string> SuggestionsList;

        /*  public class Suggestions
           {
               public string Suggestion { get; set; }
           }
           */
        private async void SearchWebBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            try
            {
                if (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                {
                    SuggestionsList = new List<string>();
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(new Uri("https://ac.ecosia.org/autocomplete?q=" + sender.Text + "&type="));
                    string SuggestionsJSONString = await response.Content.ReadAsStringAsync();
                    SuggestionsClass SuggestionList = JsonConvert.DeserializeObject<SuggestionsClass>(SuggestionsJSONString);
                    foreach (string item in SuggestionList.suggestions)
                    {
                        /*  SuggestionsList.Add(

                              Suggestion = item.ToString()
                          );*/
                        SuggestionsList.Add(item.ToString());
                    }
                    sender.ItemsSource = SuggestionsList;
                    sender.IsSuggestionListOpen = true;
                }
            }
            catch
            {
                return;
            }
        }

        private void SearchWebBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //  var s = (FrameworkElement)args.SelectedItem.ToString();
            //  var D = args.SelectedItem.DataContext;
            //  var dse = args.SelectedItem as SuggestionsClass;

            // webView.Navigate(new Uri("https://www.ecosia.org/search?q=" + sender.Text));
            sender.Text = "https://www.ecosia.org/search?q=" + args.SelectedItem.ToString();
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////

        private async void Incognito_Click(object sender, RoutedEventArgs e)
        {
            /*   localSettings.Values["IncognitoMode"] = true;
               AppWindow appWindow = await AppWindow.TryCreateAsync();
               Frame appWindowContentFrame = new Frame();
               appWindowContentFrame.Navigate(typeof(IncognitoTabView));
               ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
               await appWindow.TryShowAsync();
               appWindow.Closed += delegate
               {
                   appWindowContentFrame.Content = null;
                   appWindow = null;
               };
           }*/
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(IncognitoTabView), null);
                Window.Current.Content = frame;
                  // You have to activate the window in order to show it later.
                  Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }


        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            /*  AppWindow appWindow = await AppWindow.TryCreateAsync();
              Frame appWindowContentFrame = new Frame();
              appWindowContentFrame.Navigate(typeof(TabViewPage));
              ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
              await appWindow.TryShowAsync();
              appWindow.Closed += delegate
              {
                  appWindowContentFrame.Content = null;
                  appWindow = null;
              };*/
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(TabViewPage));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void SearchWebBox_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            SearchWebBox.Width = Window.Current.Bounds.Width - 300;
        }

        private async void DevFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?ProductId=9mzbfrmz0mnj"));
        }

        private async void SaveWebPageAsHTMLFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            HttpClient httpclient = new HttpClient();
            var result = await httpclient.GetBufferAsync(new Uri(webView.Source.ToString()));
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Hyper Text Markup Language", new List<string>() { ".html" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "Saved webpage";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                await FileIO.WriteBufferAsync(file, result);
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    var m = new MessageDialog("File " + file.Name + " saved.");
                    await m.ShowAsync();
                }
                else
                {
                   var m = new MessageDialog("File " + file.Name + " couldn't be saved.");
                    await m.ShowAsync();
                }
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        private void ShareFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ShareIMG = false;
            DataTransferManager.ShowShareUI();
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if(ShareIMG == false)
            { 
            DataRequest request = args.Request;
            string x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
            request.Data.Properties.Title = x;
            Uri ArgsUri = new Uri(webView.Source.ToString());
            string host = ArgsUri.Host;
            request.Data.Properties.Description = host;
            request.Data.SetText(webView.Source.ToString());
            }
            else
            {
                DataRequest request = args.Request;
                string x = "Shared Image";
                request.Data.Properties.Title = x;
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink)));
            }
        }

        private void NewTabItem_Click(object sender, RoutedEventArgs e)
        {
            if (IncognitoMode == true)
            {
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                newTab.Header = "Home Tab";
                /*  MenuFlyout Flyout = new MenuFlyout();
                  MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                  OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                  OpenInnewwindow.Text = "Move to new window";
                  Flyout.Items.Add(OpenInnewwindow);

                  Flyout.Items.Add(new MenuFlyoutSeparator());
                  MenuFlyoutItem newtabF = new MenuFlyoutItem();
                  newtabF.Icon = new SymbolIcon(Symbol.Add);
                  newtabF.Click += AddAll;
                  newtabF.Text = "New Tab";
                  Flyout.Items.Add(newtabF);

                  MenuFlyoutItem newWindow = new MenuFlyoutItem();
                  newWindow.Icon = new SymbolIcon(Symbol.Add);
                  newWindow.Text = "New secondary window";
                  newWindow.Click += NewWindow_Click;
                  Flyout.Items.Add(newWindow);

                  MenuFlyoutItem newIncognito = new MenuFlyoutItem();
                  newIncognito.Icon = new SymbolIcon(Symbol.Add);
                  newIncognito.Text = "New incognito window";
                  newIncognito.Click += Incognito_Click;
                  Flyout.Items.Add(newIncognito);

                  Flyout.Items.Add(new MenuFlyoutSeparator());
                  MenuFlyoutItem CloseTab = new MenuFlyoutItem();
                  CloseTab.Icon = new SymbolIcon(Symbol.Delete);
                  CloseTab.Text = "Close Tab";
                  Flyout.Items.Add(CloseTab);
                  MenuFlyoutItem CloseO = new MenuFlyoutItem();
                  CloseO.Icon = new SymbolIcon(Symbol.Delete);
                  CloseO.Text = "Close other tabs";
                  Flyout.Items.Add(CloseO);
                  MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                  CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                  CloseAll.Text = "Close all tabs";
                  CloseAll.Click += ClearAll;
                  Flyout.Items.Add(CloseAll);

                  newTab.ContextFlyout = Flyout;*/
                // The Content of a TabViewItem is often a frame which hosts a page.
                Frame frame = new Frame();
                newTab.Content = frame;
                WebViewPage.IncognitoModeStatic = true;
                WebViewPage.CurrentMainTab = newTab;
                WebViewPage.MainTab = TabviewMain;
                frame.Navigate(typeof(WebViewPage));
                TabviewMain.TabItems.Add(newTab);
            }
            else
            {
                var newTab = new TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                newTab.Header = "Home Tab";
                /* MenuFlyout Flyout = new MenuFlyout();
                 MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                 OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                 OpenInnewwindow.Text = "Move to new window";
                 Flyout.Items.Add(OpenInnewwindow);

                 Flyout.Items.Add(new MenuFlyoutSeparator());
                 MenuFlyoutItem newtabF = new MenuFlyoutItem();
                 newtabF.Icon = new SymbolIcon(Symbol.Add);
                 newtabF.Text = "New Tab";
                 newtabF.Click += AddAll;
                 Flyout.Items.Add(newtabF);

                 MenuFlyoutItem newWindow = new MenuFlyoutItem();
                 newWindow.Icon = new SymbolIcon(Symbol.Add);
                 newWindow.Text = "New secondary window";
                 newWindow.Click += NewWindow_Click;
                 Flyout.Items.Add(newWindow);

                 MenuFlyoutItem newIncognito = new MenuFlyoutItem();
                 newIncognito.Icon = new SymbolIcon(Symbol.Add);
                 newIncognito.Text = "New incognito window";
                 newIncognito.Click += Incognito_Click;
                 Flyout.Items.Add(newIncognito);

                 Flyout.Items.Add(new MenuFlyoutSeparator());
                 MenuFlyoutItem CloseTab = new MenuFlyoutItem();
                 CloseTab.Icon = new SymbolIcon(Symbol.Delete);
                 CloseTab.Text = "Close Tab";
                 Flyout.Items.Add(CloseTab);
                 MenuFlyoutItem CloseO = new MenuFlyoutItem();
                 CloseO.Icon = new SymbolIcon(Symbol.Delete);
                 CloseO.Text = "Close other tabs";
                 Flyout.Items.Add(CloseO);
                 MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                 CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                 CloseAll.Text = "Close all tabs";
                 CloseAll.Click += ClearAll;
                 Flyout.Items.Add(CloseAll);

                 newTab.ContextFlyout = Flyout;*/
                // The Content of a TabViewItem is often a frame which hosts a page.
                Frame frame = new Frame();
                newTab.Content = frame;
                WebViewPage.IncognitoModeStatic = false;
                WebViewPage.CurrentMainTab = newTab;
                WebViewPage.MainTab = TabviewMain;
                frame.Navigate(typeof(WebViewPage));
                WebViewPage.IncognitoModeStatic = false;
                TabviewMain.TabItems.Add(newTab);
            }
        }

        private async void PrintFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
             var printHelper = new PrintHelper(PrintPanel);
             // Add controls that you want to print
         
             string x = "e";
             try
             {
                 x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
             }
             catch
             {
                 x = webView.Source.ToString();
             }
            Grid RectangleToPrint = new Grid();
          //  ItemsControl ee = new ItemsControl();
            var print = await GetWebPages(webView, new Windows.Foundation.Size(1200d, 1200d));
            //  RectangleToPrint.Children.Add(ee);
            //  ContentGrid.Children.Remove(webView);
            var pageNumber = 0;
            foreach (var item in print)
          {
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });



                // Static header

                var header = new TextBlock { Text = "Windows Community Toolkit Sample App - Print Helper - Custom Print", Margin = new Thickness(0, 0, 0, 20) };

                Grid.SetRow(header, 0);

                grid.Children.Add(header);



                // Main content with layout from data template
                Windows.UI.Xaml.Shapes.Rectangle Rect = item as Windows.UI.Xaml.Shapes.Rectangle;
                
                Grid.SetRow(Rect, 1);
            
                grid.Children.Add(Rect);



                // Footer with page number

                pageNumber++;

                var footer = new TextBlock { Text = string.Format("page {0}", pageNumber), Margin = new Thickness(0, 20, 0, 0) };

                Grid.SetRow(footer, 2);

                grid.Children.Add(footer);
                 printHelper.AddFrameworkElementToPrint(grid);
            }
         //   printHelper.AddFrameworkElementToPrint(webView);
            //printHelper.AddFrameworkElementToPrint(MyPrintPages);
            // webView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            // Start printing process

           await printHelper.ShowPrintUIAsync(x);

        }
    
        async Task<IEnumerable<FrameworkElement>> GetWebPages(WebView webView, Windows.Foundation.Size pageSize)
        {
            // GETTING WIDTH FROM WEVIEW CONTENT
            var widthFromView = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });

            int contentWidth;
            if (!int.TryParse(widthFromView, out contentWidth))
                throw new Exception(string.Format("failure/width:{0}", widthFromView));
            webView.Width = contentWidth;

            // GETTING HEIGHT FROM WEBVIEW CONTENT
            var heightFromView = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            int contentHeight;
            if (!int.TryParse(heightFromView, out contentHeight))
                throw new Exception(string.Format("failure/height:{0}", heightFromView));

            webView.Height = contentHeight;

            // CALCULATING NO OF PAGES
            var scale = pageSize.Width / contentWidth;
            var scaledHeight = (contentHeight * scale);
            var pageCount = (double)scaledHeight / pageSize.Height;
            pageCount = pageCount + ((pageCount > (int)pageCount) ? 1 : 0);

            // CREATE PAGES
            var pages = new List<Windows.UI.Xaml.Shapes.Rectangle>();
            for (int i = 0; i < (int)pageCount; i++)
            {
                var translateY = -pageSize.Height * i;
                var page = new Windows.UI.Xaml.Shapes.Rectangle
                {
                    Height = pageSize.Height,
                    Width = pageSize.Width,
                    Margin = new Thickness(5),
                    Tag = new TranslateTransform { Y = translateY },
                };

                page.Loaded += async (s, e) =>
                {
                    var rectangle = s as Windows.UI.Xaml.Shapes.Rectangle;
                    var wvBrush = await GetWebViewBrush(webView);
                    wvBrush.Stretch = Stretch.UniformToFill;
                    wvBrush.AlignmentY = AlignmentY.Top;
                    wvBrush.Transform = rectangle.Tag as TranslateTransform;
                    rectangle.Fill = wvBrush;
                };

                pages.Add(page);
                webView.Height = 1000;
            }
            return pages;
        }
        async Task<WebViewBrush> GetWebViewBrush(WebView webView)
        {
            // ASSING ORIGINAL CONTENT WIDTH
            var originalWidth = webView.Width;

            var widthFromView = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });

            int contentWidth;
            if (!int.TryParse(widthFromView, out contentWidth))
                throw new Exception(string.Format("failure/width:{0}", widthFromView));
          //  webView.Width = contentWidth;

            // ASSINGING ORIGINAL CONTENT HEIGHT
            var originalHeight = webView.Height;

            var heightFromView = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            int contentHeight;
            if (!int.TryParse(heightFromView, out contentHeight))
                throw new Exception(string.Format("failure/height:{0}", heightFromView));
           // webView.Height = contentWidth;

            // CREATING BRUSH
            var originalVisibilty = webView.Visibility;
            webView.Visibility = Windows.UI.Xaml.Visibility.Visible;

            var wvBrush = new WebViewBrush
            {
                SourceName = webView.Name,
                Stretch = Stretch.Uniform
            };
            wvBrush.Redraw();
           // await Task.Delay(1000);
          //  webView.Width = originalWidth;
          //  webView.Height = originalHeight;
            webView.Visibility = originalVisibilty;

            return wvBrush;
        }
        private void InkingButton_Click(object sender, RoutedEventArgs e)
        { 
            BackButton.IsEnabled = false;
            ForwardButton.IsEnabled = false;
            RefreshButton.IsEnabled = false;
            ExtensionsButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            InkingFrameGrid.Visibility = Visibility.Visible;
            GC.Collect();
                HomePage.WebViewControl = webView;
            inkingPage.WebView = webView;
               InkingFrame.Navigate(typeof(inkingPage));
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
            CurrentTab.Header = "Inking Tab";
            try
            {
                UnloadObject(webView);
            }
            catch
            {
                UnloadObject(InfoFrameGrid);
            }
        }

        private async void PinSecondaryLiveTileMenuFlyoutSubItemPinItem_Click(object sender, RoutedEventArgs e)
        {
            string x = "";
            try
            {
               x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
                CurrentTab.Header = x;
            }
            catch
            {
                CurrentTab.Header = webView.Source.ToString();
            }
            // Use a display name you like
            string displayName = x;
            Random rnd = new Random();
            string TileId = "pin" + rnd.Next().ToString();
            // Provide all the required info in arguments so that when user
            // clicks your tile, you can navigate them to the correct content
            string arguments = webView.Source.ToString();
            Uri ArgsUri = new Uri(webView.Source.ToString());
            string host = ArgsUri.Host;
            // Initialize the tile with required arguments
              SecondaryTile tile = new SecondaryTile(
                 TileId,
                  displayName,
                  arguments,
            square150x150Logo: new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                  TileSize.Default);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await tile.RequestCreateAsync();
         /*   var secondaryTile = new SecondaryTile("tileID",
 "App Name",
 "args",
 "tile",
 TileOptions.ShowNameOnLogo,
 new Uri("ms-appx:///Assets/Square150x150Logo.png")
            { RoamingEnabled = true };

            await secondaryTile.RequestCreateAsync();*/
        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
    }
    }

