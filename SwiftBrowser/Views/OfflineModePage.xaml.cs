using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OfflineModePage : Page
    {
        public OfflineModePage()
        {
            InitializeComponent();
            OiMage = OfflineImage;
        }

        public static Image OiMage { get; set; }
    }
}
