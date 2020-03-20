using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SwiftBrowser.Models;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
      public static TabView TabviewPageControl { get; set; }

        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public bool IncognitoMode;
        TabViewItem RightClickedItem;
        public static TabViewPage SingletonReference { get; set; }
        public TabViewPage()
        {
            InitializeComponent();
            /* Flyout.Items.Add(new MenuFlyoutItem()
{
// Icon = new FontIcon() { Glyph = "\uEC4F" },
Icon 
Text = "Move to new window"

});*/
            /* Flyout.Items.Add(new MenuFlyoutItem()
            {
                Icon = new SymbolIcon(Symbol.Add),
                Text = "New Tab"
            });*/
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
            Flyout.Items.Add(OpenInnewwindow);

            Flyout.Items.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem newtabF= new MenuFlyoutItem();
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
            MenuFlyout Flyout = new MenuFlyout();
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
    }
}
