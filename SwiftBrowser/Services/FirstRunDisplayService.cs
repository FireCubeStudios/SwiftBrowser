using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SwiftBrowser.Assets;
using SwiftBrowser.Views;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace SwiftBrowser.Services
{
    public static class FirstRunDisplayService
    {
        public class SyncClass
        {
            public List<SyncJSON> Sync { get; set; }
        }

        /*  public class Sync
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class SyncJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }
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
                    }
                });
        }
    }
}
