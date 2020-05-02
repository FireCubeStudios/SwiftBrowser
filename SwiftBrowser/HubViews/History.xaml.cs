using Microsoft.Data.Sqlite;
using SwiftBrowser.Assets;
using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static SwiftBrowser.Assets.DataAccess;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews 
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class History : Page
    {
        private async void clear_click(object sender, RoutedEventArgs e)
        {
            StorageFile filed = await ApplicationData.Current.LocalFolder.GetFileAsync("sqliteHistory.db");
            if (filed != null)
            {
                await filed.DeleteAsync();
            }
            DataAccess.InitializeDatabase();
            Output.ItemsSource = null;
        }
        public History()
        {
            this.InitializeComponent();
        }
        private async void RemoveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var ide = (FrameworkElement)sender;
            var dataCxtx = ide.DataContext;
            HistoryClass dataSauce = (HistoryClass)dataCxtx;
            DataAccess.RemoveData(dataSauce);
            Output.ItemsSource = await GetData();
        }

        private void Output_ItemClick(object sender, ItemClickEventArgs e)
        {
            HistoryClass Historyitemclicked = e.ClickedItem as HistoryClass;
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
            VisualTreeHelper.DisconnectChildrenRecursive(this);
        }

        private async void CommandBar_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("Output");
            Output.ItemsSource = await GetData();
        }
    }
}
