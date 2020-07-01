using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.UI.Popups;
using SwiftBrowser.Extensions;
using SwiftBrowser.Helpers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using SwiftBrowser.HubViews;
using SwiftBrowser.SettingsViews;
using WinUI = Microsoft.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Media;
using SwiftBrowser_Runtime_Component;
using Windows.UI.Xaml.Controls.Primitives;
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
using Windows.System.Diagnostics;
using Windows.System;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPack;
using System.Text;
using Windows.Media.SpeechSynthesis;
using Windows.ApplicationModel.UserActivities;
using SwiftBrowser.Assets;
using Windows.Networking.BackgroundTransfer;
using System.Threading;
using Windows.Web.Http.Filters;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using System.Security.Cryptography.X509Certificates;
using SwiftBrowser.Extensions.Views;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using WinRTXamlToolkit.Controls.Extensions;
using Windows.Media.SpeechRecognition;
using Windows.Media.Capture;
using Windows.UI.ViewManagement.Core;

namespace SwiftBrowser.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        bool Iscompact;
        DarkMode DarkMode = new DarkMode();
        UserAgents UserAgents = new UserAgents();
        WebViewEvents WebViewEvents = new WebViewEvents();
        public static WebView WebviewControl { get; set; }
        public static TabView MainTab { get; set; }
        public static TabViewItem CurrentMainTab { get; set; }
        public MenuFlyoutItem firstItem;
        MenuFlyout ContextFlyout = new MenuFlyout();
        MenuFlyout ContextFlyoutImage = new MenuFlyout();
        public bool navigated = true;
        string NewWindowLink;
        DataPackage WebviewDataPackage;
        public static Button UserAgentbuttonControl { get; set; }
        public WebViewLongRunningScriptDetectedEventArgs e;
        public string HighlightFunctionJS;
        public static TeachingTip InfoDialog { get; set; }
        public WebView webView;
        public static Grid HomeFrameFrame { get; set; }
        System.Timers.Timer  NavTimer;
        public bool dom = false;
        bool ShareIMG = false;
        public static Boolean IncognitoModeStatic { get; set; }
        MediaElement ReadAloudElement = new MediaElement();
        public Boolean IncognitoMode;
        TabViewItem CurrentTab;
        public static string SourceToGo { get; set; }
        string fileExtensionAdblockerText;
        public bool disposing = false;
        UserActivitySession _currentActivity;
        public static WebViewPage SingletonReference { get; set; }
        public MenuFlyoutItem WindowItem;
        public string SearchEngine = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"];
        StorageFile fileExtensionAdblocker;
        public MenuFlyoutItem IncognitoItem;
        public MenuFlyoutItem AppItem;
        public System.Timers.Timer MemoryTimer;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public System.Timers.Timer RightTimer;
        public MenuFlyoutItem CopyItem;
        public MenuFlyoutItem CopyIMGItem;
        public string StringToNavigate;
        public string BackupStringToNavigate;
        System.Timers.Timer TaskMNGTimer;
        public MenuFlyoutItem SaveIMGItem;
        public MenuFlyoutItem ShareIMGItem;
        public bool isfirst = true;
        public MenuFlyoutItem DevItem;
        TabView TabviewMain;
        public WebViewPage()
        {
            //  UserAgentbuttonControl = UserAgentbutton;
            InfoDialog = InfoTip;
            SingletonReference = this;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            StringToNavigate = (string)localSettings.Values["SourceToGo"];
            BackupStringToNavigate = (string)localSettings.Values["BackupSourceToGo"];
            localSettings.Values["SourceToGo"] = null;
            localSettings.Values["BackupSourceToGo"] = null;
            if (IncognitoMode == true)
            {
                // await Task.Delay(2000);
                TabviewMain = MainTab;
                // await Task.Delay(5000);
                CurrentTab = CurrentMainTab;


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
            InitializeComponent();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            try
            {
                if (webView.CanGoBack == true && HomeFrame.IsLoaded == false)
                {
                    webView.GoBack();
                }
                else if (webView.CanGoBack == true)
                {
                    navigated = false;
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
                }
                else if (HomeFrame.IsLoaded == true | InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
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

                }
                else
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
                    if (webView.CanGoForward == true)
                    {
                        IsForwardEnabled = true;
                    }
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                    }
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                    CurrentTab.Header = "Home Tab";
                }
            }
            catch
            {
                if (webView.CanGoBack == true && InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
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
                }
                else if (webView.CanGoBack == true)
                {
                    webView.GoBack();
                }
                else
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
                    if (webView.CanGoForward == true)
                    {
                        IsForwardEnabled = true;
                    }
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                    }
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                    CurrentTab.Header = "Home Tab";
                }
            }
        }

        public async void OnListenAsync(object sender, RoutedEventArgs e)
        {
          try
         {


                this.recognizer = new SpeechRecognizer();
                await this.recognizer.CompileConstraintsAsync();

                this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
                this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

                this.recognizer.UIOptions.AudiblePrompt = "Search with voice";
                this.recognizer.UIOptions.ExampleText = "what is the time.";
                this.recognizer.UIOptions.ShowConfirmation = true;
                this.recognizer.UIOptions.IsReadBackEnabled = true;
                this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

                var result = await this.recognizer.RecognizeWithUIAsync();

                if (result != null)
                {
                    ContextMenu winRTObject = new ContextMenu();
                    webView.AddWebAllowedObject("Context", winRTObject);
                    ConsoleLog winRTConsole = new ConsoleLog();
                    webView.AddWebAllowedObject("ConsoleWinRT", winRTConsole);

                    webView.Navigate(new Uri(SearchEngine + result.Text));
                    SearchWebBox.Text = result.Text;
                }
           }
            catch
            {
                try
                {
                    // Request access to the audio capture device.
                    MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
                    settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                    settings.MediaCategory = MediaCategory.Speech;
                    MediaCapture capture = new MediaCapture();

                    await capture.InitializeAsync(settings);
                }
                catch (TypeLoadException)
                {
                    // Thrown when a media player is not available.
                    int duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("Components unavailable", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("Components unavailable", duration);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Thrown when permission to use the audio capture device is denied.
                    // If this occurs, show an error or disable recognition functionality.
                    int duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("Access denied", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("Acces denied", duration);
                    }
                }
                catch (Exception exception)
                {
                    int duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("Error occured", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("Error occured", duration);
                    }
                }
        
            }
        }
        SpeechRecognizer recognizer;
    
    public async void Startup()
        {
            if (isfirst == true)
            {
                isfirst = false;
                SearchWebBox.Text = "";
                Window.Current.SizeChanged -= ViewElapsed;
                firstItem = new MenuFlyoutItem { Text = "Open in new tab- beta" };
                CurrentTab.Tag = webView;
                firstItem.Click += FirstItem_Click;
                ContextFlyout.Items.Add(firstItem);
                WindowItem = new MenuFlyoutItem { Text = "Open in new window - beta" };
                WindowItem.Click += WindowItem_Click;
                ContextFlyout.Items.Add(WindowItem);
                IncognitoItem = new MenuFlyoutItem { Text = "Open in new incognito - beta" };
                ContextFlyout.Items.Add(IncognitoItem);
                IncognitoItem.Click += IncognitoItem_Click;
                AppItem = new MenuFlyoutItem { Text = "Open as app - beta" };
                AppItem.Click += AppItem_Click;
                ContextFlyout.Items.Add(AppItem);
                CopyItem = new MenuFlyoutItem { Text = "Copy link - beta" };
                CopyItem.Click += CopyItem_Click;
                ContextFlyout.Items.Add(CopyItem);
                CopyIMGItem = new MenuFlyoutItem { Text = "Copy image - beta" };
                CopyIMGItem.Click += CopyIMGItem_Click;
                SaveIMGItem = new MenuFlyoutItem { Text = "Save image as .png - beta" };
                SaveIMGItem.Click += SaveIMGItem_Click;
                ShareIMGItem = new MenuFlyoutItem { Text = "Share image - beta" };
                ShareIMGItem.Click += ShareIMGItem_Click;
                DevItem = new MenuFlyoutItem { Text = "DevTools" };
                DevItem.Click += DevItem_Click;
                ContextFlyout.Items.Add(DevItem);
                ContextFlyoutImage.Items.Add(CopyIMGItem);
                ContextFlyoutImage.Items.Add(SaveIMGItem);
                ContextFlyoutImage.Items.Add(ShareIMGItem);
                ContextFlyoutImage.Items.Add(DevItem);
             //  Window.Current.SizeChanged += SearchWebBox_SizeChanged;
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
                await Task.Delay(200);
                try
                {
                    webView = new WebView(WebViewExecutionMode.SeparateProcess);
                }
                catch
                {
                    await Task.Delay(100);
                    try
                    {
                        webView = new WebView(WebViewExecutionMode.SeparateProcess);
                    }
                    catch
                    {
                        await Task.Delay(500);
                        try
                        {
                            webView = new WebView(WebViewExecutionMode.SeparateProcess);
                        }
                        catch
                        {
                            await Task.Delay(500);
                            try
                            {
                                webView = new WebView(WebViewExecutionMode.SeparateProcess);
                            }
                            catch
                            {
                                await Task.Delay(1000);
                                webView = new WebView(WebViewExecutionMode.SeparateProcess);
                            }
                        }
                    }
                }
                webView.Name = "webView";
                webView.LongRunningScriptDetected += webView_LongRunningScriptDetected;
                webView.NavigationCompleted += OnNavigationCompleted;
                webView.ContentLoading += WebView_ContentLoading;
                webView.Tag = this;
               // Thickness Margin = webView.Margin;
              //  Margin.Top = 60;
              //  webView.Margin = Margin;
                webView.NavigationStarting += webView_NavigationStarting;
              webView.Height = Window.Current.Bounds.Height - 89;
                webView.Width = Window.Current.Bounds.Width;
                //    webView.ScriptNotify += webView_ScriptNotify;
                //   webView.Loaded += WebView_Loaded;
                if(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "´Windows.Mobi")
                 {
                Bar.Visibility = Visibility.Collapsed;
                FindName("MobileBar");
               }
                webView.DOMContentLoaded += WebView_DOMContentLoaded;
                webView.IsRightTapEnabled = true;
                webView.PointerWheelChanged += WebView_PointerWheelChanged;
                webView.WebResourceRequested += WebView_WebResourceRequested;
                webView.Settings.IsJavaScriptEnabled = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["Javascript"];
                webView.Settings.IsIndexedDBEnabled = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["IndexDB"];
                webView.NewWindowRequested += webView_NewWindowRequested;
                webView.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
                webView.UnsafeContentWarningDisplaying += webView_UnsafeContentWarningDisplaying;
                webView.UnsupportedUriSchemeIdentified += webView_UnsupportedUriSchemeIdentified;
                webView.UnviewableContentIdentified += webView_UnviewableContentIdentified;
                webView.NavigationFailed += OnNavigationFailed;
                webView.FrameNavigationStarting += WebView_FrameNavigationStarting;
                webView.PermissionRequested += WebView_PermissionRequested;
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
                    if ((string)StringToNavigate != null)
                    {
                        FindName("webView");
                        IsloadingPanel.Visibility = Visibility.Visible;
                        MenuFrameButton.Visibility = Visibility.Collapsed;
                        MenuButton.Visibility = Visibility.Visible;
                        Loading.IsActive = true;
                        try
                        {
                            webView.Navigate(new Uri((string)StringToNavigate));
                        }
                        catch
                        {
                            try
                            {
                                webView.Navigate(new Uri((string)BackupStringToNavigate));
                            }
                            catch
                            {
                            }
                        }
                        ContextMenu winRTObject = new ContextMenu();
                        webView.AddWebAllowedObject("Context", winRTObject);
                        ConsoleLog winRTConsole = new ConsoleLog();
                        webView.AddWebAllowedObject("ConsoleWinRT", winRTConsole);
                        StringToNavigate = null;
                        BackupStringToNavigate = null;
                    }
                    else
                    {
                        StringToNavigate = null;
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                    }
                }
                else
                {
                    IncognitoMode = false;
                    if ((string)StringToNavigate != null)
                    {
                        FindName("webView");
                        IsloadingPanel.Visibility = Visibility.Visible;
                        MenuFrameButton.Visibility = Visibility.Collapsed;
                        MenuButton.Visibility = Visibility.Visible;
                        Loading.IsActive = true;
                        try
                        {
                            webView.Navigate(new Uri((string)StringToNavigate));
                        }
                        catch
                        {
                            try
                            {
                                webView.Navigate(new Uri((string)BackupStringToNavigate));
                            }
                            catch
                            {
                            }
                        }
                        ContextMenu winRTObject = new ContextMenu();
                        webView.AddWebAllowedObject("Context", winRTObject);
                        ConsoleLog winRTConsole = new ConsoleLog();
                        webView.AddWebAllowedObject("ConsoleWinRT", winRTConsole);
                        StringToNavigate = null;
                        BackupStringToNavigate = null;
                    }
                    else
                    {
                        StringToNavigate = null;
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(HomePage));
                    }
                }
                StorageFile HighlightFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///WebFiles/highlight.js"));
                HighlightFunctionJS = await FileIO.ReadTextAsync(HighlightFile);
                WebviewControl = webView;
                HomeFrameFrame = HomeFrame;
                RightTimer = new System.Timers.Timer();
                RightTimer.Elapsed += new ElapsedEventHandler(WebView_RightTapped);
                RightTimer.Interval = 500;
                RightTimer.Enabled = true;
                MemoryTimer = new System.Timers.Timer();
                MemoryTimer.Elapsed += new ElapsedEventHandler(WebView_Memory);
                try
                {
                    List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
                    StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile file = await folder.GetFileAsync("Extensions.json");// error here
                    var JSONData = "e";
                    JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                    ExtensionsClass ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
                    foreach (var item in ExtensionsListJSONJSON.Extensions)
                    {
                       // var m = new MessageDialog(item.DescriptionJSON + item.Page + item.Id.ToString());
                  //      await m.ShowAsync();
                          if (item.IsEnabledJSON == true && item.IsToolbar == true)
                         {
                              ExtensionsListJSON.Add(new ExtensionsJSON()
                              {
                                  NameJSON = item.NameJSON,
                                  DescriptionJSON = item.DescriptionJSON,
                                  IconJSON = item.IconJSON,
                                  Page = item.Page
                              });
                          }
                    }
                    ExtensionsListToolbar.ItemsSource = ExtensionsListJSON;
                }
                catch
                {

                }
                try
                {
                    int toolCount = ExtensionsListToolbar.Items.Count;
                    int math = toolCount * 35;
                    int total = math + 340;
                    SearchWebBox.Width = Window.Current.Bounds.Width - total;
                    try { 
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                    {
                       SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
                    }
                    }
                    catch
                    {

                    }
                }
                catch
                {
                    SearchWebBox.Width = 900;
                    try { 
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                    {
                        SearchWebMobileBox.Width = 900;
                    }
                }
                catch
                {

                }
            }
                string filepathx = @"Assets\AdblockerText.txt";
                StorageFolder folderx = Windows.ApplicationModel.Package.Current.InstalledLocation;
                fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                fileExtensionAdblockerText = await Windows.Storage.FileIO.ReadTextAsync(fileExtensionAdblocker);
            }
        }

        private void WebView_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            switch((int)args.PermissionRequest.PermissionType)
            {
                case 0:

                        break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }

        private void WebView_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint p = e.GetCurrentPoint(webView);
            PointerPointProperties pp = p.Properties;
            int mp = pp.MouseWheelDelta;
            if (mp > 0)
            {
           //     Scrollline.ScrollToVerticalOffset(Scrollline.VerticalOffset - 10);
            }
            else
            {
         //       Scrollline.ScrollToVerticalOffset(Scrollline.VerticalOffset + 10);
            }
        }

            private async void WebView_WebResourceRequested(WebView sender, WebViewWebResourceRequestedEventArgs args)
            {
                try
                {
                    if ((bool)localSettings.Values["AdBlocker"] == true)
                    {
                        try
                        {
                            if (fileExtensionAdblockerText.Contains(args.Request.RequestUri.Host) == true)
                            {
                                args.Response = new HttpResponseMessage(Windows.Web.Http.HttpStatusCode.Ok);
                            }
                        }
                        catch
                        {
                            string filepathx = @"Assets\AdblockerText.txt";
                            StorageFolder folderx = Windows.ApplicationModel.Package.Current.InstalledLocation;
                            fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                            fileExtensionAdblockerText = await Windows.Storage.FileIO.ReadTextAsync(fileExtensionAdblocker);
                        }
                    }
                }
                catch
                {

                }
            }
        

        public bool ee = false;
        private async void WebView_FrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                if ((bool)localSettings.Values["AdBlocker"] == true)
                {
                    try
                    {
                        if (fileExtensionAdblockerText.Contains(args.Uri.Host) == true)
                        {
                            args.Cancel = true;
                        }
                    }
                    catch
                    {
                        string filepathx = @"Assets\AdblockerText.txt";
                        StorageFolder folderx = Windows.ApplicationModel.Package.Current.InstalledLocation;
                        fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                        fileExtensionAdblockerText = await Windows.Storage.FileIO.ReadTextAsync(fileExtensionAdblocker);
                    }
                }
            }
            catch
            {

            }
        }

        private async void DevItem_Click(object sender, RoutedEventArgs e)
        {
            AppWindow appWindow = await AppWindow.TryCreateAsync();
            Frame appWindowContentFrame = new Frame();
            Devtools.DevView = webView;
            appWindowContentFrame.Navigate(typeof(Devtools));
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
            await appWindow.TryShowAsync();
            appWindow.Closed += delegate
            {
                appWindowContentFrame.Content = null;
                appWindow = null;
            };
        }

        private async void WebView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                int toolCount = ExtensionsListToolbar.Items.Count;
                int math = toolCount * 35;
                int total = math + 340;
                SearchWebBox.Width = Window.Current.Bounds.Width - total;
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                {
                    SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
                }
            }
            catch
            {
                try
                {
                    SearchWebBox.Width = 900;
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                    {
                        SearchWebMobileBox.Width = 900;
                    }
                }
                catch
                {

                }
            }
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                try
                {

                    int height;
                    var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                    if (!int.TryParse(heightString, out height))
                    {
                        throw new Exception("Unable to get page height");
                    }
                    webView.Height = Window.Current.Bounds.Height;// - 87;
                    webView.Width = Window.Current.Bounds.Width;
                }

                catch
                {
                    try
                    {
                        webView.Height = Window.Current.Bounds.Height; //- 89;
                        webView.Width = Window.Current.Bounds.Width;
                    }
                    catch
                    {

                    }
                }
            }
            else {
                if (ApplicationView.GetForCurrentView().IsFullScreenMode == true)
                {
                    try
                    {

                        int height;
                        var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                        if (!int.TryParse(heightString, out height))
                        {
                            throw new Exception("Unable to get page height");
                        }
                        webView.Height = Window.Current.Bounds.Height;// - 87;
                        webView.Width = Window.Current.Bounds.Width;
                    }

                    catch
                    {
                        try
                        {
                            webView.Height = Window.Current.Bounds.Height; //- 89;
                            webView.Width = Window.Current.Bounds.Width;
                        }
                        catch
                        {

                        }
                    }
                }
                else {
                    try
                    {

                        int height;
                        var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                        if (!int.TryParse(heightString, out height))
                        {
                            throw new Exception("Unable to get page height");
                        }
                        webView.Height = Window.Current.Bounds.Height - 87;
                        webView.Width = Window.Current.Bounds.Width;

                    }
                    catch
                    {
                        try
                        {
                            webView.Height = Window.Current.Bounds.Height - 89;
                            webView.Width = Window.Current.Bounds.Width;
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
        private async void WebView_Memory(object source, ElapsedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GC.Collect(2);
            });

        }
        private void ShareIMGItem_Click(object sender, RoutedEventArgs e)
        {
            ShareIMG = true;
            DataTransferManager.ShowShareUI();
        }
        private async void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string func = String.Format("document.documentElement.style.zoom = (1.0 - " + e.NewValue + " * 0.01); ");       //" function ZoomFunction(" + e.NewValue.ToString() + ") { var mybody = document.body;  if (Percentage < 100){  mybody.style.overflow = 'hidden'; NewWidth = (100 * OriginalWidth) / Percentage; NewHeight = (100 * OriginalHeight) / Percentage;  } else if (Percentage == 100) {mybody.style.overflow = 'hidden'; NewWidth = OriginalWidth; NewHeight = OriginalHeight;}  else{  mybody.style.overflow = 'auto'; NewWidth = OriginalWidth * (Percentage / 100);   NewHeight = OriginalHeight * (Percentage / 10);    } }";
            if (webView != null)
            {
                await webView.InvokeScriptAsync("eval", new string[] { func });
            }
        }
        public async void Dispose()
        {
            disposing = true;
            webView.NavigateToString("d");
            await Task.Delay(1000);
            webView.LongRunningScriptDetected -= webView_LongRunningScriptDetected;
            webView.NavigationCompleted -= OnNavigationCompleted;
            webView.ContentLoading -= WebView_ContentLoading;
            webView.NavigationStarting -= webView_NavigationStarting;
            webView.DOMContentLoaded -= WebView_DOMContentLoaded;
            webView.NewWindowRequested -= webView_NewWindowRequested;
            webView.ContainsFullScreenElementChanged -= webView_ContainsFullScreenElementChanged;
            webView.UnsafeContentWarningDisplaying -= webView_UnsafeContentWarningDisplaying;
            webView.UnsupportedUriSchemeIdentified -= webView_UnsupportedUriSchemeIdentified;
            webView.UnviewableContentIdentified -= webView_UnviewableContentIdentified;
            webView.NavigationFailed -= OnNavigationFailed;
            this.ContentGrid.Children.Remove(webView);
            VisualTreeHelper.DisconnectChildrenRecursive(webView);
            webView = null;
            GC.Collect();
            try
            {
                this.InfoTip.CloseButtonClick -= InfoTip_CloseButtonClick;
                this.FindTip.CloseButtonClick -= FindTip_CloseButtonClick;
                this.ReadTip.CloseButtonClick -= ReadTip_CloseButtonClick;
            }
            catch
            {
            }
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
            firstItem.Click -= FirstItem_Click;
            DevItem.Click -= DevFlyoutItem_Click;
            SaveIMGItem.Click -= SaveIMGItem_Click;
            ShareIMGItem.Click -= ShareIMGItem_Click;
            CopyIMGItem.Click -= CopyIMGItem_Click;
            AppItem.Click -= AppItem_Click;
            CopyItem.Click -= CopyItem_Click;
            WindowItem.Click -= WindowItem_Click;
           // Window.Current.SizeChanged -= SearchWebBox_SizeChanged;
            IncognitoItem.Click -= IncognitoItem_Click;
            MemoryTimer.Stop();
            RightTimer.Stop();
            MemoryTimer.Dispose();
            RightTimer.Dispose();
            this.SearchWebBox.TextChanged -= SearchWebBox_TextChanged;
            this.SearchWebBox.QuerySubmitted -= SearchBox_QuerySubmitted;
            this.SearchWebBox.SuggestionChosen -= SearchWebBox_SuggestionChosen;
            this.ContentGrid.Children.Clear();
            this.ContentControl.Header = null;
            try
            {
                UnloadObject(this.ContentControl);
            }
            catch
            {

            }
            this.Content = null;
            VisualTreeHelper.DisconnectChildrenRecursive(this);

            //   ((IDisposable)ContentGrid.Children).Dispose();
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
                int duration = 3000;
                try { 
                TabViewPage.InAppNotificationMain.Show("The image couldnt be saved, Please try again", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("The image couldnt be saved, Please try again", duration);
                }
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
            try
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                var CopyImage = RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink));
                IRandomAccessStream stream = await CopyImage.OpenReadAsync();
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink)));
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
                await stream.FlushAsync();
            }
            catch
            {
                int duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Couldnt copy the image", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Couldnt copy the image", duration);
                }
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
            await Task.Delay(1000);
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            try
            {
                dataPackage.SetText(NewWindowLink.ToString());
            }
            catch
            {
                int duration = 3000;
                try { 
                TabViewPage.InAppNotificationMain.Show("We couldnt copy the link", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("We couldnt copy the link", duration);
                }

            }
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
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
            var uriNewWindow = new Uri(@"swiftlaunchincognito:" + NewWindowLink);
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
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
            var uriNewWindow = new Uri(@"swiftlaunch:" + NewWindowLink);
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
            /* CoreApplicationView newView = CoreApplication.CreateNewView();
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
             bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);*/
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyout.Hide();
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContextFlyoutImage.Hide();
            });
        }
        private async void NewAppItem_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            string ws = webView.Source.ToString();
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                WebApp.WebViewNavigationString = ws;
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
        bool isf = false;
        public async void WebView_RightTapped(object source, ElapsedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (isf == false)
                {
                    isf = true;
                    try
                    {
                        //your code here
                        WebviewDataPackage = await webView.CaptureSelectedContentToDataPackageAsync();
                        if (WebviewDataPackage != null && ContextMenu.hrefLink == true)
                        {

                            try
                            {
                                var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                                var x = pointerPosition.X - Window.Current.Bounds.X;
                                var y = pointerPosition.Y;
                                DataPackageView d = WebviewDataPackage.GetView();
                                if (d.GetWebLinkAsync() != null)
                                {

                                    Uri u = await d.GetWebLinkAsync() as System.Uri;
                                    NewWindowLink = u.ToString();
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContextFlyout.Hide();
                                    });
                                    //   TabviewMain.ContextFlyout = ContextFlyout;
                                    FlyoutShowOptions ee = new FlyoutShowOptions();
                                    ee.Position = pointerPosition;
                                    ContextFlyout.ShowAt(TabviewMain, ee);
                                    ContextMenu.hrefLink = false;
                                    WebviewDataPackage = null;
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContextFlyoutImage.Hide();
                                    });
                                    isf = false;
                                }
                                else if (d != null && ContextMenu.SRC != null)
                                {
                                    isf = true;
                                    NewWindowLink = ContextMenu.SRC;
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
                                    ContextMenu.SRC = null;
                                    WebviewDataPackage = null;
                                    isf = false;
                                }
                                else if (ContextMenu.hrefLink == true)
                                {
                                    Uri u = await d.GetWebLinkAsync() as System.Uri;
                                    NewWindowLink = u.ToString();
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContextFlyout.Hide();
                                    });
                                    FlyoutShowOptions ee = new FlyoutShowOptions();
                                    ee.Position = pointerPosition;
                                    ContextFlyout.ShowAt(TabviewMain, ee);
                                    ContextMenu.hrefLink = false;
                                    WebviewDataPackage = null;
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        ContextFlyoutImage.Hide();
                                    });
                                    isf = false;
                                }
                            }
                            catch
                            {

                            }
                            isf = false;
                        }
                    }
                    catch
                    {
                        isf = false;
                    }
                    isf = false;
                }
            });
        }
        public async void ViewElapsed(object sender, WindowSizeChangedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode == true)
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                Thickness Thicc = new Thickness();
                Thicc.Top = -100;
                TabviewMain.Margin = Thicc;
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                Thickness Thicc = new Thickness();
                Thicc.Top = -10;
                TabviewMain.Margin = Thicc;
            }
        }
        private async void FirstItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = NewWindowLink;//webView.Source + NewWindowLink;
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

            if (webView.CanGoForward == true)
            {
                IsForwardEnabled = true;
            }
            IsBackEnabled = true;
            if (disposing == false)
            {
                NavigationTip.Text = "Navigation finished";
                NavigationTipGrid.Visibility = Visibility.Collapsed;
                try
                {

                    string ConsoleFunctionString = @"window.onerror = function(error, url, line) {var r = 'ERR:' + error + ' url' + url + ' Line: ' + line; Window.ConsoleWinRT.setLogCombination(r); };";
                    await webView.InvokeScriptAsync("eval", new string[] { ConsoleFunctionString });
                    string functionString = @"document.oncontextmenu = function (e) {
                       window.Context.setHREFCombination();
                     };";
                    await webView.InvokeScriptAsync("eval", new string[] { functionString });
                    if ((bool)localSettings.Values["DarkMode"] == true)
                    {
                        DarkMode.DarkMode_Click(webView);
                    }

                    /*   string functionImageString = @"var anchors = document.querySelectorAll('img');      
        for (var i = 0; i < anchors.length; i += 1) {
              anchors[i].oncontextmenu = function (e) {
           var src = this.getAttribute('src');
    if (window.Context) {
   window.Context.setSRCCombination(src);
   }
              };
          }";*/
                    // window.external.notify([oX.toString(), oY.toString(), href].toString());
                    //    await webView.InvokeScriptAsync("eval", new string[] { functionImageString });
                    int height;
                    var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                    if (!int.TryParse(heightString, out height))
                    {
                        throw new Exception("Unable to get page height");
                    }
                webView.Height = Window.Current.Bounds.Height - 87;
                    webView.Width = Window.Current.Bounds.Width;
                    DOMloaded();
                    await GenerateActivityAsync();

                }
                catch
                {
                    NavigationTip.Text = "Navigation finished";

                    NavigationTipGrid.Visibility = Visibility.Collapsed;
                }
                SearchWebBox.ItemsSource = null;
            }
        }
        private async Task GenerateActivityAsync()
        {

            // Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
            if (IncognitoMode == false && (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["StoreHistory"] == true)
            {
                UserActivityChannel channel = UserActivityChannel.GetDefault();
                UserActivity userActivity = await channel.GetOrCreateUserActivityAsync(webView.Source.ToString());
                string x;
                try
                {
                    x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
                }
                catch
                {
                    x = webView.Source.ToString();
                }
                // Populate required properties
                userActivity.VisualElements.DisplayText = x;
                userActivity.VisualElements.Description = webView.Source.ToString();
                userActivity.ActivationUri = new Uri(webView.Source.ToString());

                //Save
                await userActivity.SaveAsync(); //save the new metadata

                // Dispose of any current UserActivitySession, and create a new one.
                _currentActivity?.Dispose();
                _currentActivity = userActivity.CreateSession();
            }
        }
        public async void DOMloaded()
        {
            // try
            // {

            CurrentTab.Tag = webView;
            string x;
            try
            {
                x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
                CurrentTab.Header = x;
            }
            catch
            {
                CurrentTab.Header = webView.Source;
                x = webView.Source.ToString();
            }
            try
            {
                if (IncognitoMode == false && (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["StoreHistory"] == true)
                {
                    DataAccess.AddData(webView.Source.ToString(), x);
                }
            }
            catch
            {

            }
            try
            {
                Uri ArgsUri = new Uri(webView.Source.ToString());
                string host = ArgsUri.Host;
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.BitmapIconSource() { UriSource = new Uri("http://icons.duckduckgo.com/ip2/" + host + ".ico"), ShowAsMonochrome = false };
                RefreshButtonIcon.Glyph = "\uE72C";
                OnPropertyChanged(nameof(IsBackEnabled));
                OnPropertyChanged(nameof(IsForwardEnabled));
                if (webView.Source.ToString().Contains(SearchEngine))
                {
                    string stre = webView.Source.ToString().Replace(SearchEngine, string.Empty);
                    SearchWebBox.Text = stre;
                    TextBox textBox = SearchWebBox.GetDescendantsOfType<TextBox>().FirstOrDefault();
                    textBox.Select(0, 0);
                }
                else
                {
                    SearchWebBox.Text = webView.Source.ToString();
                    TextBox textBox = SearchWebBox.GetDescendantsOfType<TextBox>().FirstOrDefault();
                    textBox.Select(0, 0);
                }
            }
            catch
            {
                RefreshButtonIcon.Glyph = "\uE72C";
                OnPropertyChanged(nameof(IsBackEnabled));
                OnPropertyChanged(nameof(IsForwardEnabled));
            }
            // }
            // catch
            // {

            //}
            NavTimer.Stop();
            NavTimer.Enabled = false;
        }
        public static bool IsMobileSiteEnabled = false;
        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {


            if (disposing == false)
            {
                if (IncognitoMode == true)
                {
                    Uri gotouri = args.Uri;
                    HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
                    HttpCookieManager cookieManager = myFilter.CookieManager;
                    myFilter.ClearAuthenticationCache();
                    myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    myFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;
                    myFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.Default;
                    HttpCookieCollection myCookieJar = cookieManager.GetCookies(gotouri);
                    foreach (HttpCookie cookie in myCookieJar)
                    {
                        cookieManager.DeleteCookie(cookie);
                    }
                    myFilter.Dispose();
                }
                else
                {
                }
                if (args.IsSuccess == false)
                {
                    if (webView.Source.ToString().Contains(SearchEngine) == false)
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
            try
            {
                NavTimer.Stop();
                NavTimer.Enabled = false;
            }
            catch
            {
                return;
            }
            navigated = true;

        }

        private void OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            try
            {
                NavTimer.Stop();
                NavTimer.Enabled = false;
            }
            catch
            {
                return;
            }
            navigated = true;
            if (disposing == false)
            {
                NavigationTip.Text = "Navigation failed";
                NavigationTipGrid.Visibility = Visibility.Collapsed;
                if (webView.Source.ToString().Contains(SearchEngine) == false)
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
                    webView.Navigate(new Uri(SearchEngine + SearchWebBox.Text));
                }
            }
        }

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            //   IsShowingFailedMessage = false;

            webView.Refresh();
        }

        public bool IsBackEnabled = false;


        public bool IsForwardEnabled = false;

  
        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webView.CanGoBack == true && HomeFrame.IsLoaded == false)
                {
                    webView.GoBack();
                }
                else if (webView.CanGoBack == true)
                {
                    navigated = false;
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
                }
                else if (HomeFrame.IsLoaded == true | InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
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

                }
                else
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
                    if (webView.CanGoForward == true)
                    {
                        IsForwardEnabled = true;
                    }
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                    }
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                    CurrentTab.Header = "Home Tab";
                }
            }
            catch
            {
                if (webView.CanGoBack == true && InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
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
                }
                else if (webView.CanGoBack == true)
                {
                    webView.GoBack();
                }
                else
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
                    if (webView.CanGoForward == true)
                    {
                        IsForwardEnabled = true;
                    }
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
                        HomePage.WebViewControl = webView;
                        HomeFrameFrameFrame.Navigate(typeof(Incognitomode));
                    }
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                    CurrentTab.Header = "Home Tab";
                }
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
                try
                {
                    webView.Refresh();
                }
                catch
                {

                }
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
            if (args.QueryText != "")
            {
                ContextMenu winRTObject = new ContextMenu();
                webView.AddWebAllowedObject("Context", winRTObject);
                ConsoleLog winRTConsole = new ConsoleLog();
                webView.AddWebAllowedObject("ConsoleWinRT", winRTConsole);
                string result = Regex.Match(args.QueryText, @"(.{4})\s*$").ToString();
                if (args.QueryText.Contains(" ") == false)
                {
                    if (result.Contains(".") == true || args.QueryText.Contains("/"))
                    {
                        if (args.QueryText.StartsWith("http://") || args.QueryText.StartsWith("https://")
                            || args.QueryText.StartsWith("ftp://"))
                        {
                            webView.Navigate(new Uri(args.QueryText));
                        }
                        else
                        {
                            webView.Navigate(new Uri("http://" + args.QueryText));
                        }
                    }
                    else
                    {
                        webView.Navigate(new Uri(SearchEngine + args.QueryText));
                    }
                }
                else
                {
                    webView.Navigate(new Uri(SearchEngine + args.QueryText));
                }
                //  webView.Navigate(new Uri("ms-appx-web:///HomeView/surf.mhtml"));
            }
        //    Scrollline.ScrollToVerticalOffset(0);
            // webView.Navigate(new Uri("ms-appx-web:///HomeView/surf.mhtml"));

        }
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        private async void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            // WebViewEvents.webView_ContainsFullScreenElementChanged(sender, args);
            var applicationView = ApplicationView.GetForCurrentView();
            if (sender.ContainsFullScreenElement)
            {
                if (Iscompact == false)
                {
                    applicationView.TryEnterFullScreenMode();
                     Thickness Thicc = new Thickness();
                      Thicc.Top = -37;
                    TabviewMain.Margin = Thicc;
                  //  TabviewMain.VerticalContentAlignment = VerticalAlignment.Stretch;
                   // TabviewMain.VerticalAlignment = VerticalAlignment.Stretch;
                    Bar.Height = 0;
                  //  ContentGrid.VerticalAlignment = VerticalAlignment.Stretch;
                  //  ContentControl.VerticalAlignment = VerticalAlignment.Stretch;
                //    Canvas.SetZIndex(webView, 1000);
                    try
                    {
                        int height;
                        var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                        if (!int.TryParse(heightString, out height))
                        {
                            throw new Exception("Unable to get page height");
                        }
                        webView.Height = Window.Current.Bounds.Height;
                        webView.Width = Window.Current.Bounds.Width;
                    }
                    catch
                    {
                        try
                        {
                            webView.Height = Window.Current.Bounds.Height;
                            webView.Width = Window.Current.Bounds.Width;
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    Thickness Thicc = new Thickness();
                    Thicc.Top = -37;
                    TabviewMain.Margin = Thicc;
                    Iscompact = true;
                    CompactButton.Visibility = Visibility.Visible;
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                }
            }
            else if (applicationView.IsFullScreenMode)
            {
                if (Iscompact == false)
                {
                    applicationView.ExitFullScreenMode();
                    Thickness Thicc = new Thickness();
                    Thicc.Top = -8.5;
                   TabviewMain.Margin = Thicc;
                    Bar.Visibility = Visibility.Visible;
                    Thickness thicc = new Thickness();
                    thicc.Top = 0;
                    webView.Margin = thicc;
                    Bar.Height = 60;
                     await Task.Delay(1350);
                    //  TabviewMain.Height = webView.Height - 300;
                    //   webView.Height = webView.Height - 300;
                    try
                    {
                        int height;
                        var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                        if (!int.TryParse(heightString, out height))
                        {
                            throw new Exception("Unable to get page height");
                        }
                        webView.Height = Window.Current.Bounds.Height - 87;
                        webView.Width = Window.Current.Bounds.Width;
                    }
                    catch
                    {
                        try
                        {
                            webView.Height = Window.Current.Bounds.Height - 89;
                            webView.Width = Window.Current.Bounds.Width;
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    Thickness Thicc = new Thickness();
                    Thicc.Top = -8.5;
                    TabviewMain.Margin = Thicc;
                    Iscompact = true;
                    CompactButton.Visibility = Visibility.Visible;
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                }
            }

        }
        public async void CreateWebViewBitmap()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(webView);
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            StorageFile file = await localFolder.CreateFileAsync("xyzyx" + ".jpg");
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)rtb.PixelWidth,
                                     (uint)rtb.PixelHeight,
                                      displayInformation.RawDpiX,
                         displayInformation.RawDpiY,
                                     pixels);
                await encoder.FlushAsync();
            }
        }
        private async void webView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {

            await Windows.System.Launcher.LaunchUriAsync(args.Uri);

        }

        private void webView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            WebViewEvents.webView_UnsafeContentWarningDisplaying(sender, args);
        }
        private void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            if (disposing == false)
            {
                try
                {
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Refresh };
                    CurrentTab.Header = "Loading...";
                    NavigationTipGrid.Visibility = Visibility.Visible;
                    NavigationTip.Text = "Loading content for " + args.Uri.ToString();
                    navigated = false;
                    NavTimer = new System.Timers.Timer();
                    NavTimer.Elapsed += new ElapsedEventHandler(WebView_NavCompleted);
                    NavTimer.Interval = 8000;
                    NavTimer.Enabled = true;
                }
                catch
                {

                }
            }
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
        Uri downloadSource;
        private void webView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            try
            {
                downloadSource = args.Uri;
                if(downloadSource.ToString().EndsWith(".pdf"))
                {
                    MainFlyout.Hide();
                    ExtensionsButton.IsEnabled = false;
                    MenuButton.IsEnabled = false;
                    MenuFrameButton.Visibility = Visibility.Visible;
                    MenuButton.Visibility = Visibility.Collapsed;
                    InkingFrameGrid.Visibility = Visibility.Visible;
                    GC.Collect();
                    HomePage.WebViewControl = webView;
                    PdfPage.STATICURI = downloadSource.ToString();
                    InkingFrame.Navigate(typeof(PdfPage));
                    CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };
                    CurrentTab.Header = "Online pdf document";
                    try
                    {
                        UnloadObject(webView);
                    }
                    catch
                    {
                        UnloadObject(InfoFrameGrid);
                    }
                }
                else
                    { 
                DownloadNotification.Show();
                }
            }
            catch
            {

            }
            // WebViewEvents.webView_UnviewableContentIdentified(sender, args);
            /*      Uri source = new Uri(args.Uri.ToString().Trim());
               string path2 = String.Format("{0}{1}{2}{3}", source.Scheme, Uri.SchemeDelimiter, source.Authority, source.AbsolutePath);
               if (Path.HasExtension(path2) == true)
               {
                   var m = new MessageDialog(source.AbsoluteUri);
                   await m.ShowAsync();
                //   WebClient webClient = new WebClient();*/
            //  webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            /* webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
             webClient.DownloadFileAsync(new Uri(source.AbsoluteUri), System.IO.Path.GetFileName(source.LocalPath));
             while (webClient.IsBusy) { }*/
            /* FolderPicker picker = new FolderPicker { SuggestedStartLocation = PickerLocationId.Downloads };
             picker.FileTypeFilter.Add("*");
             StorageFolder folder = await picker.PickSingleFolderAsync();
             //  StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(args.Uri);
             if (folder != null)
             {
                 StorageFile testfile = await folder.CreateFileAsync("System.IO.Path.GetFileName(source.LocalPath)", CreationCollisionOption.OpenIfExists);
                 /*  WebClient downloader = new WebClient();
                   downloader.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloader_DownloadFileCompleted);
                   downloader.DownloadProgressChanged +=
                       new DownloadProgressChangedEventHandler(Downloader_DownloadProgressChanged);
                   downloader.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                 downloader.DownloadFile(new Uri(source.AbsoluteUri), @"C:\Downloads\" + System.IO.Path.GetFileName(args.Uri.LocalPath));
                   while (downloader.IsBusy) { }*/

            /*    BackgroundDownloader downloader = new BackgroundDownloader();
                 DownloadOperation download = downloader.CreateDownload(new Uri(path2), testfile);
                var md = new MessageDialog(download.ResultFile.FileType + download.ResultFile.Name + download.ResultFile.ContentType);
                await md.ShowAsync();
                await download.StartAsync();
                while (download.Progress.Status != BackgroundTransferStatus.Completed)
                {
                    string pro = String.Format("{0} of {1} kb. downloaded - %{2} complete.", download.Progress.BytesReceived / 1024, download.Progress.TotalBytesToReceive / 1024, download.Progress.Status.ToString());
                    var me = new MessageDialog(pro);
                    await me.ShowAsync();
                }
            }
        }
        else
        {
            await Windows.System.Launcher.LaunchUriAsync(args.Uri);
        }*/
        }

        private async void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // print progress of download.
            var m = new MessageDialog(e.BytesReceived + " " + e.ProgressPercentage);
            await m.ShowAsync();
        }

        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // display completion status.
            if (e.Error != null)
                Console.WriteLine(e.Error.Message);
            else
                Console.WriteLine("Download Completed!!!");
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


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserAgent"] != "default")
            {
                webView.NavigationStarting -= webView_NavigationStarting;
                args.Cancel = true;
                NavigateWithHeader(args.Uri);
            }
            if (disposing == false)
            {
                navigated = false;
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
                SearchWebBox.ItemsSource = null;
           //     Scrollline.ScrollToVerticalOffset(0);
              webView.Height = Window.Current.Bounds.Height - 89;
                webView.Width = Window.Current.Bounds.Width;
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                try
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                    {
                        UnloadObject(InfoFrameGrid);
                        //   IsShowingFailedMessage = false;
                        RefreshButtonIcon.Glyph = "\uE711";
                        CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Refresh };
                        CurrentTab.Header = "Starting...";
                        navigated = false;
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


        }
        private void NavigateWithHeader(Uri uri)
        {
            try
            {
                if ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserAgent"] != "default")
                {
                    HttpRequestMessage requestMsg = new Windows.Web.Http.HttpRequestMessage(HttpMethod.Get, uri);
                    requestMsg.Headers.Add("User-Agent", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserAgent"]);
                    webView.NavigateWithHttpRequestMessage(requestMsg);
                }

                webView.NavigationStarting += webView_NavigationStarting;
            }
            catch
            {

            }
        }

        private async void WebView_NavCompleted(object source, ElapsedEventArgs e)
        {
            //  try {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
  async () =>
  {
      if (navigated == false)
      {
          navigated = true;
          if (webView.CanGoForward == true)
          {
              IsForwardEnabled = true;
          }
          IsBackEnabled = true;

          if (disposing == false)
          {
              NavigationTip.Text = "Navigation finished";
              NavigationTipGrid.Visibility = Visibility.Collapsed;
              try
              {

                  string ConsoleFunctionString = @"window.onerror = function(error, url, line) {var r = 'ERR:' + error + ' url' + url + ' Line: ' + line; Window.ConsoleWinRT.setLogCombination(r); };";
                  await webView.InvokeScriptAsync("eval", new string[] { ConsoleFunctionString });
                  string functionString = @"document.oncontextmenu = function (e) {
                       window.Context.setHREFCombination();
                     };";
                  await webView.InvokeScriptAsync("eval", new string[] { functionString });
                  if ((bool)localSettings.Values["DarkMode"] == true)
                  {
                      DarkMode.DarkMode_Click(webView);
                  }
                  int height;
                  var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                  if (!int.TryParse(heightString, out height))
                  {
                      throw new Exception("Unable to get page height");
                  }
              webView.Height = Window.Current.Bounds.Height - 87;
                  webView.Width = Window.Current.Bounds.Width;
                  DOMloaded();
                  await GenerateActivityAsync();

              }
              catch
              {
                  NavigationTip.Text = "Navigation finished";

                  NavigationTipGrid.Visibility = Visibility.Collapsed;
              }
              SearchWebBox.ItemsSource = null;
          }
          if (disposing == false)
          {
              if (IncognitoMode == true)
              {
                  Uri gotouri = webView.Source;
                  HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
                  HttpCookieManager cookieManager = myFilter.CookieManager;
                  myFilter.ClearAuthenticationCache();
                  myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                  myFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;
                  myFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.Default;
                  HttpCookieCollection myCookieJar = cookieManager.GetCookies(gotouri);
                  foreach (HttpCookie cookie in myCookieJar)
                  {
                      cookieManager.DeleteCookie(cookie);
                  }
                  myFilter.Dispose();
              }
              else
              {
              }
          }

          navigated = true;

              NavTimer.Stop();
              NavTimer.Enabled = false;
          navigated = true;
      }
  });
            //     }
            //catch
            //  {

            //  }

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
            OfflinePage.WebWeb = webView;
            OfflinePage.ImageFrame = InkingFrame;
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
                MainFlyout.Hide();
                SecondFlyout.Hide();
            }
        }
        private void HisButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            OfflinePage.WebWeb = webView;
            OfflinePage.ImageFrame = InkingFrame;
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
                MainFlyout.Hide();
                SecondFlyout.Hide();
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            MainFlyout.Hide();
            SecondFlyout.Hide();
            HubFrame.Navigate(typeof(SettingsHubPage));
        }

        private void DownlButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            OfflinePage.WebWeb = webView;
            OfflinePage.ImageFrame = InkingFrame;
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
                MainFlyout.Hide();
                SecondFlyout.Hide();
            }
        }
        private void OfflineButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            OfflinePage.WebWeb = webView;
            OfflinePage.ImageFrame = InkingFrame;
            HubPage.NavString = "O";
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
                MainFlyout.Hide();
                SecondFlyout.Hide();
            }
        }
        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            HubsplitView.IsPaneOpen = true;
            Favourites.WebWeb = webView;
            OfflinePage.WebWeb = webView;
            OfflinePage.ImageFrame = InkingFrame;
            HubPage.NavString = "P";
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
                MainFlyout.Hide();
                SecondFlyout.Hide();
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
            // BackButton.IsEnabled = false;
            if (webView.CanGoForward == true)
            {
                IsForwardEnabled = true;
            }
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
                HomePage.WebViewControl = webView;
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
            if (navigated == true)
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
                    sender.IsSuggestionListOpen = false;
                    sender.ItemsSource = null;
                }
            }
            else
            {
                sender.IsSuggestionListOpen = false;
                sender.ItemsSource = null;
            }
        }

        private void SearchWebBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //  var s = (FrameworkElement)args.SelectedItem.ToString();
            //  var D = args.SelectedItem.DataContext;
            //  var dse = args.SelectedItem as SuggestionsClass;

            // webView.Navigate(new Uri(SearchEngine + sender.Text));
            sender.Text = args.SelectedItem.ToString();
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
            var uriNewWindow = new Uri(@"swiftlaunchincognito:");
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
        }


        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            var uriNewWindow = new Uri(@"swiftlaunch:");
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
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
            /*  CoreApplicationView newView = CoreApplication.CreateNewView();
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
              bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);*/
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
                    int duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("File: " + file.Name + "is saved.", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("File: " + file.Name + "is saved.", duration);
                    }
                }
                else
                {
                    int duration = 3000;
                    try { 
                    TabViewPage.InAppNotificationMain.Show("File: " + file.Name + "couldn't be saved.", duration);
                }
                    catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("File: " + file.Name + "couldn't be saved.", duration);
                }
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
            if (ShareIMG == false)
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
                try
                {
                    TabViewPage.titlebar.Width = TabViewPage.titlebar.Width = (Window.Current.Bounds.Width - 60) - (TabviewMain.TabItems.Count * newTab.Width);
                }
                catch
                {
                    TabViewPage.titlebar.Width = 240;
                }
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
                try
                {
                    TabViewPage.titlebar.Width = TabViewPage.titlebar.Width = (Window.Current.Bounds.Width - 60) - (TabviewMain.TabItems.Count * newTab.Width);
                }
                catch
                {
                    TabViewPage.titlebar.Width = 240;
                }
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
            //  var print = await GetWebPages(webView, new Windows.Foundation.Size(1200d, 1200d));
            //  RectangleToPrint.Children.Add(ee);
            //  ContentGrid.Children.Remove(webView);
            var pageNumber = 0;
            /*  foreach (var item in print)
            {
                  var grid = new Grid();
                  grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                  grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                  grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });



                  // Static header

                  var header = new TextBlock { Text = "Swift browser web printing (beta)", Margin = new Thickness(0, 0, 0, 20) };

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
              }*/
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });



            // Static header

            var header = new TextBlock { Text = "Swift browser web printing (beta)", Margin = new Thickness(0, 0, 0, 20) };

            Grid.SetRow(header, 0);

            grid.Children.Add(header);



            // Main content with layout from data template
            InMemoryRandomAccessStream Prints = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Prints);
            Image bit = new Image();
            BitmapImage ee = new BitmapImage();
            ee.SetSource(Prints);
            bit.Source = ee;
            grid.Children.Add(bit);



            // Footer with page number

            pageNumber++;

            var footer = new TextBlock { Text = string.Format("page {0}", pageNumber), Margin = new Thickness(0, 20, 0, 0) };

            Grid.SetRow(footer, 2);

            grid.Children.Add(footer);
            printHelper.AddFrameworkElementToPrint(grid);
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
            //  BackButton.IsEnabled = false;
            //  ForwardButton.IsEnabled = false;
            //  RefreshButton.IsEnabled = false;
            MainFlyout.Hide();
            ExtensionsButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            InkingFrameGrid.Visibility = Visibility.Visible;
            GC.Collect();
            HomePage.WebViewControl = webView;
            inkingPage.WebView = webView;
            InkingFrame.Navigate(typeof(inkingPage));
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Edit };
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
        public void Churros()
        {
            try
            {
                BackButton.IsEnabled = false;
            }
            catch
            { }
            //    ForwardButton.IsEnabled = false;
            //   RefreshButton.IsEnabled = false;
            MainFlyout.Hide();
            SecondFlyout.Hide();
            ExtensionsButton.IsEnabled = false;
            InkingFrameGrid.Visibility = Visibility.Visible;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            try
            {
                UnloadObject(webView);
            }
            catch
            {
                UnloadObject(InfoFrameGrid);
            }
        }
        private async void FeedbackLink_Click(object sender, RoutedEventArgs e)
        {
            // This launcher is part of the Store Services SDK https://docs.microsoft.com/windows/uwp/monetize/microsoft-store-services-sdk
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        public async void WhatNew(object sender, RoutedEventArgs e)
        {
            var dialog = new WhatsNewDialog();
            await dialog.ShowAsync();
        }
        public void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        public async void Rate(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }
        public async void About(object sender, RoutedEventArgs e)
        {
            String about = "About: " + "AppDisplayName".GetLocalized() + "\n" + Package.Current.Id + "\n" + Package.Current.Id.Version + "\n" + Package.Current.Id.Publisher;
            try { 
            TabViewPage.InAppNotificationMain.Show(about);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show(about);
            }
        }
        public async void Help(object sender, RoutedEventArgs e)
        {
            var dialog = new FirstRunDialog();
            await dialog.ShowAsync();
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
        }

        private void ZoomFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ZoomTip.IsOpen = true;
        }

        private void ClearHighlights(object sender, RoutedEventArgs e)
        {
            ClearSearch();
        }
        private void FindFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FindTip.IsOpen = true;
            MainFlyout.Hide();
        }
        private bool Cleared = true;
        private async void HighlightTerm_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!Cleared) ClearSearch();
            await webView.InvokeScriptAsync("eval", new string[] { HighlightFunctionJS + " HighlightFunction('" + args.QueryText + "');" });
            Cleared = false;
        }
        public async void ClearSearch()
        {
            await webView.InvokeScriptAsync("eval", new string[] { HighlightFunctionJS + " RestoreFunction();" });
            Cleared = true;
        }

        private void FindTip_CloseButtonClick(TeachingTip sender, object args)
        {
            ClearSearch();
        }

        private void TranslateFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            webView.Navigate(new Uri("https://translate.google.com/translate?hl=en&sl=auto&tl=en&u=" + webView.Source));
        }
        public async void CreateRTBOffline(string Name)
        {
            //   BackButton.IsEnabled = false;
            //   ForwardButton.IsEnabled = false;
            //   RefreshButton.IsEnabled = false;
            MainFlyout.Hide();
            SecondFlyout.Hide();
            ExtensionsButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            InkingFrameGrid.Visibility = Visibility.Visible;
            GC.Collect();
            InMemoryRandomAccessStream Snips = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Snips);
            OfflineCreatePage.webView = webView;
            OfflineCreatePage.NameX = Name;
            InkingFrame.Navigate(typeof(OfflineCreatePage));
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.ZeroBars };
            CurrentTab.Header = "Offline Tab";
            try
            {
                UnloadObject(webView);
            }
            catch
            {
                UnloadObject(InfoFrameGrid);
            }
        }
        private async void PageIMGFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
             try
              {
                RenderTargetBitmap rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(ContentGrid);

                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var picker = new FileSavePicker();
                picker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });
                StorageFile file = await picker.PickSaveFileAsync();
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Premultiplied,
                                         (uint)rtb.PixelWidth,
                                         (uint)rtb.PixelHeight,
                                          displayInformation.RawDpiX,
                             displayInformation.RawDpiY,
                                         pixels);
                    await encoder.FlushAsync();
                }
                int duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Screen captured and saved", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Screen captured and saved", duration);
                }
           }
           catch
           {

           }
        }
        private async void MuteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string mutefunctionString = @" var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    [].forEach.call(videos, function(video) { video.muted = true; });
    [].forEach.call(audios, function(audio) { audio.muted = true; }); ";

            await webView.InvokeScriptAsync("eval", new string[] { mutefunctionString });
        }
        private async void unMuteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string unmutefunctionString = @"var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    [].forEach.call(videos, function(video) { video.muted = false; });
    [].forEach.call(audios, function(audio) { audio.muted = false; }); ";

            await webView.InvokeScriptAsync("eval", new string[] { unmutefunctionString });
        }

        private async void TempLaunchAppFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string source = webView.Source.ToString();
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                WebApp.WebViewNavigationString = source;
                frame.Navigate(typeof(WebApp));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private async void SnipFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ////  ForwardButton.IsEnabled = false;
            //  RefreshButton.IsEnabled = false;
            MainFlyout.Hide();
            SecondFlyout.Hide();
            ExtensionsButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            MenuFrameButton.Visibility = Visibility.Visible;
            MenuButton.Visibility = Visibility.Collapsed;
            InkingFrameGrid.Visibility = Visibility.Visible;
            GC.Collect();
            InMemoryRandomAccessStream Snips = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Snips);
            SnipPage.WebView = Snips;
            HomePage.WebViewControl = webView;
            InkingFrame.Navigate(typeof(SnipPage));
            CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Cut };
            CurrentTab.Header = "Snip Tab";
            try
            {
                UnloadObject(webView);
            }
            catch
            {
                UnloadObject(InfoFrameGrid);
            }
        }

        private async void TaskMngFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessMemoryUsageReport mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
            MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage.ToString() + "\r\nMemory Level: " + MemoryManager.AppMemoryUsageLevel.ToString();
            XMemUsage.Text = "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit.ToString() + "\r\nExpected Memory Usage Limit: " + MemoryManager.ExpectedAppMemoryUsageLimit.ToString();
            ProcessDiskUsageReport du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
            DiskUsage.Text = "Bytes Written Count: " + du.BytesWrittenCount.ToString() + "\r\nBytes Read Count:" + du.BytesReadCount.ToString() + "\r\nOther Bytes Count:" + du.OtherBytesCount.ToString();
            XDiskUsage.Text = "Write Operation Count: " + du.WriteOperationCount.ToString() + "\r\nRead Operation Count: " + du.ReadOperationCount.ToString() + "\r\nOther Operation Count:" + du.OtherOperationCount.ToString();
            ProcessCpuUsageReport cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
            CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime.ToString() + "\r\nCpu User Time:" + cu.UserTime.ToString();
            string id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
            IdProcess.Text = "Process Id: " + id;
            string st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.ToString();
            StartTime.Text = "Start Time: " + st;
            TaskMNGTimer = new System.Timers.Timer();
            TaskMNGTimer.Elapsed += TaskMNGTimer_Elapsed;
            TaskMNGTimer.Interval = 100;
            TaskMNGTimer.Enabled = true;
            await TaskMNG.ShowAsync();
        }

        private async void TaskMNGTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    ProcessMemoryUsageReport mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
    MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage.ToString() + "\r\nMemory Level: " + MemoryManager.AppMemoryUsageLevel.ToString();
    XMemUsage.Text = "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit.ToString() + "\r\nExpected Memory Usage Limit: " + MemoryManager.ExpectedAppMemoryUsageLimit.ToString();
    ProcessDiskUsageReport du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
    DiskUsage.Text = "Bytes Written Count: " + du.BytesWrittenCount.ToString() + "\r\nBytes Read Count:" + du.BytesReadCount.ToString() + "\r\nOther Bytes Count:" + du.OtherBytesCount.ToString();
    XDiskUsage.Text = "Write Operation Count: " + du.WriteOperationCount.ToString() + "\r\nRead Operation Count: " + du.ReadOperationCount.ToString() + "\r\nOther Operation Count:" + du.OtherOperationCount.ToString();
    ProcessCpuUsageReport cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
    CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime.ToString() + "\r\nCpu User Time:" + cu.UserTime.ToString();
    string id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
    IdProcess.Text = "Process Id: " + id;
    string st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.ToString();
    StartTime.Text = "Start Time: " + st;
});
        }

        private void RefreshTaskManagerButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessMemoryUsageReport mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
            MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage.ToString() + "\r\nMemory Level: " + MemoryManager.AppMemoryUsageLevel.ToString();
            XMemUsage.Text = "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit.ToString() + "\r\nExpected Memory Usage Limit: " + MemoryManager.ExpectedAppMemoryUsageLimit.ToString();
            ProcessDiskUsageReport du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
            DiskUsage.Text = "Bytes Written Count: " + du.BytesWrittenCount.ToString() + "\r\nBytes Read Count:" + du.BytesReadCount.ToString() + "\r\nOther Bytes Count:" + du.OtherBytesCount.ToString();
            XDiskUsage.Text = "Write Operation Count: " + du.WriteOperationCount.ToString() + "\r\nRead Operation Count: " + du.ReadOperationCount.ToString() + "\r\nOther Operation Count:" + du.OtherOperationCount.ToString();
            ProcessCpuUsageReport cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
            CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime.ToString() + "\r\nCpu User Time:" + cu.UserTime.ToString();
            string id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
            IdProcess.Text = "Process Id: " + id;
            string st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.ToString();
            StartTime.Text = "Start Time: " + st;
        }

        private async void CompactFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                Thickness Thicc = new Thickness();
                Thicc.Top = -100;
                TabviewMain.Margin = Thicc;
                CompactButton.Visibility = Visibility.Visible;
                Iscompact = true;
            }
        }

        private async void CompactButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            Thickness Thicc = new Thickness();
            Thicc.Top = -10;
            TabviewMain.Margin = Thicc;
            CompactButton.Visibility = Visibility.Collapsed;
            Iscompact = false;
        }

        private async void ReadAloudButton_Click(object sender, RoutedEventArgs e)
        {
            // string html;
            // obtain some arbitrary html....
            // using (var client = new WebClient())
            //  {
            //     html = client.DownloadString(webView.Source);
            //  }
            string html = await webView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            StringBuilder sb = new StringBuilder();
            doc.DocumentNode.SelectNodes("//style|//script").ToList().ForEach(n => n.Remove());
            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                sb.AppendLine(node.Text);
            }
            string final = sb.ToString();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(final);

            // Send the stream to the media object.
            ReadAloudElement.SetSource(stream, stream.ContentType);
            ReadAloudElement.Play();
            ReadTip.IsOpen = true;
            MainFlyout.Hide();
            SecondFlyout.Hide();
        }



        private void ReadTip_CloseButtonClick(TeachingTip sender, object args)
        {
            ReadAloudElement.Stop();
        }

        private void ReadingSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            if (toggle.IsOn == true)
            {
                ReadAloudElement.Play();
            }
            else
            {
                ReadAloudElement.Pause();
            }
        }

        private void ReadingModeButton_Click(object sender, RoutedEventArgs e)
        {
            string url = webView.Source.ToString();

            var t = new NReadability.NReadabilityWebTranscoder();
            bool b;
#pragma warning disable CS0618 // Type or member is obsolete
            string page = t.Transcode(url, out b);
#pragma warning restore CS0618 // Type or member is obsolete

            if (b)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);
                MainFlyout.Hide();
                SecondFlyout.Hide();
                string title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                var imgUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']").Attributes["content"].Value;
                string mainText = doc.DocumentNode.SelectSingleNode("//div[@id='readInner']").InnerText;
                //   BackButton.IsEnabled = false;
                //   ForwardButton.IsEnabled = false;
                //    RefreshButton.IsEnabled = false;
                ExtensionsButton.IsEnabled = false;
                MenuButton.IsEnabled = false;
                MenuFrameButton.Visibility = Visibility.Visible;
                MenuButton.Visibility = Visibility.Collapsed;
                InkingFrameGrid.Visibility = Visibility.Visible;
                GC.Collect();
                HomePage.WebViewControl = webView;
                ReadingModeFrame.TitleString = title;
                ReadingModeFrame.BodyString = mainText;
                InkingFrame.Navigate(typeof(ReadingModeFrame));
                CurrentTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                CurrentTab.Header = "Reading Tab";
                try
                {
                    UnloadObject(webView);
                }
                catch
                {
                    UnloadObject(InfoFrameGrid);
                }
            }
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(downloadSource);
        }

        private async void DevButton_Click(object sender, RoutedEventArgs e)
        {
            AppWindow appWindow = await AppWindow.TryCreateAsync();
            Frame appWindowContentFrame = new Frame();
            Devtools.DevView = webView;
            appWindowContentFrame.Navigate(typeof(Devtools));
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
            await appWindow.TryShowAsync();
            appWindow.Closed += delegate
            {
                appWindowContentFrame.Content = null;
                appWindow = null;
            };
        }


        public class ExtensionsClass
        {
            public List<ExtensionsJSON> Extensions { get; set; }
        }

        /*  public class Extensions
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class ExtensionsJSON
        {
            public string NameJSON { get; set; }
            public string DescriptionJSON { get; set; }
            public string IconJSON { get; set; }
            public int Id { get; set; }
            public bool IsEnabledJSON { get; set; }
            public bool IsIncognitoEnabled { get; set; }
            public bool IsToolbar { get; set; }
            public string Page { get; set; }
        }
        private async void ExtensionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("Extensions.json"); // error here
                var JSONData = "e";
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                ExtensionsClass ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
                foreach (var item in ExtensionsListJSONJSON.Extensions)
                {
                    if (item.IsEnabledJSON == true)
                    {
                        ExtensionsListJSON.Add(new ExtensionsJSON()
                        {
                            NameJSON = item.NameJSON,
                            DescriptionJSON = item.DescriptionJSON,
                            IconJSON = item.IconJSON,
                            Page = item.Page
                        });
                    }
                }
                ExtensionsList.ItemsSource = ExtensionsListJSON;
            }
            catch
            {

            }
        }


        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            Frame f = sender as Frame;
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            ExtensionsJSON Json = DataContext as ExtensionsJSON;
            switch (Json.Page)
            {
                case "AdblockerBuiltInSwiftBrowserExtension":
                    AdblockerBuiltInSwiftBrowserExtension.w = webView;
                    f.Navigate(typeof(AdblockerBuiltInSwiftBrowserExtension));
                    break;
                case "DarkmodeSwiftBrowserBuiltInExtension":
                    DarkmodeSwiftBrowserBuiltInExtension.w = webView;
                    f.Navigate(typeof(DarkmodeSwiftBrowserBuiltInExtension));
                    break;
                case "nonexistentemojipage":
                    CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
                    break;
                 
                   
            }
        }
        WebView webviewE;
        private void Image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame f = sender as Frame;
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            ExtensionsJSON Json = DataContext as ExtensionsJSON;

           if(Json.Page == "nonexistentemojipage")
            {
                CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
            }
            else if(Json.Page.Contains("http"))
            {
          

                ExtensionsplitView.IsPaneOpen = true;
                
                 webviewE = new WebView(executionMode: WebViewExecutionMode.SeparateProcess);
                webviewE.Width = 500;
           
                ExtensionsplitView.Content = webviewE;
                webviewE.Navigate(new Uri(Json.Page));
            }
           else
            {
                Image i = sender as Image;
                i.ContextFlyout.ShowAt(i);
            }
        }

        private async void ExtensionsManger_Click(object sender, RoutedEventArgs e)
        {
            ExtensionsFlyout.Hide();
            MainFlyout.Hide();
            SecondFlyout.Hide();
           HubsplitView.IsPaneOpen = true;
         HubFrame.Navigate(typeof(ExtensionsStore));
        }

        private void SecureTextTemporary_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock SecureText = sender as TextBlock;
            try
            {
                if (webView.Source.AbsoluteUri.StartsWith("https"))
                {
                    SecureText.Text = "Your connection to this site is secure";
                }
                else
                {
                    SecureText.Text = "Your connection to this site is not secure";
                }
            }
            catch
            {
                SecureText.Text = "No site is laoded";
            }
        }
        public async Task<X509Certificate2> GetServerCertificateAsync()
        {
            X509Certificate2 certificate = null;
            var httpClientHandler = new System.Net.Http.HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, cert, __, ___) =>
                {
                    certificate = new X509Certificate2(cert.GetRawCertData());
                    return true;
                }
            };

            var httpClient = new System.Net.Http.HttpClient(httpClientHandler);
            await httpClient.SendAsync(new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Head, webView.Source));

            return certificate ?? throw new NullReferenceException();
        }
        private async void SecureCertificateTemporary_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock s = sender as TextBlock;
            try
            {

                X509Certificate xcertificate = await GetServerCertificateAsync();
                X509Certificate2 cert2 = new X509Certificate2(xcertificate);
                s.Text = cert2.ToString();
            }
            catch
            {
                s.Text = "No site is laoded";
            }
        }
        public class FavouritesClass
        {
            public List<FavouritesJSON> Websites { get; set; }
        }
        public class FavouritesJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
        private async void FavToolbar_Click(object sender, RoutedEventArgs e)
        {
            StorageFile sampleFile;
            try
            {
                sampleFile = await localFolder.GetFileAsync("Favorites.json");
            }
            catch
            {
                string filepath = @"Assets\Favorites.json";
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await folder.GetFileAsync(filepath); // error here
                string JSONsData = await Windows.Storage.FileIO.ReadTextAsync(file);
                sampleFile = await localFolder.CreateFileAsync("Favorites.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONsData);
            }
            localSettings.Values["FirstFavRun"] = false;
            var JSONData = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            try
            {
                Uri ArgsUri = new Uri(webView.Source.ToString());
                string host = ArgsUri.Host;
                string x = "";
                try
                {
                    x = await webView.InvokeScriptAsync("eval", new string[] { "document.title;" });
                }
                catch
                {
                    x = webView.Source.ToString();
                }
                FavouritesListJSON.Websites.Add(new FavouritesJSON()
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = webView.Source.ToString(),
                    HeaderJSON = x
                }); ;
                var SerializedObject = JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, SerializedObject);
                int duration = 3000;
                try { 
                TabViewPage.InAppNotificationMain.Show("Saved", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Saved", duration);
                }
            }
            catch
            {
                int duration = 3000;
                try { 
                TabViewPage.InAppNotificationMain.Show("Something went wrong when saving", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Something went wrong when saving", duration);
                }
            }
        }

        private void OfflineToolbar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HomeFrame.Visibility != Visibility.Visible)
                {
                    var random = new Random();
                    int randomnumber = random.Next();
                    string rand = randomnumber.ToString();
                    CreateRTBOffline(rand);
                }
            }
            catch
            {
                var random = new Random();
                int randomnumber = random.Next();
                string rand = randomnumber.ToString();
                CreateRTBOffline(rand);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("ContentControl");
        }

        private void ContentGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                int toolCount = ExtensionsListToolbar.Items.Count;
                int math = toolCount * 35;
                int total = math + 340;
                SearchWebBox.Width = Window.Current.Bounds.Width - total;
                try
                {
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                    {
                        SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
                    }
                }
                catch
                {

                }
            }
            catch
            {
                SearchWebBox.Width = 900;
                try { 
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
                {
                    SearchWebMobileBox.Width = 900;
                }
            }
            catch
            {

            }
        }
            Startup();
        }

        private void TaskMNG_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            TaskMNGTimer.Enabled = false;
            TaskMNGTimer = null;
        }


 
        private void SearchWebBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //  AutoSuggestBox s = (AutoSuggestBox)sender;
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.Mobile")
             {
            SearchWebMobileBox.Width = SearchWebMobileBox.Width + 120;
       //     SearchWebMobileBox.MaxWidth = SearchWebMobileBox.Width + 120;
            eo.Visibility = Visibility.Collapsed;
            BackMobileButton.Visibility = Visibility.Collapsed;
          }
            //    textBox.SelectAll();
        }

        private void FlyoutMobileButton_Click(object sender, RoutedEventArgs e)
        {
            MainFlyout.ShowAt(MobileBar);
        }

        private void SearchWebMobileBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchWebMobileBox.Width = SearchWebMobileBox.Width - 120;
       //     SearchWebMobileBox.MaxWidth = SearchWebMobileBox.Width;
            eo.Visibility = Visibility.Visible;
            BackMobileButton.Visibility = Visibility.Visible;
        }

        private async void TabsButton_Checked(object sender, RoutedEventArgs e)
        {
            Thickness Thicc = new Thickness();
            Thicc.Top = -37;
            TabviewMain.Margin = Thicc;
            //  TabviewMain.VerticalContentAlignment = VerticalAlignment.Stretch;
            // TabviewMain.VerticalAlignment = VerticalAlignment.Stretch;
            //  ContentGrid.VerticalAlignment = VerticalAlignment.Stretch;
            //  ContentControl.VerticalAlignment = VerticalAlignment.Stretch;
            //    Canvas.SetZIndex(webView, 1000);
            await Task.Delay(1350);
            try
            {
                int height;
                var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                if (!int.TryParse(heightString, out height))
                {
                    throw new Exception("Unable to get page height");
                }
                webView.Height = Window.Current.Bounds.Height;
                webView.Width = Window.Current.Bounds.Width;
            }
            catch
            {
                try
                {
                    webView.Height = Window.Current.Bounds.Height;
                    webView.Width = Window.Current.Bounds.Width;
                }
                catch
                {

                }
            }
        }

        private async void TabsButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Thickness Thicc = new Thickness();
            Thicc.Top = -8.5;
            TabviewMain.Margin = Thicc;
            Thickness thicc = new Thickness();
            thicc.Top = 0;
            webView.Margin = thicc;
            await Task.Delay(1350);
            //  TabviewMain.Height = webView.Height - 300;
            //   webView.Height = webView.Height - 300;
            try
            {
                int height;
                var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                if (!int.TryParse(heightString, out height))
                {
                    throw new Exception("Unable to get page height");
                }
                webView.Height = Window.Current.Bounds.Height - 87;
                webView.Width = Window.Current.Bounds.Width;
            }
            catch
            {
                try
                {
                    webView.Height = Window.Current.Bounds.Height - 89;
                    webView.Width = Window.Current.Bounds.Width;
                }
                catch
                {

                }
            }
        }

        private void ExtensionsplitView_PaneClosed(SplitView sender, object args)
        {
            ExtensionsplitView.Content = null;
        }

        private void HubsplitView_PaneClosed(SplitView sender, object args)
        {
            ExtensionsStore.methods.closing();
        }
    }
    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
}
