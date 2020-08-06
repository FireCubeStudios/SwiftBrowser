using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SwiftBrowser.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResetSettings : Page
    {
        public ResetSettings()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
            ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
            ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
            ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
            ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
            ApplicationData.Current.LocalSettings.Values["DarkMode"] = false;
            ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePathBool"] = false;
            ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"] = "";
            ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = false;
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
            ApplicationData.Current.LocalSettings.Values["AdBlocker"] = false;
            ApplicationData.Current.LocalSettings.Values["IndexDB"] = true;
            ApplicationData.Current.LocalSettings.Values["Javascript"] = true;
            ApplicationData.Current.LocalSettings.Values["StoreHistory"] = true;
            ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.ecosia.org/search?q=";
            ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
            ApplicationData.Current.LocalSettings.Values["UserAgent"] = "Default";
            ApplicationData.Current.LocalSettings.Values["WebNotifications"] = 0;
            ApplicationData.Current.LocalSettings.Values["IDBUnlimitedPermision"] = 0;
            ApplicationData.Current.LocalSettings.Values["Media"] = 0;
            ApplicationData.Current.LocalSettings.Values["Screen"] = 0;
            ApplicationData.Current.LocalSettings.Values["WebVR"] = 0;
            ApplicationData.Current.LocalSettings.Values["Geolocation"] = 0;
            ApplicationData.Current.LocalSettings.Values["PointerLock"] = 0;
            var rnd = new Random();
            ApplicationData.Current.LocalSettings.Values["SyncId"] = rnd.Next().ToString();
            var filepath = @"Assets\Extensions.json";
            var folder = Package.Current.InstalledLocation;
            var f = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(filepath);
            var sfile = await f.CreateFileAsync("Extensions.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sfile, await FileIO.ReadTextAsync(file));
            var duration = 3000;
            try
            {
                TabViewPage.InAppNotificationMain.Show("All local settings have been reset", duration);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show("All local settings have been reset", duration);
            }
        }
    }
}
