using Microsoft.Toolkit.Services.Services.MicrosoftGraph;
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

namespace SwiftBrowser.HubViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SyncView : Page
    {
        public SyncView()
        {
            this.InitializeComponent();
            try
            {
                if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] == true)
                {
                    SyncToggle.IsOn = true;
                }
                else
                {
                    SyncToggle.IsOn = false;
                }
            }
            catch
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                SyncToggle.IsOn = false;
            }
        }

        [Obsolete]
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string[] scopes = new string[] { MicrosoftGraphScope.FilesReadWriteAll };
            Microsoft.Toolkit.Services.OneDrive.OneDriveService.Instance.Initialize
    ("bcca3da5 - 059d - 48be - bd90 - 0f09da4660c0",
    scopes,
     null,
     null);
            try
            {
                if (!await Microsoft.Toolkit.Services.OneDrive.OneDriveService.Instance.LoginAsync())
                {
                    throw new Exception("Unable to sign in");
                }
            }
            catch
            {

            }
        }

        private void SyncToggle_Toggled(object sender, RoutedEventArgs e)
        {
          if(SyncToggle.IsOn == true)
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = true;
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
            }
            else
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
            }
        }
    }
}
