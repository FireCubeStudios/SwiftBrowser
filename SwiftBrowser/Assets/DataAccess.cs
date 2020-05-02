using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace SwiftBrowser.Assets
{
    public static class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteHistory.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
                    "Site VARCHAR(20),Header VARCHAR(20))";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
        public class HistoryClassList
        {
            public List<HistoryClass> Websites { get; set; }
        }
        public class HistoryClass
            {
            public string HeaderSQL { get; set; }
            public string UrlSQL { get; set; }
            public string FavIconSQL { get; set; }
        }
        public async static Task<Stack<HistoryClass>> GetData()
        {
            Stack<HistoryClass> entries = new Stack<HistoryClass>();


                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (SqliteConnection db =
                  new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand
                       ("SELECT * FROM MyTable", db);

                    SqliteDataReader query = selectCommand.ExecuteReader();

                    while (query.Read())
                    {
                        Uri ArgsUri = new Uri(query.GetString(1));
                        string host = ArgsUri.Host;
                        entries.Push(new HistoryClass()
                        {
                            HeaderSQL = query.GetString(2),
                            UrlSQL = query.GetString(1),
                            FavIconSQL = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                        });
                    }

                    db.Close();
                }
                return entries;
        }
        public async static void AddDataS(string inputText, string inputHeaderText)
        {
            await Task.Run(() =>
            {
                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (SqliteConnection db =
                  new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO MyTable(Site, Header) VALUES (@Entry, @Header);";
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";
                    //   insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                    insertCommand.Parameters.AddWithValue("@Entry", inputText);
                    insertCommand.Parameters.AddWithValue("@Header", inputHeaderText);
                    insertCommand.ExecuteReader();

                    db.Close();
                }
            });
            }
            public async static void AddData(string inputText, string inputHeaderText)
        {
            await Task.Run(() =>
            {
                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (SqliteConnection db =
                  new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO MyTable(Site, Header) VALUES (@Entry, @Header);";
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";
                    //   insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                    insertCommand.Parameters.AddWithValue("@Entry", inputText);
                    insertCommand.Parameters.AddWithValue("@Header", inputHeaderText);
                    insertCommand.ExecuteReader();

                    db.Close();
                }
            });
            try
            {


                if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] == true)
                {
                    //  try
                    // {
                    if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] == false)
                    {
                        Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                        StorageFolder roaming = Windows.Storage.ApplicationData.Current.RoamingFolder;
                        StorageFile SyncFile = await roaming.CreateFileAsync("SyncFile.json", CreationCollisionOption.OpenIfExists);
                        string Data = await FileIO.ReadTextAsync(SyncFile);
                        try
                        {
                            SyncClass SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(Data);
                            Uri ArgsUri = new Uri(inputText);
                            string host = ArgsUri.Host;
                            SyncListJSON.Sync.Add(new SyncJSON()
                            {
                                HeaderJSON = inputHeaderText,
                                UrlJSON = inputText,
                                FavIconJSON = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                            });
                            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                            var SerializedObject = JsonConvert.SerializeObject(SyncListJSON, Formatting.Indented);
                            await Windows.Storage.FileIO.WriteTextAsync(SyncFile, SerializedObject);
                        }
                        catch
                        {
                            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                            string filepath = @"Assets\SyncFile.json";
                            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                            StorageFile file = await folder.GetFileAsync(filepath);
                            SyncClass SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(await FileIO.ReadTextAsync(file));
                            Uri ArgsUri = new Uri(inputText);
                            string host = ArgsUri.Host;
                            SyncListJSON.Sync.Add(new SyncJSON()
                            {
                                HeaderJSON = inputHeaderText,
                                UrlJSON = inputText,
                                FavIconJSON = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                            });
                            var SerializedObject = JsonConvert.SerializeObject(SyncListJSON, Formatting.Indented);
                            StorageFolder roamingX = Windows.Storage.ApplicationData.Current.RoamingFolder;
                            StorageFile SyncFileX = await roamingX.GetFileAsync("SyncFile.json");
                            await FileIO.WriteTextAsync(SyncFileX, SerializedObject);
                            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                        }

                        //ApplicationData.Current.RoamingSettings.Values["NewData"] = true;
                    }
                    else
                    {

                        //  try
                        // {
                        if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] == true)
                        {
                            // try
                            // {
                            if ((bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] == true)
                            {
                                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["SyncId"] = Windows.Storage.ApplicationData.Current.LocalSettings.Values["SyncId"];
                                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                                StorageFolder roaming = Windows.Storage.ApplicationData.Current.RoamingFolder;
                                StorageFile SyncFile = await roaming.GetFileAsync("SyncFile.json");
                                string Data = await FileIO.ReadTextAsync(SyncFile);
                                SyncClass SyncListJSON = JsonConvert.DeserializeObject<SyncClass>(Data);
                                foreach (var item in SyncListJSON.Sync)
                                {
                                    DataAccess.AddDataS(item.UrlJSON, item.HeaderJSON);
                                }
                                ApplicationData.Current.RoamingSettings.Values["NewData"] = false;
                                await SyncFile.DeleteAsync();
                            }
                            else
                            {
                                StorageFolder roaming = Windows.Storage.ApplicationData.Current.RoamingFolder;
                                try
                                {
                                    StorageFile SyncFile = await roaming.CreateFileAsync("SyncFile.json", CreationCollisionOption.FailIfExists);
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
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Syncing"] = false;
            }
            
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
        public async static void RemoveData(HistoryClass id)
        {
            await Task.Run(() =>
            {
                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (SqliteConnection db =
                  new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand DeleteCommand = new SqliteCommand();
                    DeleteCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    DeleteCommand.CommandText = "DELETE FROM MyTable WHERE Header = '" + id.HeaderSQL + "'";
                    DeleteCommand.ExecuteNonQuery();
                    // = "INSERT INTO MyTable(Col1, Col2) VALUES('Test Text ', 1); ";

                    //  DeleteCommand.Parameters.Remove(id.HeaderSQL);
                    //     DeleteCommand.ExecuteReader();

                    db.Close();
                }
            });
        }
    }
}
