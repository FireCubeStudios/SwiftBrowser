using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SwiftBrowser.Models;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
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
        public bool IncognitoMode;
        TabViewItem RightClickedItem;
        public static TabViewPage SingletonReference { get; set; }
        public TabViewPage()
        {
            InitializeComponent();
            SingletonReference = this;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            Window.Current.SetTitleBar(CustomDragRegion);
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
            // The Content of a TabViewItem is often a frame which hosts a page.
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
        
            GC.Collect();
             CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
             Window.Current.SetTitleBar(TitleGrid);
        }

        private void Fav_Click(object sender, RoutedEventArgs e)
        {
            WebView FavW = RightClickedItem.Tag as WebView;

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            WebView FavW = RightClickedItem.Tag as WebView;
            FavW.Refresh();
        }

        private async void OpenInnewwindow_Click1(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            TabsControl.TabItems.Remove(RightClickedItem);
            WebView tag = RightClickedItem.Tag as WebView;
            localSettings.Values["SourceToGo"] = tag.Source.ToString();
            // TabsControl.TabItems.Remove(args.Tab);
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

        private void NewTab_PointerExited(object sender, PointerRoutedEventArgs e)
        {
           // PreviewFlyout.Hide();
        }

        private async void NewTab_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TabViewItem Tab = sender as TabViewItem;
            ToolTip T = ToolTipService.GetToolTip(Tab) as ToolTip;
            try { 
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
        

        private async void OpenInnewwindow_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;



            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TabViewItem t = new TabViewItem();
                t = RightClickedItem;
                //TabViewPage.TabToAdd = t;
                Frame frame = new Frame();
                frame.Navigate(typeof(TabViewPage));
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
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
                    // The Content of a TabViewItem is often a frame which hosts a page.
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
                    TabsControl.TabItems.Remove(RightClickedItem);
                    GC.Collect();
                }
                else
                {
                    TabsControl.TabItems.Remove(RightClickedItem);
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
            // The Content of a TabViewItem is often a frame which hosts a page.
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
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
            // The Content of a TabViewItem is often a frame which hosts a page.
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = false;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.TabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = false;
            TabsControl.TabItems.Add(newTab);
            TabsControl.SelectedItem = newTab;
            GC.Collect();
        }
        private async void Incognito_Click(object sender, RoutedEventArgs e)
        {
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
            newTab.ContextFlyout = Flyout;// The Content of a TabViewItem is often a frame which hosts a page.
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
                    // Select the last tab
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
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
            // The Content of a TabViewItem is often a frame which hosts a page.
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
            sender.SelectedItem = newTab;
           // WebViewPage.CurrentMainTab = TabsControl.SelectedItem as TabViewItem;
            GC.Collect();
        }

        private async void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
                    // The Content of a TabViewItem is often a frame which hosts a page.
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
                    sender.TabItems.Remove(args.Tab);
                    GC.Collect();
                }
                else
                {
                    sender.TabItems.Remove(args.Tab);
                    GC.Collect();
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
            // The Content of a TabViewItem is often a frame which hosts a page.
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
        }

        private async void TabsControl_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {

                CoreApplicationView newView = CoreApplication.CreateNewView();
                int newViewId = 0;
         TabsControl.TabItems.Remove(args.Tab);
            WebView tag = args.Tab.Tag as WebView;
            localSettings.Values["SourceToGo"] = tag.Source.ToString();
            // TabsControl.TabItems.Remove(args.Tab);
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

        private void TabsControl_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            WebView tag = args.Tab.Tag as WebView;
            localSettings.Values["SourceToGo"] = tag.Source.ToString();
        }
    }

    
}
