using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebSettings : Page
    {
        public WebSettings()
        {
            this.InitializeComponent();
            I.IsOn = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["IndexDB"];
            E.IsOn = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["Javascript"];
            switch ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"])
            {
                case "https://www.ecosia.org/search?q=":
                    se.SelectedIndex = 0;
                        break;
                case "https://www.google.com/search?q=":
                    se.SelectedIndex = 2;
                    break;
                case "https://www.bing.com/search?q=":
                    se.SelectedIndex = 3;
                    break;
                case "http://www.baidu.com/s?wd=":
                    se.SelectedIndex = 5;
                    break;
                case "https://www.yandex.com/search/?text=":
                    se.SelectedIndex = 4;
                    break;
                case "https://duckduckgo.com/?q=":
                    se.SelectedIndex = 6;
                    break;
                case "https://search.yahoo.com/search?p=":
                    se.SelectedIndex = 7;
                    break;
                case "https://en.wikipedia.org/w/index.php?search=":
                    se.SelectedIndex = 1;
                    break;
            }
        }

        private void I_Toggled(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["IndexDB"] = I.IsOn;
        }

        private void E_Toggled(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Javascript"] = E.IsOn;
        }

        private void Se_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            switch (c.SelectedItem.ToString())
            {
                case "Ecosia":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.ecosia.org/search?q=";
                    break;
                case "Google":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.google.com/search?q=";
                    break;
                case "Bing":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.bing.com/search?q=";
                    break;
                case "Baidu":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "http://www.baidu.com/s?wd=";
                    break;
                case "DuckDuckGo":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://duckduckgo.com/?q=";
                    break;
                case "Yandex":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.yandex.com/search/?text=";
                    break;
                case "Yahoo":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://search.yahoo.com/search?p=";
                    break;
                case "Wikipedia":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://en.wikipedia.org/w/index.php?search=";
                    break;
            }
        }
    }
}
