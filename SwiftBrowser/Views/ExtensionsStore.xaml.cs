using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionsStore : Page
    {
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        ExtensionsClass ExtensionsListJSONJSON;
        public static ExtensionsStore methods {get; set;}
        public ExtensionsStore()
        {
            this.InitializeComponent();
            methods = this;
            LoadItems();
        }
 
        public class ExtensionsClass
        {
            public List<ExtensionsJSON>Extensions { get; set; }
        }
        List<ExtensionsJSON> ExtensionsListJSON;
        /*  public class Extensions
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class ExtensionsJSON
        {
            public string NameJSON { get; set; }
            public string DescriptionJSON { get; set; }
            public string IconJSON { get; set; }
            public int Id { get; set; }
            public bool IsEnabledJSON { get; set; }
            public bool IsIncognitoEnabled{ get; set; }
        public bool IsToolbar{ get; set; }
            public string Page { get; set; }
}
        public async void LoadItems()
        {
            ExtensionsListJSON = new List<ExtensionsJSON>();
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("Extensions.json"); // error here
            var JSONData = "e";
            JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
            ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
            foreach (var item in ExtensionsListJSONJSON.Extensions)
            {
                ExtensionsListJSON.Add(new ExtensionsJSON()
                {
                    NameJSON = item.NameJSON,
                    DescriptionJSON = item.DescriptionJSON,
                    IconJSON = item.IconJSON,
                    IsEnabledJSON = item.IsEnabledJSON,
                    Page = item.Page,
                    IsToolbar = item.IsToolbar
                });
            }
            ExtensionsList.ItemsSource = ExtensionsListJSON;
        }
        public async void closing()
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("Extensions.json"); 
            var SerializedObject = JsonConvert.SerializeObject(ExtensionsListJSONJSON, Formatting.Indented);

                await Windows.Storage.FileIO.WriteTextAsync(file, SerializedObject);
                LoadingControl.IsLoading = false;

        }
        private void Toggle_Toggled(object sender, RoutedEventArgs e)
        {
            List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
          // LoadingControl.IsLoading = true;
            ToggleSwitch t = sender as ToggleSwitch;
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
           ExtensionsJSON SenderPost = DataContext as ExtensionsJSON;
           SenderPost.IsEnabledJSON = t.IsOn;
          // ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
            ExtensionsJSON FoundItem = ExtensionsListJSONJSON.Extensions.Find(x => SenderPost.Id == SenderPost.Id);
          // SenderPost.IsToolbar = FoundItem.IsToolbar;
            //  SenderPost.IsEnabledJSON = FoundItem.IsEnabledJSON;
            //   FoundItem.IsEnabledJSON = t.IsOn;
            FoundItem.IsEnabledJSON = t.IsOn;
            StackPanel i = VisualTreeHelper.GetParent(t) as StackPanel;
            ToggleSwitch tt = i.Children[2] as ToggleSwitch;
        //    tt.IsOn = FoundItem.IsToolbar;
            //   SenderPost.IsToolbar = tt.IsOn;
            if(FoundItem.IsEnabledJSON == false)
            {
                tt.IsOn = false;
            }
            SenderPost = FoundItem;
            FoundItem.IsToolbar = tt.IsOn;
            ExtensionsListJSONJSON.Extensions.Remove(FoundItem);
            ExtensionsListJSONJSON.Extensions.Add(FoundItem);
        }

        private void ToggleToolbar_Toggled(object sender, RoutedEventArgs e)
        {
            List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
         //   LoadingControl.IsLoading = true;
            ToggleSwitch t = sender as ToggleSwitch;
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            ExtensionsJSON SenderPost = DataContext as ExtensionsJSON;

            ExtensionsJSON FoundItem = ExtensionsListJSONJSON.Extensions.Find(x => SenderPost.Id == SenderPost.Id);
            FoundItem.IsToolbar = t.IsOn;
           SenderPost.IsToolbar = t.IsOn;
           StackPanel i = VisualTreeHelper.GetParent(t) as StackPanel;
            ToggleSwitch tt = i.Children[1] as ToggleSwitch;
           SenderPost.IsEnabledJSON = tt.IsOn;
            FoundItem.IsEnabledJSON = tt.IsOn;
            if(FoundItem.IsToolbar == true)
            {
                tt.IsOn = true;
            }
            SenderPost = FoundItem;
            ExtensionsListJSONJSON.Extensions.Remove(FoundItem);
            ExtensionsListJSONJSON.Extensions.Add(FoundItem);

        }
    }
}
