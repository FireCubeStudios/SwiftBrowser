using Microsoft.Toolkit.Uwp.UI.Controls;
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
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Diagnostics;
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
    public sealed partial class SnipPage : Page
    {
        public static IRandomAccessStream WebView { get; set; }
        public SnipPage()
        {
            this.InitializeComponent();
             CaptureWebView();
        }
        private async void CaptureWebView()
        {

            /* var originalWidth = WebView.ActualWidth;
             var originalHeight = WebView.ActualHeight;

             var widthString = await WebView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
             var heightString = await WebView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

             if (!int.TryParse(widthString, out width))
             {
                 throw new Exception("Unable to get page width");
             }
             if (!int.TryParse(heightString, out height))
             {
                 throw new Exception("Unable to get page height");
             }

             // resize the webview to the content
             // WebView.Width = width;
             WebView.Height = height;

             var brush = new WebViewBrush();
             brush.SetSource(WebView);
             await Task.Delay(3000);
             SnipCropper.Width = width;
             SnipCropper.Height = height;
             Gridx.Height = height;
             Gridx.Width = width;
             Painter.Width = width;
             Painter.Height = height;
             Painter.Fill = brush;
             RenderTargetBitmap rtb = new RenderTargetBitmap();
             await rtb.RenderAsync(Gridx);
             var pixelBuffer = await rtb.GetPixelsAsync();
             var pixels = pixelBuffer.ToArray();
             IRandomAccessStream stream = null;

             WriteableBitmap bit = new WriteableBitmap(rtb.PixelWidth, rtb.PixelHeight);
             await bit.SetSourceAsync(stream);
             using (stream)
             {
                 var displayInformation = DisplayInformation.GetForCurrentView();
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
             SnipCropper.Source = bit;
             WebView.Height = 1000;*/
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(Gridx);
            WriteableBitmap WB = new WriteableBitmap(rtb.PixelWidth, rtb.PixelHeight);
            WB.SetSource(WebView);
            SnipCropper.Source = WB;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Png Image", new string[] { ".png" });
            StorageFile file = await picker.PickSaveFileAsync();
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await SnipCropper.SaveAsync(stream, BitmapFileFormat.Png);

            }
            var m = new MessageDialog("Screen captured and Saved");
            await m.ShowAsync();
        }

    }
}
