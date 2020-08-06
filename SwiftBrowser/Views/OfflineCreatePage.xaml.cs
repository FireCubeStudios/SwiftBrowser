using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using SwiftBrowser.HubViews;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfflineCreatePage : Page
    {
        private int height;
        private int width;

        public OfflineCreatePage()
        {
            InitializeComponent();
            try
            {
                OfflinePainter.Source = webView.Source;
                Startup();
            }
            catch
            {
            }
        }

        public static WebView webView { get; set; }
        public static string NameX { get; set; }

        public async void Startup()
        {
            await Task.Delay(5000);
            var originalWidth = OfflinePainter.ActualWidth;
            var originalHeight = OfflinePainter.ActualHeight;

            var widthString =
                await OfflinePainter.InvokeScriptAsync("eval", new[] {"document.body.scrollWidth.toString()"});
            var heightString =
                await OfflinePainter.InvokeScriptAsync("eval", new[] {"document.body.scrollHeight.toString()"});

            if (!int.TryParse(widthString, out width)) throw new Exception("Unable to get page width");
            if (!int.TryParse(heightString, out height)) throw new Exception("Unable to get page height");

            OfflinePainter.Width = width;
            OfflinePainter.Height = height;
            Gridx.Height = height;
            // Gridx.Width = width;
            SaveConfirm.Show();
        }

        public async void SaveRTB(object sender, RoutedEventArgs e)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(Gridx);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            var file = await localFolder.CreateFileAsync(Name + ".jpg");
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint) rtb.PixelWidth,
                    (uint) rtb.PixelHeight,
                    displayInformation.RawDpiX,
                    displayInformation.RawDpiY,
                    pixels);
                await encoder.FlushAsync();
            }

            OfflinePage.Singletonreference.OfflineAddToJSON(Name);
            var duration = 3000;
            try
            {
                TabViewPage.InAppNotificationMain.Show("Image saved", duration);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show("Image saved", duration);
            }
        }
    }
}
