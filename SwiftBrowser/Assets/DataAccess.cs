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
        public static Stack<HistoryClass> GetData()
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
