using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using SwiftBrowser.Models;

namespace SwiftBrowser.Assets
{
    public static class DataAccess
    {
        public static async void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteHistory.db",
                CreationCollisionOption.OpenIfExists);
            var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
            using (var db =
                new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = "CREATE TABLE IF NOT " +
                                   "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
                                   "Site VARCHAR(20), Header VARCHAR(20), ID VARCHAR(20))";

                var createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static async Task<Stack<HistoryClass>> GetData()
        {
            var entries = new Stack<HistoryClass>();
            await Task.Run(() =>
            {
                var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (var db =
                    new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    var selectCommand = new SqliteCommand
                        ("SELECT * FROM MyTable", db);

                    var query = selectCommand.ExecuteReader();

                    while (query.Read())
                    {
                        var ArgsUri = new Uri(query.GetString(1));
                        var host = ArgsUri.Host;
                        entries.Push(new HistoryClass
                        {
                            HeaderSQL = query.GetString(2),
                            UrlSQL = query.GetString(1),
                            IdSQL = query.GetString(3),
                            FavIconSQL = "http://icons.duckduckgo.com/ip2/" + host + ".ico"
                        });
                    }

                    db.Close();
                }

                return entries;
            });
            return entries;
        }

        public static async void AddDataS(string inputText, string inputHeaderText)
        {
            await Task.Run(() =>
            {
                var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (var db =
                    new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    var insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO MyTable(Site, Header, ID) VALUES (@Entry, @Header, @ID);";
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";
                    //   insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                    insertCommand.Parameters.AddWithValue("@Entry", inputText);
                    insertCommand.Parameters.AddWithValue("@Header", inputHeaderText);
                    var r = new Random();
                    insertCommand.Parameters.AddWithValue("@ID", r.Next().ToString());
                    insertCommand.ExecuteReader();

                    db.Close();
                }
            });
        }

        public static async void AddData(string inputText, string inputHeaderText)
        {
            await Task.Run(() =>
            {
                var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (var db =
                    new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    var insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO MyTable(Site, Header, ID) VALUES (@Entry, @Header, @ID);";
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";
                    //   insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                    insertCommand.Parameters.AddWithValue("@Entry", inputText);
                    insertCommand.Parameters.AddWithValue("@Header", inputHeaderText);
                    var r = new Random();
                    insertCommand.Parameters.AddWithValue("@ID", r.Next().ToString());
                    insertCommand.ExecuteReader();

                    db.Close();
                }
            });
            try
            {
                if ((bool) ApplicationData.Current.RoamingSettings.Values["Syncing"])
                {
                    //  try
                    // {
                    if ((bool) ApplicationData.Current.RoamingSettings.Values["NewData"] == false)
                    {
                        ApplicationData.Current.RoamingSettings.Values["SyncId"] =
                            ApplicationData.Current.LocalSettings.Values["SyncId"];
                        var roaming = ApplicationData.Current.RoamingFolder;
                        var SyncFile =
                            await roaming.CreateFileAsync("SyncFile.json", CreationCollisionOption.OpenIfExists);
                        var Data = await FileIO.ReadTextAsync(SyncFile);
                        try
                        {
                            var SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(Data);
                            var ArgsUri = new Uri(inputText);
                            var host = ArgsUri.Host;
                            SyncListJSON.Sync.Add(new SyncJSON
                            {
                                HeaderJSON = inputHeaderText,
                                UrlJSON = inputText,
                                FavIconJSON = "http://icons.duckduckgo.com/ip2/" + host + ".ico"
                            });
                            ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                            var SerializedObject = JsonConvert.SerializeObject(SyncListJSON, Formatting.Indented);
                            await FileIO.WriteTextAsync(SyncFile, SerializedObject);
                        }
                        catch
                        {
                            ApplicationData.Current.RoamingSettings.Values["SyncId"] =
                                ApplicationData.Current.LocalSettings.Values["SyncId"];
                            var filepath = @"Assets\SyncFile.json";
                            var folder = Package.Current.InstalledLocation;
                            var file = await folder.GetFileAsync(filepath);
                            var SyncListJSON =
                                JsonConvert.DeserializeObject<SyncClass>(await FileIO.ReadTextAsync(file));
                            var ArgsUri = new Uri(inputText);
                            var host = ArgsUri.Host;
                            SyncListJSON.Sync.Add(new SyncJSON
                            {
                                HeaderJSON = inputHeaderText,
                                UrlJSON = inputText,
                                FavIconJSON = "http://icons.duckduckgo.com/ip2/" + host + ".ico"
                            });
                            var SerializedObject = JsonConvert.SerializeObject(SyncListJSON, Formatting.Indented);
                            var roamingX = ApplicationData.Current.RoamingFolder;
                            var SyncFileX = await roamingX.GetFileAsync("SyncFile.json");
                            await FileIO.WriteTextAsync(SyncFileX, SerializedObject);
                            ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                        }

                        //ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                    }
                    else
                    {
                        //  try
                        // {
                        if ((bool) ApplicationData.Current.RoamingSettings.Values["Syncing"])
                        {
                            // try
                            // {
                            if ((bool) ApplicationData.Current.RoamingSettings.Values["NewData"])
                            {
                                ApplicationData.Current.RoamingSettings.Values["SyncId"] =
                                    ApplicationData.Current.LocalSettings.Values["SyncId"];
                                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                                var roaming = ApplicationData.Current.RoamingFolder;
                                var SyncFile = await roaming.GetFileAsync("SyncFile.json");
                                var Data = await FileIO.ReadTextAsync(SyncFile);
                                var SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(Data);
                                foreach (var item in SyncListJSON.Sync) AddDataS(item.UrlJSON, item.HeaderJSON);
                                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                                await SyncFile.DeleteAsync();
                            }
                            else
                            {
                                var roaming = ApplicationData.Current.RoamingFolder;
                                try
                                {
                                    var SyncFile = await roaming.CreateFileAsync("SyncFile.json",
                                        CreationCollisionOption.FailIfExists);
                                }
                                catch
                                {
                                }
                            }

                            // }
                            /*   catch
                               {
                                   ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                                   StorageFolder roaming = Windows.Storage.ApplicationData.Current.RoamingFolder;
                                   try
                                   {
                                       StorageFile SyncFile = await roaming.CreateFileAsync("SyncFile.json", CreationCollisionOption.FailIfExists);
                                   }
                                   catch
                                   {

                                   }
                               }*/
                        }

                        // }
                        /*  catch
                          {
                              Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
                          }*/
                    }
                }
            }
            catch
            {
                ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
            }
        }

        public static async void RemoveData(HistoryClass id)
        {
            await Task.Run(() =>
            {
                var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (var db =
                    new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    var DeleteCommand = new SqliteCommand();
                    DeleteCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    DeleteCommand.CommandText = "DELETE FROM MyTable WHERE ID = '" + id.IdSQL + "'";
                    DeleteCommand.ExecuteNonQuery();
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";

                    //  DeleteCommand.Parameters.Remove(id.HeaderSQL);
                    //     DeleteCommand.ExecuteReader();

                    db.Close();
                }
            });
        }

        public class HistoryClassList
        {
            public List<HistoryClass> Websites { get; set; }
        }

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
    }
}
