using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class HomePageSettings : Page
    {
        public HomePageSettings()
        {
            this.InitializeComponent();
            try
            {
                tICO.IsOn = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                TfAV.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"];
                TqUI.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"];
                TmOR.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"];
                TSea.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"];
            }
            catch
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
                tICO.IsOn = (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                TfAV.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"];
                TqUI.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"];
                TmOR.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"];
                TSea.IsOn = (Boolean)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"];
            }
            if ((bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] == true)
            {

                Option2RadioButton.IsChecked = true;

            }
            else
            {
                Option1RadioButton.IsChecked = true;

            }
        }
        private void TICO_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = toggle.IsOn;

        }

        private void TfAV_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = toggle.IsOn;

        }

        private void TqUI_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = toggle.IsOn;

        }

        private void TmOR_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = toggle.IsOn;


        }

        private void TSea_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = toggle.IsOn;

        }

        private void Option2RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Visible;
        }

        private void Option1RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Collapsed;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
        }

        private async void TextUrl_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(sender.Text.StartsWith("Https://") == true){


                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = args.QueryText;
                    }
            else
            {
                var m = new MessageDialog("Not valid url");
                await m.ShowAsync();
            }
        }
    }
}
