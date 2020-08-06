using System;
using System.Timers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SwiftBrowser_Runtime_Component;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Devtools : Page
    {
        public WebView WebVie;

        public Devtools()
        {
            InitializeComponent();
            WebVie = DevView;
            var RightTimer = new Timer();
            RightTimer.Elapsed += WebView_Log;
            RightTimer.Interval = 50;
            RightTimer.Enabled = true;
            StartUp();
        }

        public static WebView DevView { get; set; }

        private void WebView_Log(object source, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConsoleLog.LogString) == false)
                //lOG.Text = lOG.Text + "\n" + ConsoleLog.LogString;
                ConsoleLog.LogString = null;
        }

        public async void StartUp()
        {
            var html = await WebVie.InvokeScriptAsync("eval", new[] {"document.documentElement.outerHTML;"});
            HTMLeditor.Text = html;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // await WebVie.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML = " + HTMLeditor.Text + ";" });
            WebVie.NavigateToString(HTMLeditor.Text);
        }
    }
}
