using System;                 
using SwiftBrowser.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;
using SwiftBrowser.Assets;
using SwiftBrowser.Views;
using Windows.UI.Xaml.Controls;
using SwiftBrowser.HubViews;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.StartScreen;
using System.Collections.Generic;

namespace SwiftBrowser
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            App.Current.UnhandledException += OnUnhandledException;
            DataAccess.InitializeDatabase();
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }
        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            unhandledExceptionEventArgs.Handled = true;
           /* ErrorDialog E = new ErrorDialog();
            ErrorDialog.errormsgStatic = unhandledExceptionEventArgs.Message;
            ErrorDialog.errormsgExeptionStatic = unhandledExceptionEventArgs.Exception.ToString();
               await E.ShowAsync();*/
        }
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (SystemInformation.IsFirstRun)
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
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["AdBlocker"] = false;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["IndexDB"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Javascript"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["StoreHistory"] = true;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.ecosia.org/search?q=";
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["WebNotifications"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["IDBUnlimitedPermision"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Media"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Screen"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["WebVR"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Geolocation"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["PointerLock"] = 0;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserAgent"] = "Default";

                Random rnd = new Random();
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"] = rnd.Next().ToString();
                string filepath = @"Assets\Extensions.json";
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder f = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync(filepath);
                StorageFile sfile = await f.CreateFileAsync("Extensions.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sfile, await Windows.Storage.FileIO.ReadTextAsync(file));
            }
                var idOfTappedTile = args.Arguments;
            if(idOfTappedTile.Contains("http") == true) { 
                localSettings.Values["SourceToGo"] = idOfTappedTile.ToString();
            }
            else
            {
                localSettings.Values["SourceToGo"] = null;
            }
            await ConfigureJumpList();
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
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
        private async Task ConfigureJumpList()
        {
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            // using the favorites class
            // TODO: change name of "favorites" class to something else
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            // Disable the system-managed jump list group.
            jumpList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.None;

            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("QuickPinWeb.json");
                var JSONData = await FileIO.ReadTextAsync(sampleFile);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    var itemj = JumpListItem.CreateWithArguments("item.UrlJSON.ToString()", "item.HeaderJSON.ToString()");

                    itemj.Description = "item.UrlJSON";

                    itemj.GroupName = "Quick pinned: ";


                    itemj.Logo = new Uri("ms-appx:///Assets/SmallTile.scale-100.png");



                    jumpList.Items.Add(itemj);
                }
            }
            catch
            {
                var JSONData = "e";
                string filepath = @"Assets\QuickPinWeb.json";
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await folder.GetFileAsync(filepath);
                JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
                //  StorageFile sfile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                //  await FileIO.WriteTextAsync(sfile, JSONData);
                StorageFile sampleFile = await localFolder.CreateFileAsync("QuickPinWeb.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, JSONData);
                FavouritesClass FavouritesListJSON = JsonConvert.DeserializeObject<FavouritesClass>(JSONData);
                foreach (var item in FavouritesListJSON.Websites)
                {
                    var itemj = JumpListItem.CreateWithArguments(item.UrlJSON.ToString(), item.HeaderJSON.ToString());

                    itemj.Description = "item.UrlJSON";

                    itemj.GroupName = "Quick pinned: ";

                    itemj.Logo = new Uri("ms-appx:///Assets/SmallTile.scale-100.png");



                    jumpList.Items.Add(itemj);
                }
                await jumpList.SaveAsync();
            }
        }
        private void Widget1Window_Closed(object sender, Windows.UI.Core.CoreWindowEventArgs e)

        {

            Window.Current.Closed -= Widget1Window_Closed;

        }
        protected override async void OnActivated(IActivatedEventArgs args)
        {


                          if (args.Kind == ActivationKind.Protocol)
                    {
                        ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                if (string.IsNullOrEmpty(eventArgs.Uri.AbsoluteUri.ToString()) == false)
                {
                    if (eventArgs.Uri.AbsoluteUri.ToString().Contains("swiftlaunch:") == true && eventArgs.Uri.AbsoluteUri.ToString().Length > 12)
                    {
                        string x = eventArgs.Uri.AbsoluteUri.ToString().Remove(count:12, startIndex:0);
                      localSettings.Values["SourceToGo"] = x;
                        protocolargs = "normal";
                    }
                    else if(eventArgs.Uri.AbsoluteUri.ToString().Contains("swiftlaunch:") == true)
                    {
                        string x = eventArgs.Uri.AbsoluteUri.ToString().Remove(count: 12, startIndex: 0);
                        localSettings.Values["SourceToGo"] = null;
                        protocolargs = "normal";
                    }
                    else if (eventArgs.Uri.AbsoluteUri.ToString().Contains("swiftlaunchincognito:") == true && eventArgs.Uri.AbsoluteUri.ToString().Length > 21)
                    {
                        string x = eventArgs.Uri.AbsoluteUri.ToString().Remove(count: 21, startIndex: 0);
                        localSettings.Values["SourceToGo"] = x;
                        protocolargs = "incognito";
                    }
                    else if(eventArgs.Uri.AbsoluteUri.ToString().Contains("swiftlaunchincognito:") == true)
                    {
                        string x = eventArgs.Uri.AbsoluteUri.ToString().Remove(count: 21, startIndex: 0);
                        localSettings.Values["SourceToGo"] = null;
                        protocolargs = "incognito";
                    }
                    else
                    {
                        localSettings.Values["SourceToGo"] = eventArgs.Uri.AbsoluteUri.ToString();
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
        string protocolargs;
        private ActivationService CreateActivationService()
        {
            if (protocolargs == "incognito")
            {
                return new ActivationService(this, typeof(Views.IncognitoTabView));
            }
            else if(protocolargs == "normal")
            {
                return new ActivationService(this, typeof(Views.TabViewPage));
            }
            else
            {
                return new ActivationService(this, typeof(Views.TabViewPage));
            }
        }
    }
}
