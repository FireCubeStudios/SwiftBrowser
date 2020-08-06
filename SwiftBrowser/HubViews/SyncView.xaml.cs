using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Services.OneDrive;
using Microsoft.Toolkit.Services.Services.MicrosoftGraph;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SyncView : Page
    {
        public SyncView()
        {
            InitializeComponent();
            try
            {
                if ((bool) ApplicationData.Current.RoamingSettings.Values["Syncing"])
                    SyncToggle.IsOn = true;
                else
                    SyncToggle.IsOn = false;
            }
            catch
            {
                ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                ApplicationData.Current.RoamingSettings.Values["SyncId"] =
                    ApplicationData.Current.LocalSettings.Values["SyncId"];
                SyncToggle.IsOn = false;
            }
        }

        [Obsolete]
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string[] scopes = {MicrosoftGraphScope.FilesReadWriteAll};
            OneDriveService.Instance.Initialize
            ("bcca3da5 - 059d - 48be - bd90 - 0f09da4660c0",
                scopes);
            try
            {
                if (!await OneDriveService.Instance.LoginAsync()) throw new Exception("Unable to sign in");
            }
            catch
            {
            }
        }

        private void SyncToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (SyncToggle.IsOn)
            {
                ApplicationData.Current.RoamingSettings.Values["Syncing"] = true;
                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                ApplicationData.Current.RoamingSettings.Values["SyncId"] =
                    ApplicationData.Current.LocalSettings.Values["SyncId"];
            }
            else
            {
                ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
            }
        }
    }
}
