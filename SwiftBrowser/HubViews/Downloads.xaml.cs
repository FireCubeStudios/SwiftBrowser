using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.HubViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Downloads : Page
    {
        public Downloads()
        {
            InitializeComponent();
            //      start();
        }

        //  public async void start()
        //   {
        /*  await Task.Delay(5000);
          await Task.Run(async() =>
          {
            //  var downloadOpt = new DownloadConfiguration()
          {
              ParallelDownload = true, // download parts of file as parallel or not
              BufferBlockSize = 10240, // usually, hosts support max to 8000 bytes
              ChunkCount = 8, // file parts to download
              MaxTryAgainOnFailover = int.MaxValue, // the maximum number of times to fail.
              OnTheFlyDownload = true, // caching in-memory mode
              Timeout = 1000 // timeout (millisecond) per stream block reader
          };
             await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
  async() =>
  {
     // var downloader = new DownloadService(downloadOpt);
                  downloader.DownloadProgressChanged += OnDownloadProgressChangedAsync;

                  downloader.DownloadFileCompleted += OnDownloadFileCompleted;
                  var file = @"Downloads\Annotation-2019-10-04-194747.png";
      await downloader.DownloadFileTaskAsync("https://i.ibb.co/YDHN1HY/Annotation-2019-10-04-194747.png", file);
              });*/
        /*  });
          }
          private static async void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)

          {

              await Task.Delay(1000);

              Console.WriteLine();



              if (e.Cancelled)

              {

                  Console.WriteLine("Download canceled!");

              }

              else if (e.Error != null)

              {

                  Console.Error.WriteLine(e.Error);

              }

              else

              {

                  Console.WriteLine("Download completed successfully.");

                  Console.Title = "100%";

              }

          }



          private static async void OnDownloadProgressChangedAsync(object sender, DownloadProgressChangedEventArgs e)

          {

              var nonZeroSpeed = e.BytesPerSecondSpeed == 0 ? 0.0001 : e.BytesPerSecondSpeed;

              var estimateTime = (int)((e.TotalBytesToReceive - e.BytesReceived) / nonZeroSpeed);

              var isMins = estimateTime >= 60;

            var timeLeftUnit = "seconds";

              if (isMins)

              {

                  timeLeftUnit = "mins";

                  estimateTime /= 60;

              }



              //Console.Title = $"{e.ProgressPercentage:N3}%  -  {CalcMemoryMensurableUnit(e.BytesPerSecondSpeed)}/s  -  " +

            //                  $"[{CalcMemoryMensurableUnit(e.BytesReceived)} of {CalcMemoryMensurableUnit(e.TotalBytesToReceive)}], {estimateTime} {timeLeftUnit} left";

           var m = new MessageDialog((e.ProgressPercentage / 100).ToString());
              await m.ShowAsync();

          }



          public static string CalcMemoryMensurableUnit(long bigUnSignedNumber, bool isShort = true)

          {

              var kb = bigUnSignedNumber / 1024; // · 1024 Bytes = 1 Kilobyte 

              var mb = kb / 1024; // · 1024 Kilobytes = 1 Megabyte 

              var gb = mb / 1024; // · 1024 Megabytes = 1 Gigabyte 

              var tb = gb / 1024; // · 1024 Gigabytes = 1 Terabyte 



              var b = isShort ? "B" : "Bytes";

              var k = isShort ? "KB" : "Kilobytes";

              var m = isShort ? "MB" : "Megabytes";

              var g = isShort ? "GB" : "Gigabytes";

              var t = isShort ? "TB" : "Terabytes";



              return tb > 1 ? $"{tb:N0}{t}" :

                     gb > 1 ? $"{gb:N0}{g}" :

                     mb > 1 ? $"{mb:N0}{m}" :

                     kb > 1 ? $"{kb:N0}{k}" :

                     $"{bigUnSignedNumber:N0}{b}";

          }*/
    }
}
