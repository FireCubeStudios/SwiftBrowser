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
    public sealed partial class StartupSettings : Page
    {
        public StartupSettings()
        {
            this.InitializeComponent();
            if ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 1)
            {
                Option3RadioButton.IsChecked = true;
            }
            else if((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 2)
                    {
                Option2RadioButton.IsChecked = true;
            }
            else
            {
                Option1RadioButton.IsChecked = true;
            }
        }

        private void Option1RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 3;
        }

        private void Option2RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
        }

        private void Option3RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 1;
        }
    }
}
