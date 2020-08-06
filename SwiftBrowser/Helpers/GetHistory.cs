using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Collections;
using SwiftBrowser.Models;

namespace SwiftBrowser.Helpers
{
    public class GetHistory : IIncrementalSource<HistoryClass>
    {
        public static int limit = 50;
        public static bool firstrun;
        public static int skipInt;
        public static string FirstId;
        private Stack<HistoryClass> entries;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private IEnumerable<Stack<HistoryClass>> s;

        public async Task<IEnumerable<HistoryClass>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            //  Stack<HistoryClass> entries = new Stack<HistoryClass>();
            await Task.Run(() =>
            {
                entries = new Stack<HistoryClass>();
                var dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteHistory.db");
                using (var db =
                    new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();

                    var selectCommand = new SqliteCommand
                        ("SELECT * FROM MyTable limit " + skipInt + "," + limit, db);
                    limit = limit + 50;
                    skipInt = skipInt + 50;
                    var query = selectCommand.ExecuteReader();
                    while (query.Read())
                        if (firstrun == false)
                        {
                            firstrun = true;
                            FirstId = query.GetString(3);
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
                        else
                        {
                            if (query.GetString(3) == FirstId) return;

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
            });
            return entries;
        }
    }
}
