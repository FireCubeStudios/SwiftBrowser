using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Extensions.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionsWebView : Page
    {
        public static string w;

        public ExtensionsWebView()
        {
            InitializeComponent();
            // var m = new MessageDialog(w);
            //    m.ShowAsync();
            //  webview.Navigate(new Uri(w));
            w = null;
        }
    }
}
