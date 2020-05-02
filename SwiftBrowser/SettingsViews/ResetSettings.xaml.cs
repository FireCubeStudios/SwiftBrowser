using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class ResetSettings : Page
    {
        public ResetSettings()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["DarkMode"] = false;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePathBool"] = false;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"] = "";
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = false;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["AdBlocker"] = false;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["IndexDB"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Javascript"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["StoreHistory"] = true;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.ecosia.org/search?q=";
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
            Random rnd = new Random();
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"] = rnd.Next().ToString();
            string filepath = @"Assets\Extensions.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder f = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(filepath);
            StorageFile sfile = await f.CreateFileAsync("Extensions.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sfile, await Windows.Storage.FileIO.ReadTextAsync(file));
            var m = new MessageDialog("All local settings reset");
                await m.ShowAsync();
        }
    }
}
