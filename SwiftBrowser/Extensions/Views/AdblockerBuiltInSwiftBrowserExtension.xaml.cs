using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Extensions.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdblockerBuiltInSwiftBrowserExtension : Page
    {
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private WebView webview;

        public AdblockerBuiltInSwiftBrowserExtension()
        {
            InitializeComponent();
            w = webview;
            if ((bool) localSettings.Values["AdBlocker"])
                Tog.IsOn = true;
            else
                Tog.IsOn = false;
        }

        public static WebView w { get; set; }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var T = sender as ToggleSwitch;
            if (T.IsOn)
                localSettings.Values["AdBlocker"] = true;
            else
                localSettings.Values["AdBlocker"] = false;
        }
    }
}
