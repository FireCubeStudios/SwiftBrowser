using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using SwiftBrowser.Assets;
using SwiftBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class IncognitoTabView : Page
    {

        public static WinUI.TabView TabviewPageControl { get; set; }

        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public bool IncognitoMode;
        WinUI.TabViewItem RightClickedItem;
        Flyout PreviewFlyout = new Flyout();
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public static Grid titlebar { get; set; }
        public static Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification InAppNotificationMain { get; set; }
        public static IncognitoTabView SingletonReference { get; set; }
        public IncognitoTabView()
        {
            this.InitializeComponent();
            SingletonReference = this;
            titlebar = AppTitleBar;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            //    coreTitleBar.LayoutMetricsChanged += CustomDragRegion_SizeChanged;
            InAppNotificationMain = UniversalNormalNotificationInApp;
            Window.Current.SetTitleBar(AppTitleBar);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayRightInset;
            }

            CustomDragRegion.Height = ShellTitlebarInset.Height = sender.Height;
        }


        public class SyncClass
        {
            public List<SyncJSON> Sync { get; set; }
        }

        /*  public class Sync
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class SyncJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
        public async void StartUp()
        {

            try
            {
                if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] == true)
                {
                    // try
                    // {
                    if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] == true && Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"] == Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] == false)
                    {
                        Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                        StorageFolder roaming = Windows.Storage.ApplicationData.Current.RoamingFolder;
                        StorageFile SyncFile = await roaming.GetFileAsync("SyncFile.json");
                        string Data = await FileIO.ReadTextAsync(SyncFile);
                        SyncClass SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(Data);
                        foreach (var item in SyncListJSON.Sync)
                        {
                            DataAccess.AddDataS(item.UrlJSON, item.HeaderJSON);
                        }
                        ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                        await SyncFile.DeleteAsync();
                        Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                    }
                    else
                    {
                    }
                }
            }
            catch
            {
                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
            }

            // }
            /*  catch
              {
                  Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
              }*/


        }
        public class TabsJSON
        {
            public string TabViewItemJSON { get; set; }
        }
        public async void ShowRestoreDialog()
        {
            StorageFolder folderx = Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile filex = await folderx.GetFileAsync("RestoreTabItems.json");
                var textx = await FileIO.ReadTextAsync(filex);
                TabsClass Listx = JsonConvert.DeserializeObject<TabsClass>(textx);
                if (Listx.Tabs.Count != 0)
                {
                    if ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 2)
                    {
                        RestoreTip.IsOpen = true;
                        await Task.Delay(5000);
                        RestoreTip.IsOpen = false;
                    }
                    else if ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 1)
                    {
                        // try {
                        StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        StorageFile file = await folder.GetFileAsync("RestoreTabItems.json");
                        var text = await FileIO.ReadTextAsync(file);
                        TabsClass List = JsonConvert.DeserializeObject<TabsClass>(text);

                        foreach (var item in List.Tabs)
                        {
                            localSettings.Values["SourceToGo"] = item.TabViewItemJSON;
                            var newTab = new WinUI.TabViewItem();
                            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                            newTab.Header = "Home Tab";
                            MenuFlyout Flyout = new MenuFlyout();
                            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                            OpenInnewwindow.Text = "Move to new window";
                            OpenInnewwindow.Click += OpenInnewwindow_Click1;
                            Flyout.Items.Add(OpenInnewwindow);
                            Flyout.Items.Add(new MenuFlyoutSeparator());
                            MenuFlyoutItem Refresh = new MenuFlyoutItem();
                            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
                            Refresh.Text = "Refresh Tab";
                            Refresh.Click += Refresh_Click;
                            Flyout.Items.Add(Refresh);
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
                            CloseTab.Click += CloseTab_Click;
                            Flyout.Items.Add(CloseTab);
                            MenuFlyoutItem CloseO = new MenuFlyoutItem();
                            CloseO.Icon = new SymbolIcon(Symbol.Delete);
                            CloseO.Text = "Close other tabs";
                            CloseO.Click += CloseO_Click;
                            Flyout.Items.Add(CloseO);
                            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                            CloseAll.Text = "Close all tabs";
                            CloseAll.Click += ClearAll;
                            Flyout.Items.Add(CloseAll);

                            newTab.ContextFlyout = Flyout;
                            ToolTip T = new ToolTip();

                            newTab.PointerEntered += NewTab_PointerEntered;
                            newTab.RightTapped += NewTab_RightTapped;

                            Frame frame = new Frame();
                            WebViewPage.IncognitoModeStatic = true;
                            newTab.Content = frame;
                            WebViewPage.IncognitoModeStatic = true;
                            WebViewPage.CurrentMainTab = newTab;
                            WebViewPage.MainTab = this.IncognitoTabsControl;
                            frame.Navigate(typeof(WebViewPage));
                            WebViewPage.IncognitoModeStatic = true;
                            WebViewPage.MainTab = this.IncognitoTabsControl;
                            IncognitoTabsControl.TabItems.Add(newTab);
                            WebViewPage.MainTab = this.IncognitoTabsControl;
                            IncognitoTabsControl.SelectedItem = newTab;
                            WebViewPage.MainTab = this.IncognitoTabsControl;
                            try
                            {
                                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
                            }
                            catch
                            {
                                AppTitleBar.Width = 240;
                            }
                        }
                        /* }
                          catch
                          {
                              return;
                          }*/
                    }
                }
            }
            catch
            {
                return;
            }
        }
        public async void LoadTabViewRestore()
        {
            // try {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("RestoreTabItems.json");
            var text = await FileIO.ReadTextAsync(file);
            TabsClass List = JsonConvert.DeserializeObject<TabsClass>(text);

            foreach (var item in List.Tabs)
            {
                localSettings.Values["SourceToGo"] = item.TabViewItemJSON;
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                newTab.Header = "Home Tab";
                MenuFlyout Flyout = new MenuFlyout();
                MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                OpenInnewwindow.Text = "Move to new window";
                OpenInnewwindow.Click += OpenInnewwindow_Click1;
                Flyout.Items.Add(OpenInnewwindow);
                Flyout.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutItem Refresh = new MenuFlyoutItem();
                Refresh.Icon = new SymbolIcon(Symbol.Refresh);
                Refresh.Text = "Refresh Tab";
                Refresh.Click += Refresh_Click;
                Flyout.Items.Add(Refresh);
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
                CloseTab.Click += CloseTab_Click;
                Flyout.Items.Add(CloseTab);
                MenuFlyoutItem CloseO = new MenuFlyoutItem();
                CloseO.Icon = new SymbolIcon(Symbol.Delete);
                CloseO.Text = "Close other tabs";
                CloseO.Click += CloseO_Click;
                Flyout.Items.Add(CloseO);
                MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                CloseAll.Text = "Close all tabs";
                CloseAll.Click += ClearAll;
                Flyout.Items.Add(CloseAll);

                newTab.ContextFlyout = Flyout;
                ToolTip T = new ToolTip();

                newTab.PointerEntered += NewTab_PointerEntered;
                newTab.RightTapped += NewTab_RightTapped;

                Frame frame = new Frame();
                WebViewPage.IncognitoModeStatic = true;
                newTab.Content = frame;
                WebViewPage.IncognitoModeStatic = true;
                WebViewPage.CurrentMainTab = newTab;
                try
                {
                    AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
                }
                catch
                {
                    AppTitleBar.Width = 240;
                }
                WebViewPage.MainTab = this.IncognitoTabsControl;
                frame.Navigate(typeof(WebViewPage));
                WebViewPage.IncognitoModeStatic = true;
                WebViewPage.MainTab = this.IncognitoTabsControl;
                IncognitoTabsControl.TabItems.Add(newTab);
                WebViewPage.MainTab = this.IncognitoTabsControl;
                IncognitoTabsControl.SelectedItem = newTab;
                WebViewPage.MainTab = this.IncognitoTabsControl;
            }
            /* }
              catch
              {
                  return;
              }*/
        }
      

        private void TabsControl_TabDragStarting(WinUI.TabView sender, TabViewTabDragStartingEventArgs args)
        {
            try
            {
                WebView tag = args.Tab.Tag as WebView;
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
            }
            catch
            {
                return;
            }
        }
        public class TabsClass
        {
            public List<TabsJSON> Tabs { get; set; }
        }
        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            /* List<TabsJSON> TabsList = new List<TabsJSON>();
            foreach (var WinUI.TabViewItems in IncognitoTabsControl.TabItems)
             {
                WinUI.TabViewItem TabListTab = WinUI.TabViewItems as WinUI.TabViewItem;
                 WebView W = TabListTab.Tag as WebView;
                 TabsList.Add(new TabsJSON()
                 {
                     WinUI.TabViewItemJSON = "W",
                 });
             }
           ;*/
            string filepath = @"Assets\RestoreTabItems.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath);
            String Text = await FileIO.ReadTextAsync(file);
            TabsClass Tabslist = JsonConvert.DeserializeObject<TabsClass>(Text);
            //  List<TabsClass> Tabslist = new List<TabsClass>();
            foreach (WinUI.TabViewItem TabViewItems in IncognitoTabsControl.TabItems)
            {
                WebView tag = TabViewItems.Tag as WebView;
                try
                {
                    Tabslist.Tabs.Add(new TabsJSON()
                    {
                       TabViewItemJSON = tag.Source.ToString()
                    });
                }
                catch
                {

                }
            }
            StorageFile sampleFile = await localFolder.CreateFileAsync("RestoreTabItems.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(Tabslist));
            deferral.Complete();
        }
        private void Fav_Click(object sender, RoutedEventArgs e)
        {
            WebView FavW = RightClickedItem.Tag as WebView;

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebView FavW = RightClickedItem.Tag as WebView;
                FavW.Refresh();
            }
            catch
            {
                return;
            }
        }

        private async void OpenInnewwindow_Click1(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
            int w = (int)t.Width;
            try
            {
                if (IncognitoTabsControl.TabItems.Count <= 7)
                {
                    AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                }
                else
                {
                    AppTitleBar.Width = 240;
                }
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            IncognitoTabsControl.TabItems.Remove(RightClickedItem);
            WebView tag = RightClickedItem.Tag as WebView;
            localSettings.Values["SourceToGo"] = tag.Source.ToString();
            var uriNewWindow = new Uri(@"swiftlaunch:" + tag.Source.ToString());
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
        }

        private void NewTab_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // PreviewFlyout.Hide();
        }

        private async void NewTab_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            WinUI.TabViewItem Tab = sender as WinUI.TabViewItem;
            ToolTip T = ToolTipService.GetToolTip(Tab) as ToolTip;
            try
            {
                WebView eeeee = Tab.Tag as WebView;
                var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                var x = pointerPosition.X - Window.Current.Bounds.X;
                var y = pointerPosition.Y;
                FlyoutShowOptions ee = new FlyoutShowOptions();
                ee.Position = pointerPosition;
                InMemoryRandomAccessStream Prints = new InMemoryRandomAccessStream();
                await eeeee.CapturePreviewToStreamAsync(Prints);
                BitmapImage b = new BitmapImage();
                await b.SetSourceAsync(Prints);
                Image ThumbNail = new Image();
                ThumbNail.Source = b;
                ThumbNail.Width = 220;
                ThumbNail.Stretch = Stretch.Fill;
                Windows.UI.Xaml.Shapes.Rectangle r = new Windows.UI.Xaml.Shapes.Rectangle();
                r.Fill = new SolidColorBrush(Colors.Green);
                T.Content = ThumbNail;
                // T.IsOpen = true;
            }
            catch
            {
                T.Content = null;
            }

            // PreviewFlyout.ShowAt(Tab);
        }



        private void CloseO_Click(object sender, RoutedEventArgs e)
        {
            IncognitoTabsControl.TabItems.Clear();
            IncognitoTabsControl.TabItems.Add(RightClickedItem);
        }

        private async void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (IncognitoTabsControl.TabItems.Count <= 1)
                {
                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                    newTab.Header = "Home Tab";
                    MenuFlyout Flyout = new MenuFlyout();
                    MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                    OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                    OpenInnewwindow.Text = "Move to new window";
                    Flyout.Items.Add(OpenInnewwindow);
                    Flyout.Items.Add(new MenuFlyoutSeparator());
                    MenuFlyoutItem Refresh = new MenuFlyoutItem();
                    Refresh.Icon = new SymbolIcon(Symbol.Refresh);
                    Refresh.Text = "Refresh Tab";
                    Refresh.Click += Refresh_Click;
                    Flyout.Items.Add(Refresh);
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

                    ToolTip T = new ToolTip();
                    ToolTipService.SetToolTip(newTab, T);

                    CloseO.Click += CloseO_Click;
                    Flyout.Items.Add(CloseO);
                    MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                    CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                    CloseAll.Text = "Close all tabs";
                    CloseAll.Click += ClearAll;
                    Flyout.Items.Add(CloseAll);
                    newTab.RightTapped += NewTab_RightTapped;
                    newTab.ContextFlyout = Flyout;

                    Frame frame = new Frame();
                    newTab.Content = frame;
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.MainTab = IncognitoTabsControl;
                    IncognitoTabsControl.TabItems.Add(newTab);
                    try
                    {
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    IncognitoTabsControl.SelectedItem = newTab;
                    RightClickedItem.Content = null;
                    IncognitoTabsControl.TabItems.Remove(RightClickedItem);
                    WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (IncognitoTabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                        }
                        else
                        {
                            AppTitleBar.Width = 240;
                        }
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    GC.Collect();
                }
                else
                {
                    RightClickedItem.Content = null;
                    IncognitoTabsControl.TabItems.Remove(RightClickedItem);
                    WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (IncognitoTabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                        }
                        else
                        {
                            AppTitleBar.Width = 240;
                        }
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    GC.Collect();
                }
            });
        }

        private void NewTab_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            RightClickedItem = sender as WinUI.TabViewItem;
        }

        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            var uriNewWindow = new Uri(@"swiftlaunch:");
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
        }
        public void AddAll(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);

            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
            OpenInnewwindow.Text = "Move to new window";
            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);

            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";
            newTab.RightTapped += NewTab_RightTapped;

            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.MainTab = this.IncognitoTabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            this.IncognitoTabsControl.TabItems.Add(newTab);
            this.IncognitoTabsControl.SelectedItem = newTab;
            GC.Collect();
        }
        public void ClearAll(object sender, RoutedEventArgs e)
        {
            IncognitoTabsControl.TabItems.Clear();
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
            OpenInnewwindow.Text = "Move to new window";

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);

            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);

            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";
            newTab.RightTapped += NewTab_RightTapped;

            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            IncognitoTabsControl.TabItems.Add(newTab);
            IncognitoTabsControl.SelectedItem = newTab;
            GC.Collect();
        }
        private async void Incognito_Click(object sender, RoutedEventArgs e)
        {
            var uriNewWindow = new Uri(@"swiftlaunchincognito:");
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
        }

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var senderTabView = args.Element as WinUI.TabView;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            newTab.Header = "Home Tab";
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);

            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
            OpenInnewwindow.Text = "Move to new window";
            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);
            newTab.RightTapped += NewTab_RightTapped;
            newTab.ContextFlyout = Flyout;
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = IncognitoTabsControl;
            IncognitoTabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.IncognitoTabsControl;
            IncognitoTabsControl.SelectedItem = newTab;
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as WinUI.TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            if (tabToSelect < InvokedTabView.TabItems.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }
            args.Handled = true;
        }




        private void OnAddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);

            OpenInnewwindow.Text = "Move to new window";
            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);
            newTab.RightTapped += NewTab_RightTapped;
            newTab.PointerEntered += NewTab_PointerEntered;
            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";

            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = IncognitoTabsControl;
            sender.TabItems.Add(newTab);
            WebViewPage.MainTab = this.IncognitoTabsControl;
            this.IncognitoTabsControl.SelectedItem = newTab;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            GC.Collect(2);
        }

        private async void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (sender.TabItems.Count <= 1)
                {
                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                    newTab.Header = "Home Tab";
                    MenuFlyout Flyout = new MenuFlyout();
                    MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                    OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
                    OpenInnewwindow.Text = "Move to new window";
                    Flyout.Items.Add(OpenInnewwindow);

                    ToolTip T = new ToolTip();
                    ToolTipService.SetToolTip(newTab, T);
                    MenuFlyoutItem Refresh = new MenuFlyoutItem();
                    Refresh.Icon = new SymbolIcon(Symbol.Refresh);
                    Refresh.Text = "Refresh Tab";
                    Refresh.Click += Refresh_Click;
                    Flyout.Items.Add(Refresh);
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
                    CloseO.Click += CloseO_Click;
                    Flyout.Items.Add(CloseO);
                    MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                    CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                    CloseAll.Text = "Close all tabs";
                    CloseAll.Click += ClearAll;
                    Flyout.Items.Add(CloseAll);
                    newTab.RightTapped += NewTab_RightTapped;
                    newTab.ContextFlyout = Flyout;

                    Frame frame = new Frame();
                    newTab.Content = frame;
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.CurrentMainTab = newTab;

                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.MainTab = IncognitoTabsControl;
                    sender.TabItems.Add(newTab);
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    sender.SelectedItem = newTab;
                    try
                    {
                        WebView es = args.Tab.Tag as WebView;
                        WebViewPage we = es.Tag as WebViewPage;
                        we.Dispose();
                        es = null;
                    }
                    catch
                    {

                    }
                    args.Tab.Content = null;
                    args.Tab.ContextFlyout = null;
                    args.Tab.Tag = null;

                    args.Tab.PointerEntered -= NewTab_PointerEntered;
                    args.Tab.PointerExited -= NewTab_PointerExited;
                    args.Tab.RightTapped -= NewTab_RightTapped;
                    VisualTreeHelper.DisconnectChildrenRecursive(args.Tab);
                    //  ((IDisposable)sender.TabItems).Dispose();
                    sender.TabItems.Remove(args.Tab);
                    int w = (int)args.Tab.Width;
                    try
                    {
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    await Task.Delay(500);
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
                else
                {
                    try
                    {
                        WebView es = args.Tab.Tag as WebView;
                        WebViewPage we = es.Tag as WebViewPage;
                        we.Dispose();
                        es = null;
                    }
                    catch
                    {

                    }

                    args.Tab.Content = null;
                    args.Tab.ContextFlyout = null;
                    args.Tab.Tag = null;
                    args.Tab.PointerEntered -= NewTab_PointerEntered;
                    args.Tab.PointerExited -= NewTab_PointerExited;
                    args.Tab.RightTapped -= NewTab_RightTapped;
                    VisualTreeHelper.DisconnectChildrenRecursive(args.Tab);
                    //  ((IDisposable)sender.TabItems).Dispose();
                    sender.TabItems.Remove(args.Tab);

                    await Task.Delay(500);
                    WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (IncognitoTabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                        }
                        else
                        {
                            AppTitleBar.Width = 240;
                        }
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }

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




        private void IncognitoTabsControl_TabStripDrop(object sender, DragEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            newTab.Header = "Home Tab";
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
            OpenInnewwindow.Text = "Move to new window";
            Flyout.Items.Add(OpenInnewwindow);

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);
            newTab.RightTapped += NewTab_RightTapped;
            newTab.ContextFlyout = Flyout;

            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = IncognitoTabsControl;
            IncognitoTabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.IncognitoTabsControl;
            IncognitoTabsControl.SelectedItem = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
        }

        private async void IncognitoTabsControl_TabDroppedOutside(WinUI.TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            try
            {
                WebView tag = args.Tab.Tag as WebView;
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
                IncognitoTabsControl.TabItems.Remove(args.Tab);
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
                var uriNewWindow = new Uri(@"swiftlaunchincognito:" + tag.Source.ToString());
                await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
                await Task.Delay(1500);
                if (IncognitoTabsControl.TabItems.Count == 0)
                {
                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                    MenuFlyout Flyout = new MenuFlyout();
                    MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
                    OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);

                    ToolTip T = new ToolTip();
                    ToolTipService.SetToolTip(newTab, T);

                    OpenInnewwindow.Text = "Move to new window";
                    Flyout.Items.Add(OpenInnewwindow);
                    Flyout.Items.Add(new MenuFlyoutSeparator());
                    MenuFlyoutItem Refresh = new MenuFlyoutItem();
                    Refresh.Icon = new SymbolIcon(Symbol.Refresh);
                    Refresh.Text = "Refresh Tab";
                    Refresh.Click += Refresh_Click;
                    Flyout.Items.Add(Refresh);
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
                    CloseTab.Click += CloseTab_Click;
                    Flyout.Items.Add(CloseTab);
                    MenuFlyoutItem CloseO = new MenuFlyoutItem();
                    CloseO.Icon = new SymbolIcon(Symbol.Delete);
                    CloseO.Text = "Close other tabs";
                    CloseO.Click += CloseO_Click;
                    Flyout.Items.Add(CloseO);
                    MenuFlyoutItem CloseAll = new MenuFlyoutItem();
                    CloseAll.Icon = new SymbolIcon(Symbol.Delete);
                    CloseAll.Text = "Close all tabs";
                    CloseAll.Click += ClearAll;
                    Flyout.Items.Add(CloseAll);
                    newTab.RightTapped += NewTab_RightTapped;
                    newTab.PointerEntered += NewTab_PointerEntered;
                    newTab.ContextFlyout = Flyout;
                    newTab.Header = "Home Tab";

                    Frame frame = new Frame();
                    newTab.Content = frame;
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.MainTab = IncognitoTabsControl;
                    sender.TabItems.Add(newTab);
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    this.IncognitoTabsControl.SelectedItem = newTab;
                    WebViewPage.CurrentMainTab = newTab;
                    try
                    {
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void IncognitoTabsControl_TabDragStarting(WinUI.TabView sender, TabViewTabDragStartingEventArgs args)
        {
            try
            {
                WebView tag = args.Tab.Tag as WebView;
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
            }
            catch
            {
                return;
            }
        }

        private void RestoreTip_ActionButtonClick(TeachingTip sender, object args)
        {
            LoadTabViewRestore();
            sender.IsOpen = false;
        }

        private async void TitleGrid_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("IncognitoTabsControl");
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            newTab.Header = "Home Tab";
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);
            OpenInnewwindow.Text = "Move to new window";
            OpenInnewwindow.Click += OpenInnewwindow_Click1;
            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);
            newTab.ContextFlyout = Flyout;
            ToolTip T = new ToolTip();

            newTab.PointerEntered += NewTab_PointerEntered;
            newTab.RightTapped += NewTab_RightTapped;

            Frame frame = new Frame();
            WebViewPage.IncognitoModeStatic = true;
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            IncognitoTabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.IncognitoTabsControl;
            IncognitoTabsControl.SelectedItem = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;

            this.IncognitoTabsControl.SelectedItem = newTab;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            Window.Current.Activate();
            {
                GC.Collect();
                //    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                //    Window.Current.SetTitleBar(TitleGrid);
                StartUp();
                try
                {
                    if ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] != 3)
                    {
                        ShowRestoreDialog();
                    }
                }
                catch
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
                    ShowRestoreDialog();
                }
            }

            await Task.Delay(300);
            try
            {
                WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
                int w = (int)t.Width;
                try
                {
                    if (IncognitoTabsControl.TabItems.Count <= 7)
                    {
                        AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                    }
                    else
                    {
                        AppTitleBar.Width = 240;
                    }
                }
                catch
                {
                    AppTitleBar.Width = 240;
                }
            }
            catch
            {
                return;
            }


        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                WinUI.TabViewItem t = (WinUI.TabViewItem)IncognitoTabsControl.TabItems[0];
                int w = (int)t.Width;
                try
                {
                    if (IncognitoTabsControl.TabItems.Count <= 7)
                    {
                        AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * w);
                    }
                    else
                    {
                        AppTitleBar.Width = 240;
                    }
                }
                catch
                {
                    AppTitleBar.Width = 240;
                }
            }
            catch
            {
                return;
            }
        }

        private void IncognitoTabsControl_TabStripDrop_1(object sender, DragEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            MenuFlyout Flyout = new MenuFlyout();
            MenuFlyoutItem OpenInnewwindow = new MenuFlyoutItem();
            OpenInnewwindow.Icon = new SymbolIcon(Symbol.Add);

            ToolTip T = new ToolTip();
            ToolTipService.SetToolTip(newTab, T);

            OpenInnewwindow.Text = "Move to new window";
            Flyout.Items.Add(OpenInnewwindow);
            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem Refresh = new MenuFlyoutItem();
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Refresh.Text = "Refresh Tab";
            Refresh.Click += Refresh_Click;
            Flyout.Items.Add(Refresh);
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
            CloseTab.Click += CloseTab_Click;
            Flyout.Items.Add(CloseTab);
            MenuFlyoutItem CloseO = new MenuFlyoutItem();
            CloseO.Icon = new SymbolIcon(Symbol.Delete);
            CloseO.Text = "Close other tabs";
            CloseO.Click += CloseO_Click;
            Flyout.Items.Add(CloseO);
            MenuFlyoutItem CloseAll = new MenuFlyoutItem();
            CloseAll.Icon = new SymbolIcon(Symbol.Delete);
            CloseAll.Text = "Close all tabs";
            CloseAll.Click += ClearAll;
            Flyout.Items.Add(CloseAll);
            newTab.RightTapped += NewTab_RightTapped;
            newTab.PointerEntered += NewTab_PointerEntered;
            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";

            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = IncognitoTabsControl;
            IncognitoTabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.IncognitoTabsControl;
            this.IncognitoTabsControl.SelectedItem = newTab;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (IncognitoTabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
        }
    }
}
