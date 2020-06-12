using flowpad.Services.Ink;
using Flowpad.Services.Ink.UndoRedo;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SwiftBrowser.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class inkingPage : Page
    {
        public static WebView WebView { get; set; }
        int width;
        int height;
        private bool lassoSelectionButtonIsChecked;
        /* private bool touchInkingButtonIsChecked = true;
         private bool mouseInkingButtonIsChecked = true;
         private bool penInkingButtonIsChecked = true;*/
        private bool transformTextAndShapesButtonIsEnabled;
        private bool undoButtonIsEnabled;
        /*private bool redoButtonIsEnabled;
        private bool saveInkFileButtonIsEnabled;
        private bool clearAllButtonIsEnabled;*/
        private InkStrokesService strokeService;
        private InkCopyPasteService copyPasteService;
        private InkLassoSelectionService lassoSelectionService;
        private bool cutButtonIsEnabled;
        private bool copyButtonIsEnabled;
        private bool pasteButtonIsEnabled;
        private InkNodeSelectionService nodeSelectionService;
        private InkPointerDeviceService pointerDeviceService;
        private InkUndoRedoService undoRedoService;
        private InkTransformService transformService;
        private InkFileService fileService;
        private InkZoomService zoomService;
        public string FileOpen;
        private StorageFile imageFile;
        public Boolean FileSaved = false;
        public PrintHelper printHelper;
        public inkingPage()
        {
            this.InitializeComponent();
            strokeService = new InkStrokesService(InkCanvas.InkPresenter);
            var analyzer = new InkAsyncAnalyzer(InkCanvas, strokeService);
            var selectionRectangleService = new InkSelectionRectangleService(InkCanvas, selectionCanvas, strokeService);

            lassoSelectionService = new InkLassoSelectionService(InkCanvas, selectionCanvas, strokeService, selectionRectangleService);
            copyPasteService = new InkCopyPasteService(strokeService);
            nodeSelectionService = new InkNodeSelectionService(InkCanvas, selectionCanvas, analyzer, strokeService, selectionRectangleService);
            pointerDeviceService = new InkPointerDeviceService(InkCanvas);
            undoRedoService = new InkUndoRedoService(InkCanvas, strokeService);
            transformService = new InkTransformService(drawingCanvas, strokeService);
            fileService = new InkFileService(InkCanvas, strokeService);
            zoomService = new InkZoomService(canvasScroll);
            MouseInkingButton.IsChecked = true;
            TouchInkingButton.IsChecked = true;
            InkSurfacePen.IsChecked = true;
            strokeService.CopyStrokesEvent += (s, e) => RefreshEnabledButtons();
            strokeService.SelectStrokesEvent += (s, e) => RefreshEnabledButtons();
            strokeService.ClearStrokesEvent += (s, e) => RefreshEnabledButtons();
            strokeService.ClearStrokesEvent += (s, e) => RefreshEnabledButtons();
            undoRedoService.UndoEvent += (s, e) => RefreshEnabledButtons();
            undoRedoService.RedoEvent += (s, e) => RefreshEnabledButtons();
            undoRedoService.AddUndoOperationEvent += (s, e) => RefreshEnabledButtons();
            CaptureWebView();
        }
        private async void CaptureWebView()
        {
           
            var originalWidth = WebView.ActualWidth;
            var originalHeight = WebView.ActualHeight;

            var widthString = await WebView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            var heightString = await WebView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            if (!int.TryParse(widthString, out width))
            {
                throw new Exception("Unable to get page width");
            }
            if (!int.TryParse(heightString, out height))
            {
                throw new Exception("Unable to get page height");
            }

            // resize the webview to the content
       // WebView.Width = width;
            WebView.Height = height;
            Painter.Width = width;
            Painter.Height = height;
            InkCanvas.Width = width;
            InkCanvas.Height = height;
            Gridx.Height = height;
            Gridx.Width = width;
            Painter.Source = WebView.Source;
            WebView.Height = 1000;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(Gridx);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });
            StorageFile file = await picker.PickSaveFileAsync();
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)rtb.PixelWidth,
                                     (uint)rtb.PixelHeight,
                                      displayInformation.RawDpiX,
                         displayInformation.RawDpiY,
                                     pixels);
                await encoder.FlushAsync();
            }
            int duration = 3000;
            try { 
            TabViewPage.InAppNotificationMain.Show("Image saved", duration);
            }
            catch
            {
                IncognitoTabView.InAppNotificationMain.Show("Image saved", duration);
            }
        }


        public bool TransformTextAndShapesButtonIsEnabled
        {
            get => transformTextAndShapesButtonIsEnabled;
            set => Set(ref transformTextAndShapesButtonIsEnabled, value);
        }


        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public bool CutButtonIsEnabled
        {
            get => cutButtonIsEnabled;
            set => Set(ref cutButtonIsEnabled, value);
        }

        public bool CopyButtonIsEnabled
        {
            get => copyButtonIsEnabled;
            set => Set(ref copyButtonIsEnabled, value);
        }

        public bool PasteButtonIsEnabled
        {
            get => pasteButtonIsEnabled;
            set => Set(ref pasteButtonIsEnabled, value);
        }
        private void Copy_Click(object sender, RoutedEventArgs e) => copyPasteService?.Copy();

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            copyPasteService?.Cut();
            ClearSelection();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            copyPasteService?.Paste();
            ClearSelection();
        }
        private void ConfigLassoSelection(bool enableLasso)
        {
            if (enableLasso)
            {
                lassoSelectionService?.StartLassoSelectionConfig();
            }
            else
            {
                lassoSelectionService?.EndLassoSelectionConfig();
            }
        }
        public bool LassoSelectionButtonIsChecked
        {
            get => lassoSelectionButtonIsChecked;
            set
            {
                Set(ref lassoSelectionButtonIsChecked, value);
                ConfigLassoSelection(value);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void ZoomIn_Click(object sender, RoutedEventArgs e) => zoomService?.ZoomIn();

        private void ZoomOut_Click(object sender, RoutedEventArgs e) => zoomService?.ZoomOut();

        private void ResetZoom_Click(object sender, RoutedEventArgs e) => zoomService?.ResetZoom();

        private void FitToScreen_Click(object sender, RoutedEventArgs e) => zoomService?.FitToScreen();
        private void OnInkToolbarLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is InkToolbar inkToolbar)
            {
                inkToolbar.TargetInkCanvas = InkCanvas;
                InkToolbarToolButton penButton = inkToolbar.GetToolButton(InkToolbarTool.BallpointPen);
                inkToolbar.ActiveTool = penButton;
            }
        }

        private void SetCanvasSize()
        {
            InkCanvas.Width = Math.Max(canvasScroll.ViewportWidth, 1000);
            InkCanvas.Height = Math.Max(canvasScroll.ViewportHeight, 1000);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
            undoRedoService?.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            // if (InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Any() == true)
            // {
            ClearSelection();
            undoRedoService?.Redo();
            //}

        }
        private void InkToolbarCustomToggleButtonPen_Click(object sender, RoutedEventArgs e)
        {
            if (InkSurfacePen.IsChecked == true)
            {
                InkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Pen;
            }
            else
            {
                InkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Pen;
            }
        }
        private void InkToolbarCustomToggleButtonTouch_Click(object sender, RoutedEventArgs e)
        {
            if (TouchInkingButton.IsChecked == true)
            {
                InkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Touch;
            }
            else
            {
                InkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Touch;
            }
        }
        private void InkToolbarCustomToggleButtonMouse_Click(object sender, RoutedEventArgs e)
        {
            if (MouseInkingButton.IsChecked == true)
            {
                InkCanvas.InkPresenter.InputDeviceTypes |= CoreInputDeviceTypes.Mouse;
            }
            else
            {
                InkCanvas.InkPresenter.InputDeviceTypes &= ~CoreInputDeviceTypes.Mouse;
            }
        }
        private void MyColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            InkDrawingAttributes drawingAttributes = InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();

            // Assign the selected color to a variable to use outside the popup.
            Windows.UI.Color mycolor = myColorPicker.Color;
            drawingAttributes.Color = mycolor;
            InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

        }
        private async void LoadInkFile_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
            var fileLoaded = await fileService.LoadInkAsync();

            if (fileLoaded)
            {
                transformService.ClearTextAndShapes();
                undoRedoService.Reset();
            }
        }
        private void TiltSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InkCanvas.InkPresenter != null)

                {
                    var drawingAttributes = InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                    ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                    if (toggleSwitch != null)
                    {
                        if (toggleSwitch.IsOn == true)
                        {
                            drawingAttributes.IgnoreTilt = false;
                        }
                        else
                        {
                            drawingAttributes.IgnoreTilt = true;
                        }
                        InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
                    }
                }
            }
            catch
            {

            }
        }
        private void PressureSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InkCanvas.InkPresenter != null)

                {
                    var drawingAttributes = InkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                    ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                    if (toggleSwitch != null)
                    {
                        if (toggleSwitch.IsOn == true)
                        {
                            drawingAttributes.IgnorePressure = false;
                        }
                        else
                        {
                            drawingAttributes.IgnorePressure = true;

                        }
                        InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
                    }
                }
            }
            catch
            {

            }
        }
        private void InkSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AutoSaveTip.IsOpen = true;
        }
        private async void SaveInkFile_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
            await fileService.SaveInkAsync();
        }

        private async void TransformTextAndShapes_Click(object sender, RoutedEventArgs e)
        {
            var result = await transformService.TransformTextAndShapesAsync();
            if (result.TextAndShapes.Any())
            {
                ClearSelection();
                undoRedoService.AddOperation(new TransformUndoRedoOperation(result, strokeService));
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {

            var strokes = strokeService?.GetStrokes().ToList();
            var textAndShapes = transformService?.GetTextAndShapes().ToList();
            ClearSelection();
            strokeService.ClearStrokes();
            transformService.ClearTextAndShapes();
            undoRedoService.AddOperation(new ClearStrokesAndShapesUndoRedoOperation(strokes, textAndShapes, strokeService, transformService));
        }

        private void RefreshEnabledButtons()
        {

            if (InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Any() == true)
            {

                undoButtonIsEnabled = true;
                //TransformTextAndShapesButtonIsEnabled = strokeService.GetStrokes().Any();

                //strokeService.GetStrokes().Any() || transformService.HasTextAndShapes();
            }

        }

        private void Colorpick_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        /* private void ConfigLassoSelection(object sender, RoutedEventArgs e)
         {
             if (1 == 1)
             {
                 lassoSelectionService?.StartLassoSelectionConfig();
             }
             else
             {
                 lassoSelectionService?.EndLassoSelectionConfig();
             }
         }*/

        private void ClearSelection()
        {
            nodeSelectionService.ClearSelection();
            lassoSelectionService.ClearSelection();
        }

    }
}

