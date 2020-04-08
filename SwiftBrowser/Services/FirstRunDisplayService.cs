using System;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Helpers;

using SwiftBrowser.Views;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SwiftBrowser.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;
        internal static async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    if (SystemInformation.IsFirstRun && !shown)
                    {
                        shown = true;
                        var dialog = new FirstRunDialog();
                        await dialog.ShowAsync();
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
                    }
                });
        }
    }
}
