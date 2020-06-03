using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using SwiftBrowser.Assets;
using SwiftBrowser.Models;
using Windows.ApplicationModel.Core;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace SwiftBrowser.Views
{
    // For more info about the TabView Control see
    // https://docs.microsoft.com/uwp/api/microsoft.ui.xaml.controls.tabview?view=winui-2.2
    // For other samples, get the XAML Controls Gallery app http://aka.ms/XamlControlsGallery
    public sealed partial class TabViewPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<TabViewItemData> Tabs { get; } = new ObservableCollection<TabViewItemData>()
        {

        };
        //  ToolTip T = new ToolTip();
        public static TabView TabviewPageControl { get; set; }
        Flyout PreviewFlyout = new Flyout();
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public bool IncognitoMode;
        TabViewItem RightClickedItem;
        public static Grid titlebar { get; set; }
        public static Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification InAppNotificationMain { get; set; }
        public static TabViewPage SingletonReference { get; set; }
        public TabViewPage()
        {
            InitializeComponent();
            InAppNotificationMain = UniversalNormalNotificationInApp;
            SingletonReference = this;
            titlebar = AppTitleBar;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            //    coreTitleBar.LayoutMetricsChanged += CustomDragRegion_SizeChanged;

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
                            var newTab = new TabViewItem();
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
                            WebViewPage.IncognitoModeStatic = false;
                            newTab.Content = frame;
                            WebViewPage.IncognitoModeStatic = false;
                            WebViewPage.CurrentMainTab = newTab;
                            WebViewPage.MainTab = this.TabsControl;
                            frame.Navigate(typeof(WebViewPage));
                            WebViewPage.IncognitoModeStatic = false;
                            WebViewPage.MainTab = this.TabsControl;
                            TabsControl.TabItems.Add(newTab);
                            WebViewPage.MainTab = this.TabsControl;
                            TabsControl.SelectedItem = newTab;
                            WebViewPage.MainTab = this.TabsControl;
                            try
                            {
                                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
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
                var newTab = new TabViewItem();
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
                WebViewPage.IncognitoModeStatic = false;
                newTab.Content = frame;
                WebViewPage.IncognitoModeStatic = false;
                WebViewPage.CurrentMainTab = newTab;
                try
                {
                    AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
                }
                catch
                {
                    AppTitleBar.Width = 240;
                }
                WebViewPage.MainTab = this.TabsControl;
                frame.Navigate(typeof(WebViewPage));
                WebViewPage.IncognitoModeStatic = false;
                WebViewPage.MainTab = this.TabsControl;
                TabsControl.TabItems.Add(newTab);
                WebViewPage.MainTab = this.TabsControl;
                TabsControl.SelectedItem = newTab;
                WebViewPage.MainTab = this.TabsControl;
            }
            /* }
              catch
              {
                  return;
              }*/
        }
        public class TabsClass
        {
            public List<TabsJSON> Tabs { get; set; }
        }
        public class TabsJSON
        {
            public string TabViewItemJSON { get; set; }
        }
        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            /* List<TabsJSON> TabsList = new List<TabsJSON>();
            foreach (var TabviewItems in TabsControl.TabItems)
             {
                TabViewItem TabListTab = TabviewItems as TabViewItem;
                 WebView W = TabListTab.Tag as WebView;
                 TabsList.Add(new TabsJSON()
                 {
                     TabViewItemJSON = "W",
                 });
             }
           ;*/
            string filepath = @"Assets\RestoreTabItems.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath);
            String Text = await FileIO.ReadTextAsync(file);
            TabsClass Tabslist = JsonConvert.DeserializeObject<TabsClass>(Text);
            //  List<TabsClass> Tabslist = new List<TabsClass>();
            foreach (TabViewItem TabviewItems in TabsControl.TabItems)
            {
                WebView tag = TabviewItems.Tag as WebView;
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
            TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
            int w = (int)t.Width;
            try
            {
                if (TabsControl.TabItems.Count <= 7)
                {
                    AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
            TabsControl.TabItems.Remove(RightClickedItem);
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
            TabViewItem Tab = sender as TabViewItem;
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
            TabsControl.TabItems.Clear();
            TabsControl.TabItems.Add(RightClickedItem);
        }

        private async void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (TabsControl.TabItems.Count <= 1)
                {
                    var newTab = new TabViewItem();
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
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.MainTab = this.TabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.MainTab = TabsControl;
                    TabsControl.TabItems.Add(newTab);
                    try
                    {
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
                    }
                    catch
                    {
                        AppTitleBar.Width = 240;
                    }
                    WebViewPage.MainTab = this.TabsControl;
                    TabsControl.SelectedItem = newTab;
                    RightClickedItem.Content = null;
                    TabsControl.TabItems.Remove(RightClickedItem);
                    TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (TabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
                    TabsControl.TabItems.Remove(RightClickedItem);
                    TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (TabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
            RightClickedItem = sender as TabViewItem;
        }

        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            var uriNewWindow = new Uri(@"swiftlaunch:");
            localSettings.Values["SourceToGo"] = null;
            await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
        }
        public void AddAll(object sender, RoutedEventArgs e)
        {
            var newTab = new TabViewItem();
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

            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.MainTab = this.TabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = false;
            this.TabsControl.TabItems.Add(newTab);
            this.TabsControl.SelectedItem = newTab;
            GC.Collect();
        }
        public void ClearAll(object sender, RoutedEventArgs e)
        {
            TabsControl.TabItems.Clear();
            var newTab = new TabViewItem();
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
            WebViewPage.MainTab = this.TabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = false;
            TabsControl.TabItems.Add(newTab);
            TabsControl.SelectedItem = newTab;
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
            var senderTabView = args.Element as TabView;
            var newTab = new TabViewItem();
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
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.MainTab = TabsControl;
            TabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.TabsControl;
            TabsControl.SelectedItem = newTab;
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

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
            var newTab = new TabViewItem();
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
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.MainTab = TabsControl;
            sender.TabItems.Add(newTab);
            WebViewPage.MainTab = this.TabsControl;
            this.TabsControl.SelectedItem = newTab;
            WebViewPage.CurrentMainTab = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
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
                    var newTab = new TabViewItem();
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
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.CurrentMainTab = newTab;

                    WebViewPage.MainTab = this.TabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.MainTab = TabsControl;
                    sender.TabItems.Add(newTab);
                    WebViewPage.MainTab = this.TabsControl;
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
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
                    TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
                    int w = (int)t.Width;
                    try
                    {
                        if (TabsControl.TabItems.Count <= 7)
                        {
                            AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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




        private void TabsControl_TabStripDrop(object sender, DragEventArgs e)
        {
            var newTab = new TabViewItem();
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
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.MainTab = TabsControl;
            TabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.TabsControl;
            TabsControl.SelectedItem = newTab;
            try
            {
                AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
            }
            catch
            {
                AppTitleBar.Width = 240;
            }
        }

        private async void TabsControl_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
           try
           {
                WebView tag = args.Tab.Tag as WebView;
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
            TabsControl.TabItems.Remove(args.Tab);
                localSettings.Values["SourceToGo"] = tag.Source.ToString();
                var uriNewWindow = new Uri(@"swiftlaunch:" + tag.Source.ToString());
                await Windows.System.Launcher.LaunchUriAsync(uriNewWindow);
                await Task.Delay(1500);
                if (TabsControl.TabItems.Count == 0)
                {
                    var newTab = new TabViewItem();
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
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.MainTab = this.TabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.IncognitoModeStatic = false;
                    WebViewPage.MainTab = TabsControl;
                    sender.TabItems.Add(newTab);
                    WebViewPage.MainTab = this.TabsControl;
                    this.TabsControl.SelectedItem = newTab;
                    WebViewPage.CurrentMainTab = newTab;
                    try
                    {
                        AppTitleBar.Width = AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * newTab.Width);
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

        private void TabsControl_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
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
            FindName("TabsControl");
            var newTab = new TabViewItem();
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
            WebViewPage.IncognitoModeStatic = false;
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.MainTab = this.TabsControl;
            TabsControl.TabItems.Add(newTab);
            WebViewPage.MainTab = this.TabsControl;
            TabsControl.SelectedItem = newTab;
            WebViewPage.MainTab = this.TabsControl;

            this.TabsControl.SelectedItem = newTab;
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
                TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
                int w = (int)t.Width;
                try
                {
                    if (TabsControl.TabItems.Count <= 7)
                    {
                        AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
                TabViewItem t = (TabViewItem)TabsControl.TabItems[0];
                int w = (int)t.Width;
                try
                {
                    if (TabsControl.TabItems.Count <= 7)
                    {
                        AppTitleBar.Width = (Window.Current.Bounds.Width - 60) - (TabsControl.TabItems.Count * w);
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
    }
}
