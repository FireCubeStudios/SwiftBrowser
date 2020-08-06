using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using LastPass;
using SwiftBrowser.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordMasterDetails : Page
    {
        public PasswordMasterDetails()
        {
            InitializeComponent();
            if ((bool) ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePathBool"] == false)
            {
                LoadKeeButton.Visibility = Visibility.Visible;
                AddKeeButton.Visibility = Visibility.Collapsed;
                EnterKeeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadKeeButton.Visibility = Visibility.Collapsed;
                AddKeeButton.Visibility = Visibility.Visible;
                EnterKeeButton.Visibility = Visibility.Visible;
            }
        }

        private async void LoadKeeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var PassList = new List<KeepassClass>();
                var dbpath = "";
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".kdb");
                picker.FileTypeFilter.Add(".kdbx");


                var File = await picker.PickSingleFileAsync();
                var folder = ApplicationData.Current.LocalFolder;
                var file = await File.CopyAsync(folder);
                if (file != null) dbpath = file.Path;
                var masterpw = MasterPassword.Password;
                var ioConnInfo = new IOConnectionInfo {Path = file.Path};
                var compKey = new CompositeKey();
                compKey.AddUserKey(new KcpPassword(MasterPassword.Password));
                var db = new PwDatabase();
                ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePathBool"] = true;
                ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"] = file.Name;
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
                    PassList.Add(new KeepassClass
                    {
                        Title = Item.Title,
                        Website = Item.URL,
                        Password = Item.Password,
                        User = Item.Username
                    });
                PassListView.ItemsSource = PassList;
                LoadKeeButton.Visibility = Visibility.Collapsed;
                AddKeeButton.Visibility = Visibility.Visible;
                EnterKeeButton.Visibility = Visibility.Visible;
                db.Close();
            }
            catch
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Wrong password!", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Wrong password!", duration);
                }

                ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePathBool"] = false;
                ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"] = null;
            }
        }

        public async void LoadKeePass(object sender, RoutedEventArgs e)
        {
            try
            {
                var PassList = new List<KeepassClass>();
                var dbpath = "";
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(
                    (string) ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"]);
                if (file != null) dbpath = file.Path;
                var masterpw = MasterPassword.Password;
                var ioConnInfo = new IOConnectionInfo {Path = file.Path};
                var compKey = new CompositeKey();
                compKey.AddUserKey(new KcpPassword(MasterPassword.Password));
                var db = new PwDatabase();

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
                    PassList.Add(new KeepassClass
                    {
                        Title = Item.Title,
                        Website = Item.URL,
                        Password = Item.Password,
                        User = Item.Username
                    });
                PassListView.ItemsSource = PassList;
                db.Close();
            }
            catch
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("An error occured", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("An error occured", duration);
                }
            }
        }

        private async void PassListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var keepassword = e.ClickedItem as KeepassClass;
            var duration = 3000;
            try
            {
                TabViewPage.InAppNotificationMain.Show(keepassword.Password, duration);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show(keepassword.Password, duration);
            }
        }

        private async void AddKeeButton_Click(object sender, RoutedEventArgs e)
        {
            var PassList = new List<KeepassClass>();
            var dbpath = "";
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(
                (string) ApplicationData.Current.LocalSettings.Values["KeepassLocalFilePath"]);
            if (file != null) dbpath = file.Path;
            var masterpw = MasterPassword.Password;
            var ioConnInfo = new IOConnectionInfo {Path = file.Path};
            var compKey = new CompositeKey();
            compKey.AddUserKey(new KcpPassword(MasterPassword.Password));
            var db = new PwDatabase();

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
            var pwEntry = new PwEntry(true, true);

            if (!string.IsNullOrEmpty(Title.Text))
                pwEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, Title.Text));

            if (!string.IsNullOrEmpty(User.Text))
                pwEntry.Strings.Set(PwDefs.UserNameField, new ProtectedString(true, User.Text));

            if (!string.IsNullOrEmpty(Password.Text))
                pwEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, Password.Text));

            if (!string.IsNullOrEmpty(Notes.Text))
                pwEntry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, Notes.Text));

            if (!string.IsNullOrEmpty(Url.Text))
                pwEntry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, Url.Text));
            // db.RootGroup.AddEntry(pwEntry, true);
            db.RootGroup.Entries.Add(pwEntry);
            db.Modified = true;
            // db.Save();
            var stream = await file.OpenStreamForWriteAsync();
            var kdbx = new KdbxFile(db);
            kdbx.Save(stream, null, KdbxFormat.Default, null);
            db.Close();
            var duration = 3000;
            try
            {
                TabViewPage.InAppNotificationMain.Show("Saved, please refresh list", duration);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show("Saved, please refresh list", duration);
            }
        }

        private void LoadLastPassButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] B = { };
            var blob = new Blob(B, 2, "e");
            var vault = Vault.Create(blob, LastPassUser.Text, LastPassPassword.Text);
            for (var i = 0; i < vault.Accounts.Length; ++i)

            {
                var account = vault.Accounts[i];

                Console.WriteLine("{0}:\n" +
                                  "        id: {1}\n" +
                                  "      name: {2}\n" +
                                  "  username: {3}\n" +
                                  "  password: {4}\n" +
                                  "       url: {5}\n" +
                                  "     group: {6}\n",
                    i + 1,
                    account.Id,
                    account.Name,
                    account.Username,
                    account.Password,
                    account.Url,
                    account.Group);
            }
        }

        public class KeepassClass
        {
            public string Title { get; set; }
            public string Website { get; set; }
            public string Password { get; set; }
            public string User { get; set; }
        }

        private class TextUi : Ui

        {
            public override string ProvideSecondFactorPassword(SecondFactorMethod method)

            {
                return GetAnswer(string.Format("Please enter {0} code", method));
            }


            public override void AskToApproveOutOfBand(OutOfBandMethod method)

            {
                Console.WriteLine("Please approve out-of-band via {0}", method);
            }


            private static string GetAnswer(string prompt)

            {
                Console.WriteLine(prompt);

                Console.Write("> ");

                var input = Console.ReadLine();


                return input == null ? "" : input.Trim();
            }
        }
    }
}
