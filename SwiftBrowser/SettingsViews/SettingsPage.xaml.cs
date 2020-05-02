using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SwiftBrowser.Helpers;
using SwiftBrowser.Services;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SwiftBrowser.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        ExtensionsClass ExtensionsListJSONJSON;
        public static ExtensionsStore methods { get; set; }
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

   
        public SettingsPage()
        {
            InitializeComponent();
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

            LoadItems();
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
        public async void LoadItems()
        {
            List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("Extensions.json"); // error here
            var JSONData = "e";
            JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
            ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
            foreach (var item in ExtensionsListJSONJSON.Extensions)
            {
                if (item.IsEnabledJSON == true)
                {
                    ExtensionsListJSON.Add(new ExtensionsJSON()
                    {
                        NameJSON = item.NameJSON,
                        DescriptionJSON = item.DescriptionJSON,
                        IconJSON = item.IconJSON,
                        IsEnabledJSON = item.IsEnabledJSON,
                        Page = item.Page,
                        IsToolbar = item.IsToolbar
                    });
                }
            }
            ExtensionsList.ItemsSource = ExtensionsListJSON;
        }
        public async void closing(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("Extensions.json");
            var SerializedObject = JsonConvert.SerializeObject(ExtensionsListJSONJSON, Formatting.Indented);

            await Windows.Storage.FileIO.WriteTextAsync(file, SerializedObject);

        }

        private void ToggleToolbar_Toggled(object sender, RoutedEventArgs e)
        {
            List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
            //   LoadingControl.IsLoading = true;
            ToggleSwitch t = sender as ToggleSwitch;
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            ExtensionsJSON SenderPost = DataContext as ExtensionsJSON;
            ExtensionsJSON FoundItem = ExtensionsListJSONJSON.Extensions.Find(x => SenderPost.Id == SenderPost.Id);
            FoundItem.IsToolbar = t.IsOn;
            SenderPost.IsToolbar = t.IsOn;
            FoundItem.IsEnabledJSON = true;
            ExtensionsListJSONJSON.Extensions.Remove(FoundItem);
            ExtensionsListJSONJSON.Extensions.Add(FoundItem);

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
            if (sender.Text.StartsWith("Https://") == true)
            {


                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrl"] = args.QueryText;
            }
            else
            {
                var m = new MessageDialog("Not valid url");
                await m.ShowAsync();
            }
        }


        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
            }
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
