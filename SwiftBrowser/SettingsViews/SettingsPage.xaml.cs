using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Newtonsoft.Json;
using SwiftBrowser.Helpers;
using SwiftBrowser.Services;

namespace SwiftBrowser.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private readonly Compositor _compositor;

        private int _count;
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        private double _gearDimension = 87;

        private ScalarKeyFrameAnimation _gearMotionScalarAnimation;

        private List<Visual> _gearVisuals;

        private double _x = 87, _y, _width = 100, _height = 100;
        private ExtensionsClass ExtensionsListJSONJSON;
        private readonly StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        public SettingsPage()
        {
            InitializeComponent();
            try
            {
                tICO.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                TfAV.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeFav"];
                TqUI.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomePin"];
                TmOR.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeMore"];
                TSea.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"];
            }
            catch
            {
                ApplicationData.Current.LocalSettings.Values["HomeIcon"] = true;
                ApplicationData.Current.LocalSettings.Values["HomeFav"] = true;
                ApplicationData.Current.LocalSettings.Values["HomeSearch"] = true;
                ApplicationData.Current.LocalSettings.Values["HomePin"] = true;
                ApplicationData.Current.LocalSettings.Values["HomeMore"] = true;
                tICO.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeIcon"];
                TfAV.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeFav"];
                TqUI.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomePin"];
                TmOR.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeMore"];
                TSea.IsOn = (bool) ApplicationData.Current.LocalSettings.Values["HomeSearch"];
            }

            try
            {
                if ((bool) ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"])
                    Imageoption.IsChecked = true;
                else
                    DefaultacrylicOption.IsChecked = true;
            }
            catch
            {
                DefaultacrylicOption.IsChecked = true;
                ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
            }

            if ((bool) ApplicationData.Current.LocalSettings.Values["CustomUrlBool"])
                Option2RadioButton.IsChecked = true;
            else
                Option1RadioButton.IsChecked = true;
            _compositor = ElementCompositionPreview.GetElementVisual(this)?.Compositor;
            Setup();
            LoadItems();
        }

        public static ExtensionsStore methods { get; set; }

        public ElementTheme ElementTheme
        {
            get => _elementTheme;

            set => Set(ref _elementTheme, value);
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

        private void DefaultacrylicOption_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = "";
        }

        private async void Imageoption_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = Imageoption.IsChecked;
            if ((string) ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] == "")
            {
                try
                {
                    var picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    picker.FileTypeFilter.Add(".jpg");
                    picker.FileTypeFilter.Add(".jpeg");
                    picker.FileTypeFilter.Add(".png");

                    var file = await picker.PickSingleFileAsync();
                    var File = await file.CopyAsync(localFolder);
                    var bitmapImage = new BitmapImage(); // dimension, so long as one dimension measurement is provided
                    bitmapImage.UriSource = new Uri(File.Path);
                    ApplicationData.Current.LocalSettings.Values["CustomBackgroundPath"] = File.Path;
                    var duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("Saved", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("Saved", duration);
                    }
                }
                catch
                {
                    ApplicationData.Current.LocalSettings.Values["CustomBackgroundBool"] = false;
                    DefaultacrylicOption.IsChecked = true;
                    var duration = 3000;
                    try
                    {
                        TabViewPage.InAppNotificationMain.Show("Canceled", duration);
                    }
                    catch
                    {
                        IncognitoTabView.InAppNotificationMain.Show("Canceled", duration);
                    }
                }
            }
        }

        public async void LoadItems()
        {
            var ExtensionsListJSON = new List<ExtensionsJSON>();
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync("Extensions.json"); // error here
            var JSONData = "e";
            JSONData = await FileIO.ReadTextAsync(file);
            ExtensionsListJSONJSON = JsonConvert.DeserializeObject<ExtensionsClass>(JSONData);
            foreach (var item in ExtensionsListJSONJSON.Extensions)
                if (item.IsEnabledJSON)
                    ExtensionsListJSON.Add(new ExtensionsJSON
                    {
                        NameJSON = item.NameJSON,
                        DescriptionJSON = item.DescriptionJSON,
                        IconJSON = item.IconJSON,
                        IsEnabledJSON = item.IsEnabledJSON,
                        Page = item.Page,
                        IsToolbar = item.IsToolbar
                    });
            ExtensionsList.ItemsSource = ExtensionsListJSON;
        }

        public async void closing(object sender, RoutedEventArgs e)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync("Extensions.json");
            var SerializedObject = JsonConvert.SerializeObject(ExtensionsListJSONJSON, Formatting.Indented);

            await FileIO.WriteTextAsync(file, SerializedObject);
        }

        private void ToggleToolbar_Toggled(object sender, RoutedEventArgs e)
        {
            var ExtensionsListJSON = new List<ExtensionsJSON>();
            //   LoadingControl.IsLoading = true;
            var t = sender as ToggleSwitch;
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as ExtensionsJSON;
            var FoundItem = ExtensionsListJSONJSON.Extensions.Find(x => SenderPost.Id == SenderPost.Id);
            FoundItem.IsToolbar = t.IsOn;
            SenderPost.IsToolbar = t.IsOn;
            FoundItem.IsEnabledJSON = true;
            ExtensionsListJSONJSON.Extensions.Remove(FoundItem);
            ExtensionsListJSONJSON.Extensions.Add(FoundItem);
        }

        private void TICO_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeIcon"] = toggle.IsOn;
        }

        private void TfAV_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeFav"] = toggle.IsOn;
        }

        private void TqUI_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomePin"] = toggle.IsOn;
        }

        private void TmOR_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeMore"] = toggle.IsOn;
        }

        private void TSea_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values["HomeSearch"] = toggle.IsOn;
        }

        private void Option2RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Visible;
        }

        private void Option1RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TextUrl.Visibility = Visibility.Collapsed;
            ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
            ApplicationData.Current.LocalSettings.Values["CustomUrl"] = "";
        }

        private async void TextUrl_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text.StartsWith("Https://"))
            {
                ApplicationData.Current.LocalSettings.Values["CustomUrlBool"] = Option2RadioButton.IsChecked;
                ApplicationData.Current.LocalSettings.Values["CustomUrl"] = args.QueryText;
            }
            else
            {
                var duration = 3000;
                try
                {
                    TabViewPage.InAppNotificationMain.Show("Not a valid url!", duration);
                }
                catch
                {
                    IncognitoTabView.InAppNotificationMain.Show("Not a valid url!", duration);
                }
            }
        }


        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalizedSwift();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null) await ThemeSelectorService.SetThemeAsync((ElementTheme) param);
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ExtensionsClass
        {
            public List<ExtensionsJSON> Extensions { get; set; }
        }

        /*  public class Extensions
          {
              public string Header { get; set; }
              public string Url { get; set; }
              public string FavIcon { get; set; }
          }*/
        public class ExtensionsJSON
        {
            public string NameJSON { get; set; }
            public string DescriptionJSON { get; set; }
            public string IconJSON { get; set; }
            public int Id { get; set; }
            public bool IsEnabledJSON { get; set; }
            public bool IsIncognitoEnabled { get; set; }
            public bool IsToolbar { get; set; }
            public string Page { get; set; }
        }
    }
}
