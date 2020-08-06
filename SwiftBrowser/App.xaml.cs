using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SwiftBrowser.Assets;
using SwiftBrowser.Services;
using SwiftBrowser.Views;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace SwiftBrowser
{
    public sealed partial class App : Application
    {
        private readonly Lazy<ActivationService> _activationService;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private string protocolargs;

        public App()
        {
            InitializeComponent();
            Current.UnhandledException += OnUnhandledException;
            DataAccess.InitializeDatabase();
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        private ActivationService ActivationService => _activationService.Value;

        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            unhandledExceptionEventArgs.Handled = true;
            /* ErrorDialog E = new ErrorDialog();
             ErrorDialog.errormsgStatic = unhandledExceptionEventArgs.Message;
             ErrorDialog.errormsgExeptionStatic = unhandledExceptionEventArgs.Exception.ToString();
                await E.ShowAsync();*/
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (SystemInformation.Instance.IsFirstRun)
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
                ApplicationData.Current.LocalSettings.Values["WebNotifications"] = 0;
                ApplicationData.Current.LocalSettings.Values["IDBUnlimitedPermision"] = 0;
                ApplicationData.Current.LocalSettings.Values["Media"] = 0;
                ApplicationData.Current.LocalSettings.Values["Screen"] = 0;
                ApplicationData.Current.LocalSettings.Values["WebVR"] = 0;
                ApplicationData.Current.LocalSettings.Values["Geolocation"] = 0;
                ApplicationData.Current.LocalSettings.Values["PointerLock"] = 0;
                ApplicationData.Current.LocalSettings.Values["UserAgent"] = "Default";

                var rnd = new Random();
                ApplicationData.Current.LocalSettings.Values["SyncId"] = rnd.Next().ToString();
                var filepath = @"Assets\Extensions.json";
                var folder = Package.Current.InstalledLocation;
                var f = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(filepath);
                var sfile = await f.CreateFileAsync("Extensions.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sfile, await FileIO.ReadTextAsync(file));
            }

            var idOfTappedTile = args.Arguments;
            if (idOfTappedTile.Contains("http"))
                localSettings.Values["SourceToGo"] = idOfTappedTile;
            else
                localSettings.Values["SourceToGo"] = null;
            await ConfigureJumpList();
            var canEnablePrelaunch =
                ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
            if (args.PrelaunchActivated == false)
            {
                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                if (canEnablePrelaunch) TryEnablePrelaunch();
                await ActivationService.ActivateAsync(args);
            }
            else
            {
                if (!args.PrelaunchActivated) await ActivationService.ActivateAsync(args);
            }
        }

        private void TryEnablePrelaunch()
        {
            CoreApplication.EnablePrelaunch(true);
        }

        private async Task ConfigureJumpList()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            // using the favorites class
            // TODO: change name of "favorites" class to something else
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            // Disable the system-managed jump list group.
            jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

            try
            {
                var sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    var itemj = JumpListItem.CreateWithArguments("item.UrlJSON.ToString()",
                        "item.HeaderJSON.ToString()");

                    itemj.Description = "item.UrlJSON";

                    itemj.GroupName = "Quick pinned: ";


                    itemj.Logo = new Uri("ms-appx:///Assets/SmallTile.scale-100.png");


                    jumpList.Items.Add(itemj);
                }
            }
            catch
            {
                var JSONData = "e";
                var filepath = @"Assets\QuickPinWeb.json";
                var folder = Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(filepath);
                JSONData = await FileIO.ReadTextAsync(file);
                //  StorageFile sfile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                //  await FileIO.WriteTextAsync(sfile, JSONData);
                var sampleFile =
                    await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONData);
                var FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    var itemj = JumpListItem.CreateWithArguments(item.UrlJSON, item.HeaderJSON);

                    itemj.Description = "item.UrlJSON";

                    itemj.GroupName = "Quick pinned: ";

                    itemj.Logo = new Uri("ms-appx:///Assets/SmallTile.scale-100.png");


                    jumpList.Items.Add(itemj);
                }

                await jumpList.SaveAsync();
            }
        }

        private void Widget1Window_Closed(object sender, CoreWindowEventArgs e)

        {
            Window.Current.Closed -= Widget1Window_Closed;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                var eventArgs = args as ProtocolActivatedEventArgs;
                if (string.IsNullOrEmpty(eventArgs.Uri.AbsoluteUri) == false)
                {
                    if (eventArgs.Uri.AbsoluteUri.Contains("swiftlaunch:") && eventArgs.Uri.AbsoluteUri.Length > 12)
                    {
                        var x = eventArgs.Uri.AbsoluteUri.Remove(count: 12, startIndex: 0);
                        localSettings.Values["SourceToGo"] = x;
                        protocolargs = "normal";
                    }
                    else if (eventArgs.Uri.AbsoluteUri.Contains("swiftlaunch:"))
                    {
                        var x = eventArgs.Uri.AbsoluteUri.Remove(count: 12, startIndex: 0);
                        localSettings.Values["SourceToGo"] = null;
                        protocolargs = "normal";
                    }
                    else if (eventArgs.Uri.AbsoluteUri.Contains("swiftlaunchincognito:") &&
                             eventArgs.Uri.AbsoluteUri.Length > 21)
                    {
                        var x = eventArgs.Uri.AbsoluteUri.Remove(count: 21, startIndex: 0);
                        localSettings.Values["SourceToGo"] = x;
                        protocolargs = "incognito";
                    }
                    else if (eventArgs.Uri.AbsoluteUri.Contains("swiftlaunchincognito:"))
                    {
                        var x = eventArgs.Uri.AbsoluteUri.Remove(count: 21, startIndex: 0);
                        localSettings.Values["SourceToGo"] = null;
                        protocolargs = "incognito";
                    }
                    else
                    {
                        localSettings.Values["SourceToGo"] = eventArgs.Uri.AbsoluteUri;
                        protocolargs = "normal";
                    }
                }
                else
                {
                    protocolargs = "normal";
                }

                // The received URI is eventArgs.Uri.AbsoluteUri
            }

            //  }
            await ActivationService.ActivateAsync(args);
            //   }
        }

        private ActivationService CreateActivationService()
        {
            if (protocolargs == "incognito")
                return new ActivationService(this, typeof(IncognitoTabView));
            if (protocolargs == "normal")
                return new ActivationService(this, typeof(TabViewPage));
            return new ActivationService(this, typeof(TabViewPage));
        }

        public class FavouritesJSON
        {
            public string HeaderJSON { get; set; }
            public string UrlJSON { get; set; }
            public string FavIconJSON { get; set; }
        }

        public class FavouritesClass
        {
            public List<FavouritesJSON> Websites { get; set; }
        }
    }
}
