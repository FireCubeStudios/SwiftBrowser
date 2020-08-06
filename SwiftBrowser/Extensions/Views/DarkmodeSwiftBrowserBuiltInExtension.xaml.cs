using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Extensions.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DarkmodeSwiftBrowserBuiltInExtension : Page
    {
        private readonly DarkMode DarkMode = new DarkMode();
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public DarkmodeSwiftBrowserBuiltInExtension()
        {
            InitializeComponent();
            if ((bool) localSettings.Values["DarkMode"])
                Tog.IsOn = true;
            else
                Tog.IsOn = false;
        }

        public static WebView w { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DarkMode.DarkMode_Click(w);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var T = sender as ToggleSwitch;
            if (T.IsOn)
            {
                DarkMode.DarkMode_Click(w);
                localSettings.Values["DarkMode"] = true;
            }
            else
            {
                localSettings.Values["DarkMode"] = false;
            }
        }
    }
}
