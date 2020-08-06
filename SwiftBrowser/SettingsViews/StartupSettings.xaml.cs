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
    public sealed partial class StartupSettings : Page
    {
        private readonly Compositor _compositor;

        private int _count;

        private double _gearDimension = 87;

        private ScalarKeyFrameAnimation _gearMotionScalarAnimation;

        private List<Visual> _gearVisuals;

        private double _x = 87, _y, _width = 100, _height = 100;

        public StartupSettings()
        {
            InitializeComponent();
            _compositor = ElementCompositionPreview.GetElementVisual(this)?.Compositor;
            Setup();
            if ((int) ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 1)
                Option3RadioButton.IsChecked = true;
            else if ((int) ApplicationData.Current.LocalSettings.Values["RestoreSettings"] == 2)
                Option2RadioButton.IsChecked = true;
            else
                Option1RadioButton.IsChecked = true;
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

        private void Option1RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 3;
        }

        private void Option2RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 2;
        }

        private void Option3RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["RestoreSettings"] = 1;
        }
    }
}
