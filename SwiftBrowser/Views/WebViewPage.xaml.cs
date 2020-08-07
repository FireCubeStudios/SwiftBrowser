using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.ViewManagement.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Web.Http.Filters;
using HtmlAgilityPack;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using NReadability;
using SwiftBrowser.Assets;
using SwiftBrowser.Extensions;
using SwiftBrowser.Extensions.Views;
using SwiftBrowser.Helpers;
using SwiftBrowser.HubViews;
using SwiftBrowser.SettingsViews;
using SwiftBrowser_Runtime_Component;
using WinRTXamlToolkit.Controls.Extensions;
using HttpClient = System.Net.Http.HttpClient;
using HttpMethod = Windows.Web.Http.HttpMethod;
using HttpRequestMessage = Windows.Web.Http.HttpRequestMessage;
using HttpResponseMessage = Windows.Web.Http.HttpResponseMessage;
using HttpStatusCode = Windows.Web.Http.HttpStatusCode;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace SwiftBrowser.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        public static bool IsMobileSiteEnabled = false;
        private UserActivitySession _currentActivity;
        public MenuFlyoutItem AppItem;
        public string BackupStringToNavigate;
        private bool Cleared = true;
        private readonly MenuFlyout ContextFlyout = new MenuFlyout();
        private readonly MenuFlyout ContextFlyoutImage = new MenuFlyout();
        public MenuFlyoutItem CopyIMGItem;
        public MenuFlyoutItem CopyItem;
        private readonly WinUI.TabViewItem CurrentTab;
        private readonly DarkMode DarkMode = new DarkMode();
        public MenuFlyoutItem DevItem;
        public bool disposing;
        public bool dom = false;
        private Uri downloadSource;
        public WebViewLongRunningScriptDetectedEventArgs e;


        public bool ee = false;
        private StorageFile fileExtensionAdblocker;
        private string fileExtensionAdblockerText;
        public MenuFlyoutItem firstItem;
        public string HighlightFunctionJS;
        public MenuFlyoutItem IncognitoItem;
        public bool IncognitoMode;

        public bool IsBackEnabled;
        private bool Iscompact;
        private bool isf;
        public bool isfirst = true;


        public bool IsForwardEnabled;

        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        // TODO WTS: Set the URI of the page to show by default
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Timer MemoryTimer;
        public bool navigated = true;
        private Timer NavTimer;
        private string NewWindowLink;
        private readonly MediaElement ReadAloudElement = new MediaElement();
        private SpeechRecognizer recognizer;
        public Timer RightTimer;
        public MenuFlyoutItem SaveIMGItem;
        public string SearchEngine = (string) ApplicationData.Current.LocalSettings.Values["SearchEngine"];
        private bool ShareIMG;
        public MenuFlyoutItem ShareIMGItem;
        public string StringToNavigate;
        private List<string> SuggestionsList;
        private readonly WinUI.TabView TabviewMain;
        private Timer TaskMNGTimer;
        private readonly UserAgents UserAgents = new UserAgents();
        public WebView webView;
        private DataPackage WebviewDataPackage;
        private WebView webviewE;
        private readonly WebViewEvents WebViewEvents = new WebViewEvents();
        public MenuFlyoutItem WindowItem;

        public WebViewPage()
        {
            //  UserAgentbuttonControl = UserAgentbutton;
            InfoDialog = InfoTip;
            SingletonReference = this;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            StringToNavigate = (string) localSettings.Values["SourceToGo"];
            BackupStringToNavigate = (string) localSettings.Values["BackupSourceToGo"];
            localSettings.Values["SourceToGo"] = null;
            localSettings.Values["BackupSourceToGo"] = null;
            if (IncognitoMode)
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

        public static WebView WebviewControl { get; set; }
        public static WinUI.TabView MainTab { get; set; }
        public static WinUI.TabViewItem CurrentMainTab { get; set; }
        public static Button UserAgentbuttonControl { get; set; }
        public static WinUI.TeachingTip InfoDialog { get; set; }
        public static Grid HomeFrameFrame { get; set; }
        public static bool IncognitoModeStatic { get; set; }
        public static string SourceToGo { get; set; }

        public static WebViewPage SingletonReference { get; set; }

        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            try
            {
                if (webView.CanGoBack && HomeFrame.IsLoaded == false)
                {
                    webView.GoBack();
                }
                else if (webView.CanGoBack)
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                else if (HomeFrame.IsLoaded | (InkingFrameGrid.Visibility == Visibility.Visible))
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                    if (webView.CanGoForward) IsForwardEnabled = true;
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

                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
                    CurrentTab.Header = "Home Tab";
                }
            }
            catch
            {
                if (webView.CanGoBack && InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                else if (webView.CanGoBack)
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
                    if (webView.CanGoForward) IsForwardEnabled = true;
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

                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
                    CurrentTab.Header = "Home Tab";
                }
            }
        }

        public async void OnListenAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                recognizer = new SpeechRecognizer();
                await recognizer.CompileConstraintsAsync();

                recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
                recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

                recognizer.UIOptions.AudiblePrompt = "Search with voice";
                recognizer.UIOptions.ExampleText = "what is the time.";
                recognizer.UIOptions.ShowConfirmation = true;
                recognizer.UIOptions.IsReadBackEnabled = true;
                recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

                var result = await recognizer.RecognizeWithUIAsync();

                if (result != null)
                {
                    var winRTObject = new ContextMenu();
                    webView.AddWebAllowedObject("Context", winRTObject);
                    var winRTConsole = new ConsoleLog();
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
                    var settings = new MediaCaptureInitializationSettings();
                    settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                    settings.MediaCategory = MediaCategory.Speech;
                    var capture = new MediaCapture();

                    await capture.InitializeAsync(settings);
                }
                catch (TypeLoadException)
                {
                    // Thrown when a media player is not available.
                    var duration = 3000;
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
                    var duration = 3000;
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
                    var duration = 3000;
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

        public async void Startup()
        {
            Window.Current.CoreWindow.KeyDown += (s, eArgs) =>
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && eArgs.VirtualKey == VirtualKey.F)
                {
                    // do your stuff
                    FindTip.IsOpen = true;
                    MainFlyout.Hide();
                }
            };
            Window.Current.CoreWindow.KeyDown += (s, eArgs) =>
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && eArgs.VirtualKey == VirtualKey.R)
                {
                    // do your stuff
                        if (RefreshButton.IsEnabled == true)
                        { webView.Refresh(); }
                }
            };
            Window.Current.CoreWindow.KeyDown += (s, eArgs) =>
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && eArgs.VirtualKey == VirtualKey.H)
                {
                    // do your stuff
                    Favourites.WebWeb = webView;
                    OfflinePage.WebWeb = webView;
                    OfflinePage.ImageFrame = InkingFrame;
                    HubPage.NavString = "H";
                    HubFrame.Navigate(typeof(HubPage));
                    try
                    {
                        if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
                    }
                    catch
                    {
                        Favourites.BoolWeb = true;
                    }
                }
            };
            if (isfirst)
            {
                isfirst = false;
                SearchWebBox.Text = "";
                Window.Current.SizeChanged -= ViewElapsed;
                firstItem = new MenuFlyoutItem {Text = "Open in new tab- beta"};
                CurrentTab.Tag = webView;
                firstItem.Click += FirstItem_Click;
                ContextFlyout.Items.Add(firstItem);
                WindowItem = new MenuFlyoutItem {Text = "Open in new window - beta"};
                WindowItem.Click += WindowItem_Click;
                ContextFlyout.Items.Add(WindowItem);
                IncognitoItem = new MenuFlyoutItem {Text = "Open in new incognito - beta"};
                ContextFlyout.Items.Add(IncognitoItem);
                IncognitoItem.Click += IncognitoItem_Click;
                AppItem = new MenuFlyoutItem {Text = "Open as app - beta"};
                AppItem.Click += AppItem_Click;
                ContextFlyout.Items.Add(AppItem);
                CopyItem = new MenuFlyoutItem {Text = "Copy link - beta"};
                CopyItem.Click += CopyItem_Click;
                ContextFlyout.Items.Add(CopyItem);
                CopyIMGItem = new MenuFlyoutItem {Text = "Copy image - beta"};
                CopyIMGItem.Click += CopyIMGItem_Click;
                SaveIMGItem = new MenuFlyoutItem {Text = "Save image as .png - beta"};
                SaveIMGItem.Click += SaveIMGItem_Click;
                ShareIMGItem = new MenuFlyoutItem {Text = "Share image - beta"};
                ShareIMGItem.Click += ShareIMGItem_Click;
                DevItem = new MenuFlyoutItem {Text = "DevTools"};
                DevItem.Click += DevItem_Click;
                ContextFlyout.Items.Add(DevItem);
                ContextFlyoutImage.Items.Add(CopyIMGItem);
                ContextFlyoutImage.Items.Add(SaveIMGItem);
                ContextFlyoutImage.Items.Add(ShareIMGItem);
                ContextFlyoutImage.Items.Add(DevItem);
                //  Window.Current.SizeChanged += SearchWebBox_SizeChanged;
                var dataTransferManager = DataTransferManager.GetForCurrentView();
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
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "´Windows.Mobi")
                {
                    Bar.Visibility = Visibility.Collapsed;
                    FindName("MobileBar");
                }

                webView.DOMContentLoaded += WebView_DOMContentLoaded;
                webView.IsRightTapEnabled = true;
                webView.PointerWheelChanged += WebView_PointerWheelChanged;
                webView.WebResourceRequested += WebView_WebResourceRequested;
                webView.Settings.IsJavaScriptEnabled =
                    (bool) ApplicationData.Current.LocalSettings.Values["Javascript"];
                webView.Settings.IsIndexedDBEnabled = (bool) ApplicationData.Current.LocalSettings.Values["IndexDB"];
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
                if (IncognitoModeStatic)
                {
                    IncognitoMode = true;
                    localSettings.Values["IncognitoMode"] = false;
                    if (StringToNavigate != null)
                    {
                        FindName("webView");
                        IsloadingPanel.Visibility = Visibility.Visible;
                        MenuFrameButton.Visibility = Visibility.Collapsed;
                        MenuButton.Visibility = Visibility.Visible;
                        Loading.IsActive = true;
                        try
                        {
                            webView.Navigate(new Uri(StringToNavigate));
                        }
                        catch
                        {
                            try
                            {
                                webView.Navigate(new Uri(BackupStringToNavigate));
                            }
                            catch
                            {
                            }
                        }

                        var winRTObject = new ContextMenu();
                        webView.AddWebAllowedObject("Context", winRTObject);
                        var winRTConsole = new ConsoleLog();
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
                    if (StringToNavigate != null)
                    {
                        FindName("webView");
                        IsloadingPanel.Visibility = Visibility.Visible;
                        MenuFrameButton.Visibility = Visibility.Collapsed;
                        MenuButton.Visibility = Visibility.Visible;
                        Loading.IsActive = true;
                        try
                        {
                            webView.Navigate(new Uri(StringToNavigate));
                        }
                        catch
                        {
                            try
                            {
                                webView.Navigate(new Uri(BackupStringToNavigate));
                            }
                            catch
                            {
                            }
                        }

                        var winRTObject = new ContextMenu();
                        webView.AddWebAllowedObject("Context", winRTObject);
                        var winRTConsole = new ConsoleLog();
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

                var HighlightFile =
                    await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///WebFiles/highlight.js"));
                HighlightFunctionJS = await FileIO.ReadTextAsync(HighlightFile);
                WebviewControl = webView;
                HomeFrameFrame = HomeFrame;
                RightTimer = new Timer();
                RightTimer.Elapsed += WebView_RightTapped;
                RightTimer.Interval = 500;
                RightTimer.Enabled = true;
                MemoryTimer = new Timer();
                MemoryTimer.Elapsed += WebView_Memory;
                try
                {
                    var ExtensionsListJSON = new List<ExtensionsJSON>();
                    var folder = ApplicationData.Current.LocalFolder;
                    var file = await folder.GetFileAsync("Extensions.json"); // error here
                    var JSONData = "e";
                    JSONData = await FileIO.ReadTextAsync(file);
                    var ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
                    foreach (var item in ExtensionsListJSONJSON.Extensions)
                        // var m = new MessageDialog(item.DescriptionJSON + item.Page + item.Id.ToString());
                        //      await m.ShowAsync();
                        if (item.IsEnabledJSON && item.IsToolbar)
                            ExtensionsListJSON.Add(new ExtensionsJSON
                            {
                                NameJSON = item.NameJSON,
                                DescriptionJSON = item.DescriptionJSON,
                                IconJSON = item.IconJSON,
                                Page = item.Page
                            });
                    ExtensionsListToolbar.ItemsSource = ExtensionsListJSON;
                }
                catch
                {
                }

                try
                {
                    var toolCount = ExtensionsListToolbar.Items.Count;
                    var math = toolCount * 35;
                    var total = math + 340;
                    SearchWebBox.Width = Window.Current.Bounds.Width - total;
                    try
                    {
                        if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                            SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    SearchWebBox.Width = 900;
                    try
                    {
                        if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile") SearchWebMobileBox.Width = 900;
                    }
                    catch
                    {
                    }
                }

                var filepathx = @"Assets\AdblockerText.txt";
                var folderx = Package.Current.InstalledLocation;
                fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                fileExtensionAdblockerText = await FileIO.ReadTextAsync(fileExtensionAdblocker);
            }
        }

        private void WebView_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            switch ((int) args.PermissionRequest.PermissionType)
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
            var p = e.GetCurrentPoint(webView);
            var pp = p.Properties;
            var mp = pp.MouseWheelDelta;
            if (mp > 0)
            {
                //     Scrollline.ScrollToVerticalOffset(Scrollline.VerticalOffset - 10);
            }
        }

        private async void WebView_WebResourceRequested(WebView sender, WebViewWebResourceRequestedEventArgs args)
        {
            try
            {
                if ((bool) localSettings.Values["AdBlocker"])
                    try
                    {
                        if (fileExtensionAdblockerText.Contains(args.Request.RequestUri.Host))
                            args.Response = new HttpResponseMessage(HttpStatusCode.Ok);
                    }
                    catch
                    {
                        var filepathx = @"Assets\AdblockerText.txt";
                        var folderx = Package.Current.InstalledLocation;
                        fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                        fileExtensionAdblockerText = await FileIO.ReadTextAsync(fileExtensionAdblocker);
                    }
            }
            catch
            {
            }
        }

        private async void WebView_FrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                if ((bool) localSettings.Values["AdBlocker"])
                    try
                    {
                        if (fileExtensionAdblockerText.Contains(args.Uri.Host)) args.Cancel = true;
                    }
                    catch
                    {
                        var filepathx = @"Assets\AdblockerText.txt";
                        var folderx = Package.Current.InstalledLocation;
                        fileExtensionAdblocker = await folderx.GetFileAsync(filepathx);
                        fileExtensionAdblockerText = await FileIO.ReadTextAsync(fileExtensionAdblocker);
                    }
            }
            catch
            {
            }
        }

        private async void DevItem_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = await AppWindow.TryCreateAsync();
            var appWindowContentFrame = new Frame();
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
                var toolCount = ExtensionsListToolbar.Items.Count;
                var math = toolCount * 35;
                var total = math + 340;
                SearchWebBox.Width = Window.Current.Bounds.Width - total;
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
            }
            catch
            {
                try
                {
                    SearchWebBox.Width = 900;
                    if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile") SearchWebMobileBox.Width = 900;
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
                    var heightString =
                        await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                    if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
                    webView.Height = Window.Current.Bounds.Height; // - 87;
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
            else
            {
                if (ApplicationView.GetForCurrentView().IsFullScreenMode)
                    try
                    {
                        int height;
                        var heightString =
                            await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                        if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
                        webView.Height = Window.Current.Bounds.Height; // - 87;
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
                else
                    try
                    {
                        int height;
                        var heightString =
                            await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                        if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
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

        private async void WebView_Memory(object source, ElapsedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { GC.Collect(2); });
        }

        private void ShareIMGItem_Click(object sender, RoutedEventArgs e)
        {
            ShareIMG = true;
            DataTransferManager.ShowShareUI();
        }

        private async void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var
                func = string.Format("document.documentElement.style.zoom = (1.0 - " + e.NewValue +
                                     " * 0.01); "); //" function ZoomFunction(" + e.NewValue.ToString() + ") { var mybody = document.body;  if (Percentage < 100){  mybody.style.overflow = 'hidden'; NewWidth = (100 * OriginalWidth) / Percentage; NewHeight = (100 * OriginalHeight) / Percentage;  } else if (Percentage == 100) {mybody.style.overflow = 'hidden'; NewWidth = OriginalWidth; NewHeight = OriginalHeight;}  else{  mybody.style.overflow = 'auto'; NewWidth = OriginalWidth * (Percentage / 100);   NewHeight = OriginalHeight * (Percentage / 10);    } }";
            if (webView != null) await webView.InvokeScriptAsync("eval", new[] {func});
        }

        public async void Dispose()
        {
            disposing = true;
            var count = 0;
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Start();
            timer.Tick += (s, p) =>
            {
                webView.Source = new Uri("about:blank");
                count++;
                if (count == 20) timer.Stop();
            };
            await Task.Delay(22000);
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
            ContentGrid.Children.Remove(webView);
            VisualTreeHelper.DisconnectChildrenRecursive(webView);
            webView = null;
            GC.Collect();
            try
            {
                InfoTip.CloseButtonClick -= InfoTip_CloseButtonClick;
                FindTip.CloseButtonClick -= FindTip_CloseButtonClick;
                ReadTip.CloseButtonClick -= ReadTip_CloseButtonClick;
            }
            catch
            {
            }

            var dataTransferManager = DataTransferManager.GetForCurrentView();
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
            SearchWebBox.TextChanged -= SearchWebBox_TextChanged;
            SearchWebBox.QuerySubmitted -= SearchBox_QuerySubmitted;
            SearchWebBox.SuggestionChosen -= SearchWebBox_SuggestionChosen;
            ContentGrid.Children.Clear();
            ContentControl.Header = null;
            try
            {
                UnloadObject(ContentControl);
            }
            catch
            {
            }

            Content = null;
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
                savePicker.FileTypeChoices.Add("png image", new List<string> {".png"});
                savePicker.SuggestedFileName = "Saved image";
                var sFile = await savePicker.PickSaveFileAsync();
                if (sFile != null)
                {
                    CachedFileManager.DeferUpdates(sFile);


                    var client = new HttpClient(); // Create HttpClient
                    var bBuffer = await client.GetByteArrayAsync(new Uri(NewWindowLink)); // Download file
                    using (var sstream = await sFile.OpenStreamForWriteAsync())
                    {
                        sstream.Write(bBuffer, 0, bBuffer.Length); // Save
                    }


                    var status = await CachedFileManager.CompleteUpdatesAsync(sFile);
                }
            }
            catch
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("The image couldnt be saved, Please try again", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("The image couldnt be saved, Please try again",
                        duration);
                }
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
        }

        private async void CopyIMGItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataPackage = new DataPackage();
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
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Couldnt copy the image", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Couldnt copy the image", duration);
                }
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
        }

        private async void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            var dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            try
            {
                dataPackage.SetText(NewWindowLink);
            }
            catch
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("We couldnt copy the link", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("We couldnt copy the link", duration);
                }
            }

            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
        }

        private async void IncognitoItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = webView.Source + NewWindowLink;
            var uriNewWindow = new Uri(@"swiftlaunchincognito:" + NewWindowLink);
            await Launcher.LaunchUriAsync(uriNewWindow);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
        }

        private async void WindowItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = webView.Source + NewWindowLink;
            var uriNewWindow = new Uri(@"swiftlaunch:" + NewWindowLink);
            localSettings.Values["SourceToGo"] = null;
            await Launcher.LaunchUriAsync(uriNewWindow);
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
        }

        private async void NewAppItem_Click(object sender, RoutedEventArgs e)
        {
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            var ws = webView.Source.ToString();
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                WebApp.WebViewNavigationString = ws;
                frame.Navigate(typeof(WebApp));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
        }

        private async void AppItem_Click(object sender, RoutedEventArgs e)
        {
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                WebApp.WebViewNavigationString = NewWindowLink;
                frame.Navigate(typeof(WebApp));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
        }

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
                        if (WebviewDataPackage != null && ContextMenu.hrefLink)
                        {
                            try
                            {
                                var pointerPosition = CoreWindow.GetForCurrentThread().PointerPosition;
                                var x = pointerPosition.X - Window.Current.Bounds.X;
                                var y = pointerPosition.Y;
                                var d = WebviewDataPackage.GetView();
                                if (d.GetWebLinkAsync() != null)
                                {
                                    var u = await d.GetWebLinkAsync();
                                    NewWindowLink = u.ToString();
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyout.Hide(); });
                                    //   TabviewMain.ContextFlyout = ContextFlyout;
                                    var ee = new FlyoutShowOptions();
                                    ee.Position = pointerPosition;
                                    ContextFlyout.ShowAt(TabviewMain, ee);
                                    ContextMenu.hrefLink = false;
                                    WebviewDataPackage = null;
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyoutImage.Hide(); });
                                    isf = false;
                                }
                                else if (d != null && ContextMenu.SRC != null)
                                {
                                    isf = true;
                                    NewWindowLink = ContextMenu.SRC;
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyout.Hide(); });
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyoutImage.Hide(); });
                                    //  TabviewMain.ContextFlyout = ContextFlyoutImage;
                                    var ee = new FlyoutShowOptions();
                                    ee.Position = pointerPosition;
                                    ContextFlyoutImage.ShowAt(TabviewMain, ee);
                                    ContextMenu.SRC = null;
                                    WebviewDataPackage = null;
                                    isf = false;
                                }
                                else if (ContextMenu.hrefLink)
                                {
                                    var u = await d.GetWebLinkAsync();
                                    NewWindowLink = u.ToString();
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyout.Hide(); });
                                    var ee = new FlyoutShowOptions();
                                    ee.Position = pointerPosition;
                                    ContextFlyout.ShowAt(TabviewMain, ee);
                                    ContextMenu.hrefLink = false;
                                    WebviewDataPackage = null;
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () => { ContextFlyoutImage.Hide(); });
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
            if (view.IsFullScreenMode)
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                var Thicc = new Thickness();
                Thicc.Top = -100;
                TabviewMain.Margin = Thicc;
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                var Thicc = new Thickness();
                Thicc.Top = -10;
                TabviewMain.Margin = Thicc;
            }
        }

        private async void FirstItem_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SourceToGo"] = NewWindowLink;
            localSettings.Values["BackupSourceToGo"] = NewWindowLink; //webView.Source + NewWindowLink;
            NewTabItem_Click(sender, e);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyoutImage.Hide(); });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ContextFlyout.Hide(); });
        }

        private async void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (webView.CanGoForward) IsForwardEnabled = true;
            IsBackEnabled = true;
            if (disposing == false)
            {
                NavigationTip.Text = "Navigation finished";
                NavigationTipGrid.Visibility = Visibility.Collapsed;
                try
                {
                    var ConsoleFunctionString =
                        @"window.onerror = function(error, url, line) {var r = 'ERR:' + error + ' url' + url + ' Line: ' + line; Window.ConsoleWinRT.setLogCombination(r); };";
                    await webView.InvokeScriptAsync("eval", new[] {ConsoleFunctionString});
                    var functionString = @"document.oncontextmenu = function (e) {
                       window.Context.setHREFCombination();
                     };";
                    await webView.InvokeScriptAsync("eval", new[] {functionString});
                    if ((bool) localSettings.Values["DarkMode"]) DarkMode.DarkMode_Click(webView);

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
                    var heightString =
                        await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                    if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
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
            if (IncognitoMode == false && (bool) ApplicationData.Current.LocalSettings.Values["StoreHistory"])
            {
                var channel = UserActivityChannel.GetDefault();
                var userActivity = await channel.GetOrCreateUserActivityAsync(webView.Source.ToString());
                string x;
                try
                {
                    x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
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
                x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
                CurrentTab.Header = x;
            }
            catch
            {
                CurrentTab.Header = webView.Source;
                x = webView.Source.ToString();
            }

            try
            {
                if (IncognitoMode == false && (bool) ApplicationData.Current.LocalSettings.Values["StoreHistory"])
                    DataAccess.AddData(webView.Source.ToString(), x);
            }
            catch
            {
            }

            try
            {
                var ArgsUri = new Uri(webView.Source.ToString());
                var host = ArgsUri.Host;
                CurrentTab.IconSource = new WinUI.BitmapIconSource
                    {UriSource = new Uri("http://icons.duckduckgo.com/ip2/" + host + ".ico"), ShowAsMonochrome = false};
                RefreshButtonIcon.Glyph = "\uE72C";
                OnPropertyChanged(nameof(IsBackEnabled));
                OnPropertyChanged(nameof(IsForwardEnabled));
                if (webView.Source.ToString().Contains(SearchEngine))
                {
                    var stre = webView.Source.ToString().Replace(SearchEngine, string.Empty);
                    SearchWebBox.Text = stre;
                    var textBox = SearchWebBox.GetDescendantsOfType<TextBox>().FirstOrDefault();
                    textBox.Select(0, 0);
                }
                else
                {
                    SearchWebBox.Text = webView.Source.ToString();
                    var textBox = SearchWebBox.GetDescendantsOfType<TextBox>().FirstOrDefault();
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

        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (disposing == false)
            {
                if (IncognitoMode)
                {
                    var gotouri = args.Uri;
                    var myFilter = new HttpBaseProtocolFilter();
                    var cookieManager = myFilter.CookieManager;
                    myFilter.ClearAuthenticationCache();
                    myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    myFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                    var myCookieJar = cookieManager.GetCookies(gotouri);
                    foreach (var cookie in myCookieJar) cookieManager.DeleteCookie(cookie);
                    myFilter.Dispose();
                }

                if (args.IsSuccess == false)
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
                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Globe};
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


        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webView.CanGoBack && HomeFrame.IsLoaded == false)
                {
                    webView.GoBack();
                }
                else if (webView.CanGoBack)
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                else if (HomeFrame.IsLoaded | (InkingFrameGrid.Visibility == Visibility.Visible))
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                    if (webView.CanGoForward) IsForwardEnabled = true;
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

                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
                    CurrentTab.Header = "Home Tab";
                }
            }
            catch
            {
                if (webView.CanGoBack && InkingFrameGrid.Visibility == Visibility.Visible)
                {
                    navigated = false;
                    UnloadObject(HomeFrame);
                    var winRTObject = new ContextMenu();
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
                else if (webView.CanGoBack)
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
                    if (webView.CanGoForward) IsForwardEnabled = true;
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

                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
                    CurrentTab.Header = "Home Tab";
                }
            }
        }

        private void OnGoForward(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward) webView.GoForward();
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RefreshButtonIcon.Glyph == "\uE711" && webView.CanGoBack)
                    webView.GoBack();
                else if (HomeFrame.Visibility == Visibility.Collapsed) webView.Refresh();
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
            if (HomeFrame == null) await Launcher.LaunchUriAsync(webView.Source);
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsSuggestionListOpen = false;
            if (args.QueryText != "")
            {
                var winRTObject = new ContextMenu();
                webView.AddWebAllowedObject("Context", winRTObject);
                var winRTConsole = new ConsoleLog();
                webView.AddWebAllowedObject("ConsoleWinRT", winRTConsole);
                var result = Regex.Match(args.QueryText, @"(.{4})\s*$").ToString();
                if (args.QueryText.Contains(" ") == false)
                {
                    if (result.Contains(".") || args.QueryText.Contains("/"))
                    {
                        if (args.QueryText.StartsWith("http://") || args.QueryText.StartsWith("https://")
                                                                 || args.QueryText.StartsWith("ftp://"))
                            webView.Navigate(new Uri(args.QueryText));
                        else
                            webView.Navigate(new Uri("http://" + args.QueryText));
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
                    var Thicc = new Thickness();
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
                        var heightString =
                            await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                        if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
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
                    var Thicc = new Thickness();
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
                    var Thicc = new Thickness();
                    Thicc.Top = -8.5;
                    TabviewMain.Margin = Thicc;
                    Bar.Visibility = Visibility.Visible;
                    var thicc = new Thickness();
                    thicc.Top = 0;
                    webView.Margin = thicc;
                    Bar.Height = 60;
                    await Task.Delay(1350);
                    //  TabviewMain.Height = webView.Height - 300;
                    //   webView.Height = webView.Height - 300;
                    try
                    {
                        int height;
                        var heightString =
                            await webView.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

                        if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");
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
                    var Thicc = new Thickness();
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
            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(webView);
            var localFolder = ApplicationData.Current.LocalFolder;
            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            var file = await localFolder.CreateFileAsync("xyzyx" + ".jpg");
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint) rtb.PixelWidth,
                    (uint) rtb.PixelHeight,
                    displayInformation.RawDpiX,
                    displayInformation.RawDpiY,
                    pixels);
                await encoder.FlushAsync();
            }
        }

        private async void webView_UnsupportedUriSchemeIdentified(WebView sender,
            WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            await Launcher.LaunchUriAsync(args.Uri);
        }

        private void webView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            WebViewEvents.webView_UnsafeContentWarningDisplaying(sender, args);
        }

        private void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            if (disposing == false)
                try
                {
                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Refresh};
                    CurrentTab.Header = "Loading...";
                    NavigationTipGrid.Visibility = Visibility.Visible;
                    NavigationTip.Text = "Loading content for " + args.Uri;
                    navigated = false;
                    NavTimer = new Timer();
                    NavTimer.Elapsed += WebView_NavCompleted;
                    NavTimer.Interval = 8000;
                    NavTimer.Enabled = true;
                }
                catch
                {
                }
        }

        private void WebView_SeparateProcessLost(WebView sender, WebViewSeparateProcessLostEventArgs args)
        {
            FindName("EmergencyFrame");
            Processlostframe.Navigate(typeof(PorcessLost));
            try
            {
                CurrentTab.Header = "Process Lost";
                CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Delete};
            }
            catch
            {
            }

            //never realized i spell wrong
        }

        private void webView_UnviewableContentIdentified(WebView sender,
            WebViewUnviewableContentIdentifiedEventArgs args)
        {
            try
            {
                downloadSource = args.Uri;
                if (downloadSource.ToString().EndsWith(".pdf"))
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
                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Document};
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


        private void InfoTip_CloseButtonClick(WinUI.TeachingTip sender, object args)
        {
            webView.GoBack();
        }

        private async void webView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            var ParentTab = TabviewMain;
            var newTab = new WinUI.TabViewItem();
            var ArgsUri = new Uri(args.Uri.ToString());
            var host = ArgsUri.Host;
            newTab.IconSource = new WinUI.BitmapIconSource
                {UriSource = new Uri("http://icons.duckduckgo.com/ip2/" + host + ".ico"), ShowAsMonochrome = false};
            ;
            var x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
            newTab.Header = args.Uri.ToString();
            // The Content of a TabViewItem is often a frame which hosts a page.
            var frame = new Frame();
            newTab.Content = frame;
            if (IncognitoMode)
                IncognitoModeStatic = false;
            else
                IncognitoModeStatic = true;
            localSettings.Values["SourceToGo"] = args.Uri.ToString();
            CurrentMainTab = newTab;
            MainTab = TabviewMain;
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
            if ((string) ApplicationData.Current.LocalSettings.Values["UserAgent"] != "default")
            {
                webView.NavigationStarting -= webView_NavigationStarting;
                args.Cancel = true;
                NavigateWithHeader(args.Uri);
            }

            if (disposing == false)
            {
                navigated = false;
                UnloadObject(HomeFrame);
                var winRTObject = new ContextMenu();
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
                        CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Refresh};
                        CurrentTab.Header = "Starting...";
                        navigated = false;
                    }
                }
                catch
                {
                    args.Cancel = true;
                    CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Globe};
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
                if ((string) ApplicationData.Current.LocalSettings.Values["UserAgent"] != "default")
                {
                    var requestMsg = new HttpRequestMessage(HttpMethod.Get, uri);
                    requestMsg.Headers.Add("User-Agent",
                        (string) ApplicationData.Current.LocalSettings.Values["UserAgent"]);
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (navigated == false)
                    {
                        navigated = true;
                        if (webView.CanGoForward) IsForwardEnabled = true;
                        IsBackEnabled = true;

                        if (disposing == false)
                        {
                            NavigationTip.Text = "Navigation finished";
                            NavigationTipGrid.Visibility = Visibility.Collapsed;
                            try
                            {
                                var ConsoleFunctionString =
                                    @"window.onerror = function(error, url, line) {var r = 'ERR:' + error + ' url' + url + ' Line: ' + line; Window.ConsoleWinRT.setLogCombination(r); };";
                                await webView.InvokeScriptAsync("eval", new[] {ConsoleFunctionString});
                                var functionString = @"document.oncontextmenu = function (e) {
                       window.Context.setHREFCombination();
                     };";
                                await webView.InvokeScriptAsync("eval", new[] {functionString});
                                if ((bool) localSettings.Values["DarkMode"]) DarkMode.DarkMode_Click(webView);
                                int height;
                                var heightString = await webView.InvokeScriptAsync("eval",
                                    new[] {"document.body.scrollHeight.toString()"});

                                if (!int.TryParse(heightString, out height))
                                    throw new Exception("Unable to get page height");
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
                            if (IncognitoMode)
                            {
                                var gotouri = webView.Source;
                                var myFilter = new HttpBaseProtocolFilter();
                                var cookieManager = myFilter.CookieManager;
                                myFilter.ClearAuthenticationCache();
                                myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                                myFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                                myFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                                var myCookieJar = cookieManager.GetCookies(gotouri);
                                foreach (var cookie in myCookieJar) cookieManager.DeleteCookie(cookie);
                                myFilter.Dispose();
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
            await WebView.ClearTemporaryWebDataAsync();
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
                if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
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
                if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
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
                if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
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
                if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
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
                if (HomeFrame.IsLoaded) Favourites.BoolWeb = false;
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
            if (webView.CanGoForward) IsForwardEnabled = true;
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

            CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
            CurrentTab.Header = "Home Tab";
        }

        /*  public class Suggestions
           {
               public string Suggestion { get; set; }
           }
           */
        private async void SearchWebBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (navigated)
            {
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                try
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                    {
                        SuggestionsList = new List<string>();
                        var client = new Windows.Web.Http.HttpClient();
                        var response =
                            await client.GetAsync(
                                new Uri("https://ac.ecosia.org/autocomplete?q=" + sender.Text + "&type="));
                        var SuggestionsJSONString = await response.Content.ReadAsStringAsync();
                        var SuggestionList = JsonConvert.DeserializeObject<SuggestionsClass>(SuggestionsJSONString);
                        foreach (var item in SuggestionList.suggestions)
                            /*  SuggestionsList.Add(
    
                                      Suggestion = item.ToString()
                                  );*/
                            SuggestionsList.Add(item);
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

        private void SearchWebBox_SuggestionChosen(AutoSuggestBox sender,
            AutoSuggestBoxSuggestionChosenEventArgs args)
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
            await Launcher.LaunchUriAsync(uriNewWindow);
        }


        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            var uriNewWindow = new Uri(@"swiftlaunch:");
            localSettings.Values["SourceToGo"] = null;
            await Launcher.LaunchUriAsync(uriNewWindow);
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
            await Launcher.LaunchUriAsync(
                new Uri("ms-windows-store://pdp/?ProductId=9mzbfrmz0mnj"));
        }

        private async void SaveWebPageAsHTMLFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var httpclient = new Windows.Web.Http.HttpClient();
            var result = await httpclient.GetBufferAsync(new Uri(webView.Source.ToString()));
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation =
                PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Hyper Text Markup Language",
                new List<string> {".html"});
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "Saved webpage";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteBufferAsync(file, result);
                var status =
                    await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    var duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("File: " + file.Name + "is saved.",
                            duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show(
                            "File: " + file.Name + "is saved.", duration);
                    }
                }
                else
                {
                    var duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show(
                            "File: " + file.Name + "couldn't be saved.", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show(
                            "File: " + file.Name + "couldn't be saved.", duration);
                    }
                }
            }
        }

        private void ShareFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ShareIMG = false;
            DataTransferManager.ShowShareUI();
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender,
            DataRequestedEventArgs args)
        {
            if (ShareIMG == false)
            {
                var request = args.Request;
                var x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
                request.Data.Properties.Title = x;
                var ArgsUri = new Uri(webView.Source.ToString());
                var host = ArgsUri.Host;
                request.Data.Properties.Description = host;
                request.Data.SetText(webView.Source.ToString());
            }
            else
            {
                var request = args.Request;
                var x = "Shared Image";
                request.Data.Properties.Title = x;
                request.Data.SetBitmap(
                    RandomAccessStreamReference.CreateFromUri(new Uri(NewWindowLink)));
            }
        }

        private void NewTabItem_Click(object sender, RoutedEventArgs e)
        {
            if (IncognitoMode)
            {
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
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
                var frame = new Frame();
                newTab.Content = frame;
                IncognitoModeStatic = true;
                CurrentMainTab = newTab;
                MainTab = TabviewMain;
                frame.Navigate(typeof(WebViewPage));
                TabviewMain.TabItems.Add(newTab);
                try
                {
                    TabViewPage.titlebar.Width = TabViewPage.titlebar.Width =
                        Window.Current.Bounds.Width - 60 -
                        TabviewMain.TabItems.Count * newTab.Width;
                }
                catch
                {
                    TabViewPage.titlebar.Width = 240;
                }
            }
            else
            {
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
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
                var frame = new Frame();
                newTab.Content = frame;
                IncognitoModeStatic = false;
                CurrentMainTab = newTab;
                MainTab = TabviewMain;
                frame.Navigate(typeof(WebViewPage));
                IncognitoModeStatic = false;
                TabviewMain.TabItems.Add(newTab);
                try
                {
                    TabViewPage.titlebar.Width = TabViewPage.titlebar.Width =
                        Window.Current.Bounds.Width - 60 -
                        TabviewMain.TabItems.Count * newTab.Width;
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

            var x = "e";
            try
            {
                x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
            }
            catch
            {
                x = webView.Source.ToString();
            }

            var RectangleToPrint = new Grid();
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
            grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});

            grid.RowDefinitions.Add(new RowDefinition
                {Height = new GridLength(1, GridUnitType.Star)});

            grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});


            // Static header

            var header = new TextBlock
                {Text = "Swift browser web printing (beta)", Margin = new Thickness(0, 0, 0, 20)};

            Grid.SetRow(header, 0);

            grid.Children.Add(header);


            // Main content with layout from data template
            var Prints = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Prints);
            var bit = new Image();
            var ee = new BitmapImage();
            ee.SetSource(Prints);
            bit.Source = ee;
            grid.Children.Add(bit);


            // Footer with page number

            pageNumber++;

            var footer = new TextBlock
                {Text = string.Format("page {0}", pageNumber), Margin = new Thickness(0, 20, 0, 0)};

            Grid.SetRow(footer, 2);

            grid.Children.Add(footer);
            printHelper.AddFrameworkElementToPrint(grid);
            //   printHelper.AddFrameworkElementToPrint(webView);
            //printHelper.AddFrameworkElementToPrint(MyPrintPages);
            // webView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            // Start printing process

            await printHelper.ShowPrintUIAsync(x);
        }

        private async Task<IEnumerable<FrameworkElement>> GetWebPages(WebView webView,
            Size pageSize)
        {
            // GETTING WIDTH FROM WEVIEW CONTENT
            var widthFromView = await webView.InvokeScriptAsync("eval",
                new[] {"document.body.scrollWidth.toString()"});

            int contentWidth;
            if (!int.TryParse(widthFromView, out contentWidth))
                throw new Exception(string.Format("failure/width:{0}", widthFromView));
            webView.Width = contentWidth;

            // GETTING HEIGHT FROM WEBVIEW CONTENT
            var heightFromView = await webView.InvokeScriptAsync("eval",
                new[] {"document.body.scrollHeight.toString()"});

            int contentHeight;
            if (!int.TryParse(heightFromView, out contentHeight))
                throw new Exception(string.Format("failure/height:{0}", heightFromView));

            webView.Height = contentHeight;

            // CALCULATING NO OF PAGES
            var scale = pageSize.Width / contentWidth;
            var scaledHeight = contentHeight * scale;
            var pageCount = scaledHeight / pageSize.Height;
            pageCount = pageCount + (pageCount > (int) pageCount ? 1 : 0);

            // CREATE PAGES
            var pages = new List<Rectangle>();
            for (var i = 0; i < (int) pageCount; i++)
            {
                var translateY = -pageSize.Height * i;
                var page = new Rectangle
                {
                    Height = pageSize.Height,
                    Width = pageSize.Width,
                    Margin = new Thickness(5),
                    Tag = new TranslateTransform {Y = translateY}
                };

                page.Loaded += async (s, e) =>
                {
                    var rectangle = s as Rectangle;
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

        private async Task<WebViewBrush> GetWebViewBrush(WebView webView)
        {
            // ASSING ORIGINAL CONTENT WIDTH
            var originalWidth = webView.Width;

            var widthFromView = await webView.InvokeScriptAsync("eval",
                new[] {"document.body.scrollWidth.toString()"});

            int contentWidth;
            if (!int.TryParse(widthFromView, out contentWidth))
                throw new Exception(string.Format("failure/width:{0}", widthFromView));
            //  webView.Width = contentWidth;

            // ASSINGING ORIGINAL CONTENT HEIGHT
            var originalHeight = webView.Height;

            var heightFromView = await webView.InvokeScriptAsync("eval",
                new[] {"document.body.scrollHeight.toString()"});

            int contentHeight;
            if (!int.TryParse(heightFromView, out contentHeight))
                throw new Exception(string.Format("failure/height:{0}", heightFromView));
            // webView.Height = contentWidth;

            // CREATING BRUSH
            var originalVisibilty = webView.Visibility;
            webView.Visibility = Visibility.Visible;

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
            CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Edit};
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
            {
            }

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
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
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
            await Launcher.LaunchUriAsync(
                new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }

        public async void About(object sender, RoutedEventArgs e)
        {
            var about = "About: " + "AppDisplayName".GetLocalizedSwift() + "\n" +
                        Package.Current.Id + "\n" + Package.Current.Id.Version + "\n" +
                        Package.Current.Id.Publisher;
            try
            {
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

        private async void PinSecondaryLiveTileMenuFlyoutSubItemPinItem_Click(object sender,
            RoutedEventArgs e)
        {
            var x = "";
            try
            {
                x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
                CurrentTab.Header = x;
            }
            catch
            {
                CurrentTab.Header = webView.Source.ToString();
            }

            // Use a display name you like
            var displayName = x;
            var rnd = new Random();
            var TileId = "pin" + rnd.Next();
            // Provide all the required info in arguments so that when user
            // clicks your tile, you can navigate them to the correct content
            var arguments = webView.Source.ToString();
            var ArgsUri = new Uri(webView.Source.ToString());
            var host = ArgsUri.Host;
            // Initialize the tile with required arguments
            var tile = new SecondaryTile(
                TileId,
                displayName,
                arguments,
                new Uri("ms-appx:///Assets/Square150x150Logo.png"),
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

        private async void HighlightTerm_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (!Cleared) ClearSearch();
                await webView.InvokeScriptAsync("eval",
                    new[] { HighlightFunctionJS + " HighlightFunction('" + args.QueryText + "');" });
                Cleared = false;
            }
            catch
            {

            }
        }

        public async void ClearSearch()
        {
            try
            {
                await webView.InvokeScriptAsync("eval",
                    new[] { HighlightFunctionJS + " RestoreFunction();" });
                Cleared = true;
            }
            catch
            {

            }
        }

        private void FindTip_CloseButtonClick(WinUI.TeachingTip sender, object args)
        {
            ClearSearch();
        }

        private void TranslateFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            webView.Navigate(new Uri(
                "https://translate.google.com/translate?hl=en&sl=auto&tl=en&u=" + webView.Source));
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
            var Snips = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Snips);
            OfflineCreatePage.webView = webView;
            OfflineCreatePage.NameX = Name;
            InkingFrame.Navigate(typeof(OfflineCreatePage));
            CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.ZeroBars};
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
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(ContentGrid);

                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var picker = new FileSavePicker();
                picker.FileTypeChoices.Add("JPEG Image", new[] {".jpg"});
                var file = await picker.PickSaveFileAsync();
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder =
                        await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Premultiplied,
                        (uint) rtb.PixelWidth,
                        (uint) rtb.PixelHeight,
                        displayInformation.RawDpiX,
                        displayInformation.RawDpiY,
                        pixels);
                    await encoder.FlushAsync();
                }

                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Screen captured and saved", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Screen captured and saved",
                        duration);
                }
            }
            catch
            {
            }
        }

        private async void MuteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var mutefunctionString = @" var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    [].forEach.call(videos, function(video) { video.muted = true; });
    [].forEach.call(audios, function(audio) { audio.muted = true; }); ";

            await webView.InvokeScriptAsync("eval", new[] {mutefunctionString});
        }

        private async void unMuteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var unmutefunctionString = @"var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    [].forEach.call(videos, function(video) { video.muted = false; });
    [].forEach.call(audios, function(audio) { audio.muted = false; }); ";

            await webView.InvokeScriptAsync("eval", new[] {unmutefunctionString});
        }

        private async void TempLaunchAppFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var source = webView.Source.ToString();
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                WebApp.WebViewNavigationString = source;
                frame.Navigate(typeof(WebApp));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
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
            var Snips = new InMemoryRandomAccessStream();
            await webView.CapturePreviewToStreamAsync(Snips);
            SnipPage.WebView = Snips;
            HomePage.WebViewControl = webView;
            InkingFrame.Navigate(typeof(SnipPage));
            CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Cut};
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
            var mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
            MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage + "\r\nMemory Level: " +
                            MemoryManager.AppMemoryUsageLevel;
            XMemUsage.Text = "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit +
                             "\r\nExpected Memory Usage Limit: " +
                             MemoryManager.ExpectedAppMemoryUsageLimit;
            var du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
            DiskUsage.Text = "Bytes Written Count: " + du.BytesWrittenCount +
                             "\r\nBytes Read Count:" + du.BytesReadCount +
                             "\r\nOther Bytes Count:" + du.OtherBytesCount;
            XDiskUsage.Text = "Write Operation Count: " + du.WriteOperationCount +
                              "\r\nRead Operation Count: " + du.ReadOperationCount +
                              "\r\nOther Operation Count:" + du.OtherOperationCount;
            var cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
            CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime + "\r\nCpu User Time:" +
                            cu.UserTime;
            var id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
            IdProcess.Text = "Process Id: " + id;
            var st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.ToString();
            StartTime.Text = "Start Time: " + st;
            TaskMNGTimer = new Timer();
            TaskMNGTimer.Elapsed += TaskMNGTimer_Elapsed;
            TaskMNGTimer.Interval = 100;
            TaskMNGTimer.Enabled = true;
            await TaskMNG.ShowAsync();
        }

        private async void TaskMNGTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
                    MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage +
                                    "\r\nMemory Level: " + MemoryManager.AppMemoryUsageLevel;
                    XMemUsage.Text =
                        "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit +
                        "\r\nExpected Memory Usage Limit: " +
                        MemoryManager.ExpectedAppMemoryUsageLimit;
                    var du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
                    DiskUsage.Text =
                        "Bytes Written Count: " + du.BytesWrittenCount + "\r\nBytes Read Count:" +
                        du.BytesReadCount + "\r\nOther Bytes Count:" + du.OtherBytesCount;
                    XDiskUsage.Text =
                        "Write Operation Count: " + du.WriteOperationCount +
                        "\r\nRead Operation Count: " + du.ReadOperationCount +
                        "\r\nOther Operation Count:" + du.OtherOperationCount;
                    var cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
                    CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime + "\r\nCpu User Time:" +
                                    cu.UserTime;
                    var id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
                    IdProcess.Text = "Process Id: " + id;
                    var st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime
                        .ToString();
                    StartTime.Text = "Start Time: " + st;
                });
        }

        private void RefreshTaskManagerButton_Click(object sender, RoutedEventArgs e)
        {
            var mu = ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport();
            MemUsage.Text = "Memory Usage: " + MemoryManager.AppMemoryUsage + "\r\nMemory Level: " +
                            MemoryManager.AppMemoryUsageLevel;
            XMemUsage.Text = "Memory Usage Limit: " + MemoryManager.AppMemoryUsageLimit +
                             "\r\nExpected Memory Usage Limit: " +
                             MemoryManager.ExpectedAppMemoryUsageLimit;
            var du = ProcessDiagnosticInfo.GetForCurrentProcess().DiskUsage.GetReport();
            DiskUsage.Text = "Bytes Written Count: " + du.BytesWrittenCount +
                             "\r\nBytes Read Count:" + du.BytesReadCount +
                             "\r\nOther Bytes Count:" + du.OtherBytesCount;
            XDiskUsage.Text = "Write Operation Count: " + du.WriteOperationCount +
                              "\r\nRead Operation Count: " + du.ReadOperationCount +
                              "\r\nOther Operation Count:" + du.OtherOperationCount;
            var cu = ProcessDiagnosticInfo.GetForCurrentProcess().CpuUsage.GetReport();
            CpuUsage.Text = "Cpu Kernel Time: " + cu.KernelTime + "\r\nCpu User Time:" +
                            cu.UserTime;
            var id = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId.ToString();
            IdProcess.Text = "Process Id: " + id;
            var st = ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.ToString();
            StartTime.Text = "Start Time: " + st;
        }

        private async void CompactFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView()
                .IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                await ApplicationView.GetForCurrentView()
                    .TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                var Thicc = new Thickness();
                Thicc.Top = -100;
                TabviewMain.Margin = Thicc;
                CompactButton.Visibility = Visibility.Visible;
                Iscompact = true;
            }
        }

        private async void CompactButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplicationView.GetForCurrentView()
                .TryEnterViewModeAsync(ApplicationViewMode.Default);
            var Thicc = new Thickness();
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
            var html = await webView.InvokeScriptAsync("eval",
                new[] {"document.documentElement.outerHTML;"});
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var sb = new StringBuilder();
            doc.DocumentNode.SelectNodes("//style|//script").ToList().ForEach(n => n.Remove());
            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//text()"))
                sb.AppendLine(node.Text);
            var final = sb.ToString();
            var synth = new SpeechSynthesizer();
            var stream = await synth.SynthesizeTextToStreamAsync(final);

            // Send the stream to the media object.
            ReadAloudElement.SetSource(stream, stream.ContentType);
            ReadAloudElement.Play();
            ReadTip.IsOpen = true;
            MainFlyout.Hide();
            SecondFlyout.Hide();
        }


        private void ReadTip_CloseButtonClick(WinUI.TeachingTip sender, object args)
        {
            ReadAloudElement.Stop();
        }

        private void ReadingSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            if (toggle.IsOn)
                ReadAloudElement.Play();
            else
                ReadAloudElement.Pause();
        }

        private void ReadingModeButton_Click(object sender, RoutedEventArgs e)
        {
            var url = webView.Source.ToString();

            var t = new NReadabilityWebTranscoder();
            bool b;
#pragma warning disable CS0618 // Type or member is obsolete
            var page = t.Transcode(url, out b);
#pragma warning restore CS0618 // Type or member is obsolete

            if (b)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(page);
                MainFlyout.Hide();
                SecondFlyout.Hide();
                var title = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                var imgUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")
                    .Attributes["content"].Value;
                var mainText = doc.DocumentNode.SelectSingleNode("//div[@id='readInner']")
                    .InnerText;
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
                CurrentTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Home};
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
            await Launcher.LaunchUriAsync(downloadSource);
        }

        private async void DevButton_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = await AppWindow.TryCreateAsync();
            var appWindowContentFrame = new Frame();
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

        private async void ExtensionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ExtensionsListJSON = new List<ExtensionsJSON>();
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync("Extensions.json"); // error here
                var JSONData = "e";
                JSONData = await FileIO.ReadTextAsync(file);
                var ExtensionsListJSONJSON =
                    JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
                foreach (var item in ExtensionsListJSONJSON.Extensions)
                    if (item.IsEnabledJSON)
                        ExtensionsListJSON.Add(new ExtensionsJSON
                        {
                            NameJSON = item.NameJSON,
                            DescriptionJSON = item.DescriptionJSON,
                            IconJSON = item.IconJSON,
                            Page = item.Page
                        });
                ExtensionsList.ItemsSource = ExtensionsListJSON;
            }
            catch
            {
            }
        }


        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            var f = sender as Frame;
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Json = DataContext as ExtensionsJSON;
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var f = sender as Frame;
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Json = DataContext as ExtensionsJSON;

            if (Json.Page == "nonexistentemojipage")
            {
                CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
            }
            else if (Json.Page.Contains("http"))
            {
                ExtensionsplitView.IsPaneOpen = true;

                webviewE = new WebView(WebViewExecutionMode.SeparateProcess);
                webviewE.Width = 500;

                ExtensionsplitView.Content = webviewE;
                webviewE.Navigate(new Uri(Json.Page));
            }
            else
            {
                var i = sender as Image;
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
            var SecureText = sender as TextBlock;
            try
            {
                if (webView.Source.AbsoluteUri.StartsWith("https"))
                    SecureText.Text = "Your connection to this site is secure";
                else
                    SecureText.Text = "Your connection to this site is not secure";
            }
            catch
            {
                SecureText.Text = "No site is laoded";
            }
        }

        public async Task<X509Certificate2> GetServerCertificateAsync()
        {
            X509Certificate2 certificate = null;
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, cert, __, ___) =>
                {
                    certificate = new X509Certificate2(cert.GetRawCertData());
                    return true;
                }
            };

            var httpClient = new HttpClient(httpClientHandler);
            await httpClient.SendAsync(
                new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Head,
                    webView.Source));

            return certificate ?? throw new NullReferenceException();
        }

        private async void SecureCertificateTemporary_Loaded(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBlock;
            try
            {
                X509Certificate xcertificate = await GetServerCertificateAsync();
                var cert2 = new X509Certificate2(xcertificate);
                s.Text = cert2.ToString();
            }
            catch
            {
                s.Text = "No site is laoded";
            }
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
                var filepath = @"Assets\Favorites.json";
                var folder = Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(filepath); // error here
                var JSONsData = await FileIO.ReadTextAsync(file);
                sampleFile = await localFolder.CreateFileAsync("Favorites.json",
                    CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONsData);
            }

            localSettings.Values["FirstFavRun"] = false;
            var JSONData = await FileIO.ReadTextAsync(sampleFile);
            var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
            try
            {
                var ArgsUri = new Uri(webView.Source.ToString());
                var host = ArgsUri.Host;
                var x = "";
                try
                {
                    x = await webView.InvokeScriptAsync("eval", new[] {"document.title;"});
                }
                catch
                {
                    x = webView.Source.ToString();
                }

                FavouritesListJSON.Websites.Add(new FavouritesJSON
                {
                    FavIconJSON = " https://icons.duckduckgo.com/ip2/" + host + ".ico",
                    UrlJSON = webView.Source.ToString(),
                    HeaderJSON = x
                });
                ;
                var SerializedObject =
                    JsonConvert.SerializeObject(FavouritesListJSON, Formatting.Indented);
                await FileIO.WriteTextAsync(sampleFile, SerializedObject);
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
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Something went wrong when saving",
                        duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Something went wrong when saving",
                        duration);
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
                    var randomnumber = random.Next();
                    var rand = randomnumber.ToString();
                    CreateRTBOffline(rand);
                }
            }
            catch
            {
                var random = new Random();
                var randomnumber = random.Next();
                var rand = randomnumber.ToString();
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
                var toolCount = ExtensionsListToolbar.Items.Count;
                var math = toolCount * 35;
                var total = math + 340;
                SearchWebBox.Width = Window.Current.Bounds.Width - total;
                try
                {
                    if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                        SearchWebMobileBox.Width = Window.Current.Bounds.Width - total;
                }
                catch
                {
                }
            }
            catch
            {
                SearchWebBox.Width = 900;
                try
                {
                    if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                        SearchWebMobileBox.Width = 900;
                }
                catch
                {
                }
            }

            Startup();
        }

        private void TaskMNG_CloseButtonClick(ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            TaskMNGTimer.Enabled = false;
            TaskMNGTimer = null;
        }


        private void SearchWebBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //  AutoSuggestBox s = (AutoSuggestBox)sender;
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
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
            var Thicc = new Thickness();
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
                var heightString = await webView.InvokeScriptAsync("eval",
                    new[] {"document.body.scrollHeight.toString()"});

                if (!int.TryParse(heightString, out height))
                    throw new Exception("Unable to get page height");
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
            var Thicc = new Thickness();
            Thicc.Top = -8.5;
            TabviewMain.Margin = Thicc;
            var thicc = new Thickness();
            thicc.Top = 0;
            webView.Margin = thicc;
            await Task.Delay(1350);
            //  TabviewMain.Height = webView.Height - 300;
            //   webView.Height = webView.Height - 300;
            try
            {
                int height;
                var heightString = await webView.InvokeScriptAsync("eval",
                    new[] {"document.body.scrollHeight.toString()"});

                if (!int.TryParse(heightString, out height))
                    throw new Exception("Unable to get page height");
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

        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        public class SuggestionsClass
        {
            public List<string> suggestions { get; set; }
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
    }

    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
}
