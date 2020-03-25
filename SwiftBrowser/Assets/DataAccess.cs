using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
                    "Text_Entry NVARCHAR(2048) NULL)";

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
        public static List<HistoryClass> GetData()
        {
            List<HistoryClass> entries = new List<HistoryClass>();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Text_Entry from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Uri ArgsUri = new Uri(query.GetString(0));
                    string host = ArgsUri.Host;
                    entries.Add(new HistoryClass()
                    {
                        HeaderSQL = query.GetString(0),
                        UrlSQL = query.GetString(0),
                        FavIconSQL = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                    });
                }

                db.Close();
            }

            return entries;
        }
        public static void AddData(string inputText)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }
    }
}
