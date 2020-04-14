using SwiftBrowser.HubViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfflineCreatePage : Page
    {
        int width;
        int height;
        public static WebView webView {get; set; }
        public static string Name { get; set; }
        public OfflineCreatePage()
        {
            this.InitializeComponent();
            OfflinePainter.Source = webView.Source;
            Startup();
        }
        public async void Startup()
        {
            await Task.Delay(5000);
            var originalWidth = OfflinePainter.ActualWidth;
            var originalHeight = OfflinePainter.ActualHeight;

            var widthString = await OfflinePainter.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            var heightString = await OfflinePainter.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            if (!int.TryParse(widthString, out width))
            {
                throw new Exception("Unable to get page width");
            }
            if (!int.TryParse(heightString, out height))
            {
                throw new Exception("Unable to get page height");
            }

            OfflinePainter.Width = width;
            OfflinePainter.Height = height;
            Gridx.Height = height;
           // Gridx.Width = width;
            SaveConfirm.Show();
        }
        public async void SaveRTB(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(Gridx);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            StorageFile file = await localFolder.CreateFileAsync(Name + ".jpg");
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)rtb.PixelWidth,
                                     (uint)rtb.PixelHeight,
                                      displayInformation.RawDpiX,
                         displayInformation.RawDpiY,
                                     pixels);
                await encoder.FlushAsync();
            }
            OfflinePage.Singletonreference.OfflineAddToJSON(Name);
            var m = new MessageDialog("saved");
            await m.ShowAsync();
        }
    }
}
