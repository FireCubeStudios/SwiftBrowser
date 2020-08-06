using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.SettingsViews
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebSettings : Page
    {
        private readonly Compositor _compositor;

        private int _count;

        private double _gearDimension = 87;

        private ScalarKeyFrameAnimation _gearMotionScalarAnimation;

        private List<Visual> _gearVisuals;

        private double _x = 87, _y, _width = 100, _height = 100;

        public WebSettings()
        {
            InitializeComponent();
            I.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["IndexDB"];
            E.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["Javascript"];
            switch ((string) ApplicationData.Current.LocalSettings.Values["SearchEngine"])
            {
                case "https://www.ecosia.org/search?q=":
                    se.SelectedIndex = 0;
                    break;
                case "https://www.google.com/search?q=":
                    se.SelectedIndex = 2;
                    break;
                case "https://www.bing.com/search?q=":
                    se.SelectedIndex = 3;
                    break;
                case "http://www.baidu.com/s?wd=":
                    se.SelectedIndex = 5;
                    break;
                case "https://www.yandex.com/search/?text=":
                    se.SelectedIndex = 4;
                    break;
                case "https://duckduckgo.com/?q=":
                    se.SelectedIndex = 6;
                    break;
                case "https://search.yahoo.com/search?p=":
                    se.SelectedIndex = 7;
                    break;
                case "https://en.wikipedia.org/w/index.php?search=":
                    se.SelectedIndex = 1;
                    break;
            }

            switch ((string) ApplicationData.Current.LocalSettings.Values["UserAgent"])
            {
                case "Default":
                    UserAgent.SelectedIndex = 0;
                    break;
                case
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36"
                    :
                    UserAgent.SelectedIndex = 2;
                    break;
                case "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0":
                    UserAgent.SelectedIndex = 1;
                    break;
                case
                    "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36"
                    :
                    UserAgent.SelectedIndex = 3;
                    break;
                case
                    "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/69.0.3497.105 Mobile/15E148 Safari/605.1"
                    :
                    UserAgent.SelectedIndex = 4;
                    break;
                case
                    "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.1058"
                    :
                    UserAgent.SelectedIndex = 5;
                    break;
                case
                    "Mozilla/5.0 (Linux; Android 7.0; SM-T827R4 Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.116 Safari/537.36"
                    :
                    UserAgent.SelectedIndex = 6;
                    break;
                case
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9"
                    :
                    UserAgent.SelectedIndex = 7;
                    break;
                case
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36"
                    :
                    UserAgent.SelectedIndex = 8;
                    break;
                case
                    "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36"
                    :
                    UserAgent.SelectedIndex = 9;
                    break;
                case "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1":
                    UserAgent.SelectedIndex = 10;
                    break;
                case
                    "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10586"
                    :
                    UserAgent.SelectedIndex = 11;
                    break;
            }

            _compositor = ElementCompositionPreview.GetElementVisual(this)?.Compositor;
            Setup();
        }

        public int Count

        {
            get => _count;

            set

            {
                _count = value;

                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Setup()

        {
            var firstGearVisual = ElementCompositionPreview.GetElementVisual(FirstGear);

            firstGearVisual.Size = new Vector2((float) FirstGear.ActualWidth, (float) FirstGear.ActualHeight);

            firstGearVisual.AnchorPoint = new Vector2(0.5f, 0.5f);


            for (var i = Container.Children.Count - 1; i > 0; i--) Container.Children.RemoveAt(i);


            _x = 87;

            _y = 0d;

            _width = 100;

            _height = 100;

            _gearDimension = 87;


            Count = 1;

            _gearVisuals = new List<Visual> {firstGearVisual};
            var bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/Gear.png"));

            var image = new Image

            {
                Source = bitmapImage,

                Width = _width,

                Height = _height,

                RenderTransformOrigin = new Point(0.5, 0.5)
            };


            // Set the coordinates of where the image should be

            Canvas.SetLeft(image, _x);

            Canvas.SetTop(image, _y);


            PerformLayoutCalculation();


            // Add the gear to the container

            Container.Children.Add(image);


            // Add a gear visual to the screen

            var gearVisual = AddGear(image);


            ConfigureGearAnimation(_gearVisuals[_gearVisuals.Count - 1], _gearVisuals[_gearVisuals.Count - 2]);
            await Task.Delay(1000);
            StartGearMotor(5);
        }

        private Visual AddGear(Image gear)

        {
            // Create a visual based on the XAML object

            var visual = ElementCompositionPreview.GetElementVisual(gear);

            visual.Size = new Vector2((float) gear.ActualWidth, (float) gear.ActualHeight);

            visual.AnchorPoint = new Vector2(0.5f, 0.5f);

            _gearVisuals.Add(visual);


            Count++;


            return visual;
        }

        private void StartGearMotor(double secondsPerRotation)

        {
            // Start the first gear (the red one)

            if (_gearMotionScalarAnimation == null)

            {
                _gearMotionScalarAnimation = _compositor.CreateScalarKeyFrameAnimation();

                var linear = _compositor.CreateLinearEasingFunction();


                var startingValue = ExpressionValues.StartingValue.CreateScalarStartingValue();

                _gearMotionScalarAnimation.InsertExpressionKeyFrame(0.0f, startingValue);

                _gearMotionScalarAnimation.InsertExpressionKeyFrame(1.0f, startingValue + 360f, linear);


                _gearMotionScalarAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            }


            _gearMotionScalarAnimation.Duration = TimeSpan.FromSeconds(secondsPerRotation);

            _gearVisuals.First().StartAnimation("RotationAngleInDegrees", _gearMotionScalarAnimation);
        }


        private void ConfigureGearAnimation(Visual currentGear, Visual previousGear)

        {
            // If rotation expression is null then create an expression of a gear rotating the opposite direction


            var rotateExpression = -previousGear.GetReference().RotationAngleInDegrees;


            // Start the animation based on the Rotation Angle in Degrees.

            currentGear.StartAnimation("RotationAngleInDegrees", rotateExpression);
        }


        private void PerformLayoutCalculation()

        {
            if (
                _x + Container.Margin.Left + _width > Container.ActualWidth && _gearDimension > 0 ||
                _x < Container.Margin.Left && _gearDimension < 0)

            {
                if (_gearDimension < 0)
                    _y -= _gearDimension;

                else
                    _y += _gearDimension;

                _gearDimension = -_gearDimension;
            }

            else

            {
                _x += _gearDimension;
            }
        }


        private void RaisePropertyChanged([CallerMemberName] string property = "")

        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void UserAgent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = sender as ComboBox;
            switch (c.SelectedItem.ToString())
            {
                case "Default":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] = "Default";
                    break;
                case "Google":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";
                    break;
                case "Firefox":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
                    break;
                case "Android (galaxy s9)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36";
                    break;
                case "Iphone (iphone XS chrome)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/69.0.3497.105 Mobile/15E148 Safari/605.1";
                    break;
                case "Windows phone (lumia 950)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.1058";
                    break;
                case "Tablet (samsung tab s3)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Linux; Android 7.0; SM-T827R4 Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.116 Safari/537.36";
                    break;
                case "Desktop (Macos safari)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9";
                    break;
                case "Desktop (Windows 7 chrome)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36";
                    break;
                case "Desktop (chromeos chrome)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36";
                    break;
                case "Desktop (linux firefox)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
                    break;
                case "Console (xbox one)":
                    ApplicationData.Current.LocalSettings.Values["UserAgent"] =
                        "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10586";
                    break;
            }
        }

        private void I_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["IndexDB"] = I.IsOn;
        }

        private void E_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["Javascript"] = E.IsOn;
        }

        private void Se_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = sender as ComboBox;
            switch (c.SelectedItem.ToString())
            {
                case "Ecosia":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.ecosia.org/search?q=";
                    break;
                case "Google":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.google.com/search?q=";
                    break;
                case "Bing":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://www.bing.com/search?q=";
                    break;
                case "Baidu":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "http://www.baidu.com/s?wd=";
                    break;
                case "DuckDuckGo":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://duckduckgo.com/?q=";
                    break;
                case "Yandex":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] =
                        "https://www.yandex.com/search/?text=";
                    break;
                case "Yahoo":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] = "https://search.yahoo.com/search?p=";
                    break;
                case "Wikipedia":
                    ApplicationData.Current.LocalSettings.Values["SearchEngine"] =
                        "https://en.wikipedia.org/w/index.php?search=";
                    break;
            }
        }
    }
}
