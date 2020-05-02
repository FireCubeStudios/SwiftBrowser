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

namespace SwiftBrowser.Extensions.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DarkmodeSwiftBrowserBuiltInExtension : Page
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        DarkMode DarkMode = new DarkMode();
        public static WebView w { get; set; }
        public DarkmodeSwiftBrowserBuiltInExtension()
        {
            this.InitializeComponent();
            if ((bool)localSettings.Values["DarkMode"] == true)
            {
                Tog.IsOn = true;
            }
            else
                {
                Tog.IsOn = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DarkMode.DarkMode_Click(w);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch T = sender as ToggleSwitch;
            if(T.IsOn == true)
            {
                DarkMode.DarkMode_Click(w);
                localSettings.Values["DarkMode"] = true;
            }
            else
            {
                localSettings.Values["DarkMode"] = false;
            }
        }
    }
}
