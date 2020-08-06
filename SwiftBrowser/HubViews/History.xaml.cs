using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp;
using SwiftBrowser.Helpers;
using SwiftBrowser.Models;
using SwiftBrowser.Views;
using static SwiftBrowser.Assets.DataAccess;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class History : Page
    {
        public History()
        {
            InitializeComponent();
        }

        private async void clear_click(object sender, RoutedEventArgs e)
        {
            var filed = await ApplicationData.Current.LocalFolder.GetFileAsync("sqliteHistory.db");
            if (filed != null) await filed.DeleteAsync();
            InitializeDatabase();
            Output.ItemsSource = null;
        }

        private async void RemoveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var ide = (FrameworkElement) sender;
            var dataCxtx = ide.DataContext;
            var dataSauce = (HistoryClass) dataCxtx;
            RemoveData(dataSauce);
            await Task.Delay(500);
            Output.ItemsSource = await GetData();
        }

        private void Output_ItemClick(object sender, ItemClickEventArgs e)
        {
            var Historyitemclicked = e.ClickedItem as HistoryClass;
            WebViewPage.WebviewControl.Navigate(new Uri(Historyitemclicked.UrlSQL));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Output.Items.Clear();
            }
            catch
            {
            }

            gRID.Children.Clear();
            GetHistory.limit = 50;
            GetHistory.skipInt = 0;
            GetHistory.firstrun = false;
            GetHistory.FirstId = null;
            VisualTreeHelper.DisconnectChildrenRecursive(this);
        }

        private async void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("Output");
            var HistoryCollection = new IncrementalLoadingCollection<GetHistory, HistoryClass>();

            Output.ItemsSource = HistoryCollection;
            //  Output.ItemsSource = await GetData();
        }
    }
}
