using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Services.Store.Engagement;
using SwiftBrowser.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private string _versionDescription;

        public AboutPage()
        {
            InitializeComponent();
        }

        public string VersionDescription
        {
            get => _versionDescription;

            set => Set(ref _versionDescription, value);
        }

        private async void FeedbackLink_Click(object sender, RoutedEventArgs e)
        {
            // This launcher is part of the Store Services SDK https://docs.microsoft.com/windows/uwp/monetize/microsoft-store-services-sdk
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalizedSwift();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void Rate_Click(object sender, RoutedEventArgs e)

        {
            await Launcher.LaunchUriAsync(
                new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }

        private async void PinAppToTaskbar_Click(object sender, RoutedEventArgs e)

        {
            var isPinningAllowed = TaskbarManager.GetDefault().IsPinningAllowed;

            if (isPinningAllowed)

            {
                if (ApiInformation.IsTypePresent("Windows.UI.Shell.TaskbarManager"))

                {
                    var isPinned = await TaskbarManager.GetDefault().IsCurrentAppPinnedAsync();


                    if (isPinned)

                    {
                        await new MessageDialog("If not you can manually pin the app to the taskbar",
                            "You already have the app pinned in your taskbar").ShowAsync();
                    }

                    else

                    {
                        var IsPinned = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                    }
                }


                else

                {
                    await new MessageDialog("Update your device to the Fall creators update or higher to pin this app",
                        "Update your device").ShowAsync();
                }
            }


            else

            {
                var t = AnalyticsInfo.VersionInfo.DeviceFamily;

                switch (t)

                {
                    case "Windows.Desktop":

                        await new MessageDialog(
                            "It seems you are using a computer. Group policy disabled pinning of app in taskbar",
                            "Taskbar pin failed").ShowAsync();

                        break;

                    case "Windows.Mobile":

                        await new MessageDialog(
                            "It seems you are using a Windows 10 on ARM device or mobile device. Group policy disabled pinning of the app",
                            "Taskbar pin failed").ShowAsync();

                        break;

                    case "Windows.IoT":

                        await new MessageDialog(
                            "It seems you are using a IoT device which doesn't support taskbar pin API",
                            "Taskbar pin failed").ShowAsync();

                        break;

                    case "Windows.Team":

                        break;

                    case "Windows.Holographic":

                        await new MessageDialog("It seems you are using hololens. Hololens doesn't have a taskbar",
                            "Taskbar pin failed").ShowAsync();

                        break;

                    case "Windows.Xbox":

                        await new MessageDialog("It seems you are using a xbox. Xbox doesn't have a taskbar",
                            "Taskbar pin failed").ShowAsync();

                        break;

                    default:

                        await new MessageDialog(
                            "It seems you are using a " + t +
                            " device. This device does not support taskbar API or Group policy disabled pinning of the app",
                            "Taskbar pin failed").ShowAsync();

                        break;
                }
            }
        }

        private async void LivePin(object sender, RoutedEventArgs e)

        {
            // Get your own app list entry

            // (which is always the first app list entry assuming you are not a multi-app package)

            var entry = (await Package.Current.GetAppListEntriesAsync())[0];


            // Check if Start supports your app

            var isSupported = StartScreenManager.GetDefault().SupportsAppListEntry(entry);

            if (isSupported)

            {
                if (ApiInformation.IsTypePresent("Windows.UI.StartScreen.StartScreenManager"))

                {
                    // Primary tile API's supported!

                    var isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);

                    if (isPinned)

                    {
                        await new MessageDialog("If not you can manually put the live tile on to the StartScreen",
                            "You already have the live tile in your StartScreen").ShowAsync();
                    }

                    else

                    {
                        var IsPinned = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
                    }
                }

                else

                {
                    await new MessageDialog("You need to update your device to enable automatic pinning",
                        "Update your device").ShowAsync();
                }
            }

            else

            {
                var t = AnalyticsInfo.VersionInfo.DeviceFamily;

                switch (t)

                {
                    case "Windows.IoT":

                        await new MessageDialog(
                            "It seems you are using a IoT device which doesn't support Primary tile API",
                            "live tile failed").ShowAsync();

                        break;

                    case "Windows.Team":

                        break;

                    case "Windows.Holographic":

                        await new MessageDialog("It seems you are using hololens. Hololens doesn't support live tile",
                            "live tile failed").ShowAsync();

                        break;

                    case "Windows.Xbox":

                        await new MessageDialog("It seems you are using a xbox. Xbox doesn't support live tile",
                            "live tile failed").ShowAsync();

                        break;

                    default:

                        await new MessageDialog(
                            "It seems you are using a " + t + " device. This device does not support Primary tile API",
                            "live tile failed").ShowAsync();

                        break;
                }
            }
        }
    }
}
