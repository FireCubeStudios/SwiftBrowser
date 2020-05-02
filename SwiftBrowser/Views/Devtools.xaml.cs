using SwiftBrowser_Runtime_Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Devtools : Page
    {
        public static WebView DevView { get; set; }
        public WebView WebVie;
        public Devtools()
        {
            this.InitializeComponent();
            WebVie = DevView;
            System.Timers.Timer RightTimer = new System.Timers.Timer();
            RightTimer.Elapsed += new ElapsedEventHandler(WebView_Log);
            RightTimer.Interval = 50;
            RightTimer.Enabled = true;
            StartUp();
        }
        private void WebView_Log(object source, ElapsedEventArgs e)
        {
            if (String.IsNullOrEmpty(ConsoleLog.LogString) == false)
            {
                lOG.Text = lOG.Text + "\n" + ConsoleLog.LogString;
                ConsoleLog.LogString = null;
            }
        }
        public async void StartUp()
        {     
            string html = await WebVie.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            HTMLeditor.Text = html;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // await WebVie.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML = " + HTMLeditor.Text + ";" });
            WebVie.NavigateToString(HTMLeditor.Text);
        }
    }
}
