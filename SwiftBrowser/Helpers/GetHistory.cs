using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Collections;
using SwiftBrowser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace SwiftBrowser.Helpers
{
    public class HistoryClassList
    {
        public List<HistoryClass> Websites { get; set; }
    }
    public class GetHistory : IIncrementalSource<HistoryClass>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 50;
        public static bool firstrun = false;
        public static int skipInt = 0;
        public static string FirstId;
        private Stack<HistoryClass> entries;
        private IEnumerable<Stack<HistoryClass>> s;
       public async Task<IEnumerable<HistoryClass>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            //  Stack<HistoryClass> entries = new Stack<HistoryClass>();
            await Task.Run(() =>
            {
                entries = new Stack<HistoryClass>();
                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (SqliteConnection db =
                  new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    SqliteCommand selectCommand = new SqliteCommand
                       ("SELECT * FROM MyTable limit " + skipInt + "," + limit, db);
                    limit = limit + 50;
                    skipInt = skipInt + 50;
                    SqliteDataReader query = selectCommand.ExecuteReader();
                    while (query.Read())
                    {
                        if (firstrun == false)
                        {
                            firstrun = true;
                            FirstId = query.GetString(3);
                            Uri ArgsUri = new Uri(query.GetString(1));
                            string host = ArgsUri.Host;
                            entries.Push(new HistoryClass()
                            {
                                HeaderSQL = query.GetString(2),
                                UrlSQL = query.GetString(1),
                                IdSQL = query.GetString(3),
                                FavIconSQL = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                            });
                        }
                        else
                        {
                            if (query.GetString(3) == FirstId)
                            {
                                return;
                            }
                            else
                            {
                                Uri ArgsUri = new Uri(query.GetString(1));
                                string host = ArgsUri.Host;
                                entries.Push(new HistoryClass()
                                {
                                    HeaderSQL = query.GetString(2),
                                    UrlSQL = query.GetString(1),
                                    IdSQL = query.GetString(3),
                                    FavIconSQL = "http://icons.duckduckgo.com/ip2/" + host + ".ico",
                                });
                            }
                        }
                    }

                    db.Close();
                }
            });
            return entries;
        }

    }
}


