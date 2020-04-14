using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib;
using System.Security.Cryptography;
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
using System.Threading.Tasks;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordMasterDetails : Page
    {
        public class KeepassClass
            {
            public string Title { get; set; }
            public string Website { get; set; }
        }
        public PasswordMasterDetails()
        {
            this.InitializeComponent();
        }

        private async void LoadKeeButton_Click(object sender, RoutedEventArgs e)
        {
            List<KeepassClass> PassList = new List<KeepassClass>();
            var dbpath = "";
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".kdb");
            picker.FileTypeFilter.Add(".kdbx");


            Windows.Storage.StorageFile File = await picker.PickSingleFileAsync();
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
          StorageFile file = await File.CopyAsync(folder);
            if (file != null)
            {
                dbpath = file.Path;
            }
            var masterpw = MasterPassword.Password;
            var ioConnInfo = new IOConnectionInfo { Path = file.Path };
            var compKey = new CompositeKey();
            compKey.AddUserKey(new KcpPassword(MasterPassword.Password));
            var db = new KeePassLib.PwDatabase();
          /*  var kdbx = new KdbxFile(db);
            using (var fs = await file.OpenReadAsync())
            {
                await Task.Run(() =>
                {
                    kdbx.Load(file.Path, KdbxFormat.Default, null);
                });

               // return new KdbxDatabase(dbFile, db, dbFile.IdFromPath());
            }*/
           db.Open(ioConnInfo, compKey, null);
            var kpdata = from entry in db.RootGroup.GetEntries(true)
                         select new
                         {
                             Group = entry.ParentGroup.Name,
                             Title = entry.Strings.ReadSafe("Title"),
                             Username = entry.Strings.ReadSafe("UserName"),
                             Password = entry.Strings.ReadSafe("Password"),
                             URL = entry.Strings.ReadSafe("URL"),
                             Notes = entry.Strings.ReadSafe("Notes")

                         };
            foreach (var Item in kpdata)
            {
                PassList.Add(new KeepassClass()
                {
                    Title = Item.Title,
                    Website = Item.URL
                });
            }
            PassListView.ItemsSource = PassList;
            db.Close();
        }

        private void PassListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
