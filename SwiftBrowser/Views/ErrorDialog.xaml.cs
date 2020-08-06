using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        public ErrorDialog()
        {
            InitializeComponent();
            errormsg.Text = "Message: " + errormsgStatic;
            errormsgExeption.Text = "Exception: " + errormsgExeptionStatic;
        }

        public static string errormsgStatic { get; set; }
        public static string errormsgExeptionStatic { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
