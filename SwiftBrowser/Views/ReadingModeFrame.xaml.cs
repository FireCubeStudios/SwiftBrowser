using HtmlAgilityPack;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
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
    public sealed partial class ReadingModeFrame : Page
    {
        public static string TitleString { get; set; }
       MediaElement ReadAloudElement = new MediaElement();
        public static string ImageString { get; set; }
        public static string BodyString { get; set; }
        public List<Tuple<string, FontFamily>> Fonts { get; } = new List<Tuple<string, FontFamily>>()

            {

                new Tuple<string, FontFamily>("Arial", new FontFamily("Arial")),

                new Tuple<string, FontFamily>("Comic Sans MS", new FontFamily("Comic Sans MS")),

                new Tuple<string, FontFamily>("Courier New", new FontFamily("Courier New")),

                new Tuple<string, FontFamily>("Segoe UI", new FontFamily("Segoe UI")),

                new Tuple<string, FontFamily>("Times New Roman", new FontFamily("Times New Roman"))

            };
        public List<double> FontSizes { get; } = new List<double>()

            {

                8,

                9,

                10,

                11,

                12,

                14,

                16,

                18,

                20,

                24,

                28,

                36,

                48,

                72

            };
        public ReadingModeFrame()
        {
            this.InitializeComponent();
            Title.Text = TitleString;
            // ImageArticle.Source = new
            RichBodyText.Text = BodyString;
        }

         private async void ReadAloudButton_Click(object sender, RoutedEventArgs e)
     {
         var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
         SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(RichBodyText.Text);

         // Send the stream to the media object.
         ReadAloudElement.SetSource(stream, stream.ContentType);
         ReadAloudElement.Play();
         ReadTip.IsOpen = true;
     }



     private void ReadTip_CloseButtonClick(TeachingTip sender, object args)
     {
         ReadAloudElement.Stop();
     }

        private void ReadingSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            if (toggle.IsOn == true)
            {
                ReadAloudElement.Play();
            }
            else
            {
                ReadAloudElement.Pause();
            }
        }
        private void Combo3_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)

        {

            bool isDouble = double.TryParse(sender.Text, out double newValue);



            // Set the selected item if:

            // - The value successfully parsed to double AND

            // - The value is in the list of sizes OR is a custom value between 8 and 100

            if (isDouble && (FontSizes.Contains(newValue) || (newValue < 100 && newValue > 8)))

            {

                // Update the SelectedItem to the new value. 

                sender.SelectedItem = newValue;

            }

            else

            {

                // If the item is invalid, reject it and revert the text. 

                sender.Text = sender.SelectedValue.ToString();



                var dialog = new ContentDialog();

                dialog.Content = "The font size must be a number between 8 and 100.";

                dialog.CloseButtonText = "Close";

                dialog.DefaultButton = ContentDialogButton.Close;

                var task = dialog.ShowAsync();

            }



            // Mark the event as handled so the framework doesn’t update the selected item automatically. 

            args.Handled = true;

        }

        private void Combo2_Loaded(object sender, RoutedEventArgs e)
        {
            Combo2.SelectedIndex = 3;
            Combo3.SelectedIndex = 8;
        }
    }
}
