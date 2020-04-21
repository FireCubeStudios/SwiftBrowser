using Newtonsoft.Json;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionsStore : Page
    {
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public ExtensionsStore()
        {
            this.InitializeComponent();
            LoadItems();
        }
 
        public class ExtensionsClass
        {
            public List<ExtensionsJSON>Extensions { get; set; }
        }

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
}
        public async void LoadItems()
        {
            List<ExtensionsJSON> ExtensionsListJSON = new List<ExtensionsJSON>();
            string filepath = @"Assets\Extensions.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath); // error here
            var JSONData = "e";
            JSONData = await Windows.Storage.FileIO.ReadTextAsync(file);
            ExtensionsClass ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
            foreach (var item in ExtensionsListJSONJSON.Extensions)
            {
                ExtensionsListJSON.Add(new ExtensionsJSON()
                {
                    NameJSON = item.NameJSON,
                    DescriptionJSON = item.DescriptionJSON,
                    IconJSON = item.IconJSON,
             
                });
            }
            ExtensionsList.ItemsSource = ExtensionsListJSON;
        }
    }
}
