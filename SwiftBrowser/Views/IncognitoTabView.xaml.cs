using Microsoft.Toolkit.Uwp.UI.Controls;
using SwiftBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        public ObservableCollection<TabViewItemData> Tabs { get; } = new ObservableCollection<TabViewItemData>()
        {

        };
        public static WinUI.TabView TabviewPageControl { get; set; }

        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public bool IncognitoMode;

        public static IncognitoTabView SingletonReference { get; set; }
        public IncognitoTabView()
        {
            this.InitializeComponent();
            SingletonReference = this;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            Window.Current.SetTitleBar(CustomDragRegion);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(TitleGrid);
            FindName("TabsControl");
            TabviewPageControl = IncognitoTabsControl;
            var newTab = new WinUI.TabViewItem();
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

            newTab.ContextFlyout = Flyout;
            newTab.IconSource= new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            newTab.Header = "Home Tab";
            // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            IncognitoTabsControl.TabItems.Add(newTab);

            // CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            // Window.Current.SetTitleBar(TitleGrid);
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
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
            newTab.Header = "Home Tab";         // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            IncognitoTabsControl.TabItems.Add(newTab);
            args.Handled = true;
        }



        private void OnAddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
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

            newTab.ContextFlyout = Flyout;
            // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            sender.TabItems.Add(newTab);
            sender.SelectedItem = newTab;
            GC.Collect();
        }
        private async void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.TabItems.Count == 1)
                {
                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Home };
                    newTab.Header ="Home Tab";
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

                    newTab.ContextFlyout = Flyout;
                    // The Content of a TabViewItem is often a frame which hosts a page.
                    Frame frame = new Frame();
                    newTab.Content = frame;
                    WebViewPage.IncognitoModeStatic = true;
                    WebViewPage.CurrentMainTab = newTab;
                    WebViewPage.MainTab = this.IncognitoTabsControl;
                    frame.Navigate(typeof(WebViewPage));
                    sender.TabItems.Add(newTab);
                    sender.SelectedItem = newTab;
                    sender.TabItems.Remove(args.Tab);
                    GC.Collect(2);
                }
                else
                {
                    sender.TabItems.Remove(args.Tab);
                    GC.Collect(2);
                }
            });
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
            var newTab = new WinUI.TabViewItem();
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

            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";
            // The Content of a TabViewItem is often a frame which hosts a page.
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            IncognitoTabsControl.TabItems.Add(newTab);
            IncognitoTabsControl.SelectedItem = newTab;
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

            newTab.ContextFlyout = Flyout;
            newTab.Header = "Home Tab";
            // The Content of a TabViewItem is often a frame which hosts a page.
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            Frame frame = new Frame();
            newTab.Content = frame;
            WebViewPage.IncognitoModeStatic = true;
            WebViewPage.CurrentMainTab = newTab;
            WebViewPage.MainTab = this.IncognitoTabsControl;
            frame.Navigate(typeof(WebViewPage));
            WebViewPage.IncognitoModeStatic = true;
            IncognitoTabsControl.TabItems.Add(newTab);
            IncognitoTabsControl.SelectedItem = newTab;
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
