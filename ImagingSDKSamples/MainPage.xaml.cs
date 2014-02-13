using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using Nokia.Graphics.Imaging;
using System.Windows.Media.Imaging;

namespace ImagingSDKSamples
{
    public partial class MainPage
    {
        // Used when needed a second image for the filter
        private static readonly ColorImageSource ColorSource = new ColorImageSource(new Windows.Foundation.Size(640, 480),
            Windows.UI.Color.FromArgb(255, 255, 0, 0));

        // List of filters and their constructors
        private readonly List<FilterListObject> _filterList = new List<FilterListObject>
        {
            new FilterListObject { Name="No filter"},
            new FilterListObject { Name="AntiqueFilter"},
            new FilterListObject { Name="AutoEnhanceFilter"},
            new FilterListObject { Name="AutoLevelsFilter"},
            new FilterListObject { Name="BlendFilter", Constructor = new object[]{ColorSource, BlendFunction.Colorburn, 0.8}},
            new FilterListObject { Name="BlurFilter"},
            new FilterListObject { Name="BrightnessFilter"},
            new FilterListObject { Name="CartoonFilter"},
            new FilterListObject { Name="ChromaKeyFilter"},
            new FilterListObject { Name="ColorAdjustFilter"},
            new FilterListObject { Name="ColorBoostFilter"},
            new FilterListObject { Name="ColorSwapFilter"},
            new FilterListObject { Name="ColorizationFilter"},
            new FilterListObject { Name="ContrastFilter"},
            new FilterListObject { Name="CropFilter"},
            new FilterListObject { Name="CurvesFilter"},
            new FilterListObject { Name="DespeckleFilter"},
            new FilterListObject { Name="EmbossFilter"},
            new FilterListObject { Name="ExposureFilter"},
            new FilterListObject { Name="FlipFilter"}, 
            new FilterListObject { Name="FogFilter"},
            new FilterListObject { Name="FoundationFilter"},
            new FilterListObject { Name="GrayscaleFilter"},
            new FilterListObject { Name="GrayscaleNegativeFilter"},
            new FilterListObject { Name="HueSaturationFilter"},
            new FilterListObject { Name="ImageFusionFilter"},
            new FilterListObject { Name="LevelsFilter"},
            new FilterListObject { Name="LocalBoostAutomaticFilter"},
            new FilterListObject { Name="LocalBoostFilter"},
            new FilterListObject { Name="LomoFilter"},
            new FilterListObject { Name="MagicPenFilter"},
            new FilterListObject { Name="MilkyFilter"},
            new FilterListObject { Name="MilkyFilter"},
            new FilterListObject { Name="MirrorFilter"},
            new FilterListObject { Name="MonoColorFilter"},
            new FilterListObject { Name="MoonlightFilter"},
            new FilterListObject { Name="NegativeFilter"},
            new FilterListObject { Name="NoiseFilter"},
            new FilterListObject { Name="OilyFilter"},
            new FilterListObject { Name="PaintFilter"},
            new FilterListObject { Name="PosterizeFilter"},
            new FilterListObject { Name="ReframingFilter"},
            new FilterListObject { Name="RotationFilter"},
            new FilterListObject { Name="SepiaFilter"},
            new FilterListObject { Name="SharpnessFilter"},
            new FilterListObject { Name="SketchFilter"},
            new FilterListObject { Name="SolarizeFilter"},
            new FilterListObject { Name="SplitToneFilter"},
            new FilterListObject { Name="SpotlightFilter"},
            new FilterListObject { Name="StampFilter"},
            new FilterListObject { Name="TemperatureAndTintFilter"},
            new FilterListObject { Name="VignettingFilter"},
            new FilterListObject { Name="WarpFilter"},
            new FilterListObject { Name="WatercolorFilter"},
            new FilterListObject { Name="WhiteBalanceFilter"},
            new FilterListObject { Name="WhiteboardEnhancementFilter"}
        };
        
        private bool _rendering; // Do not start rendering if we haven't finished previous rendering yet

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FilterBox.DataContext = _filterList;
        }

        /// <summary>
        /// Render the photo to screen without any filter at first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void CaptureTask_Completed(object sender, PhotoResult e)
        {
            App.ChosenPhoto = e.ChosenPhoto;

            if (FilterBox.SelectedIndex == 0)
            {
                await RenderPlainPhoto();
            }
            else
            {
                FilterChanged(null, null);
            }

            FilterBox.IsEnabled = true;
        }

        private async Task RenderPlainPhoto()
        {
            using (var source = new StreamImageSource(App.ChosenPhoto))
            {
                // Create a target where the filtered image will be rendered to
                var target = new WriteableBitmap((int) ImageControl.ActualWidth, (int) ImageControl.ActualHeight);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(source, target))
                {
                    // Render the image with the filter(s)
                    await renderer.RenderAsync();

                    // Set the output image to Image control as a source
                    ImageControl.Source = target;
                    App.ChosenPhoto.Position = 0;

                }
            }
        }

        /// <summary>
        /// User selects a new filter from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FilterChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get the name of the filter
            var selectedFilter = FilterBox.Items[FilterBox.SelectedIndex] as FilterListObject;

            if (selectedFilter == null || App.ChosenPhoto == null)
            {
                return;
            }

            if (string.Compare(selectedFilter.Name, "No filter", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                await RenderPlainPhoto();
            }
            else
            {
                // Format the fully qualified name of the class
                var type = string.Format(
                    "Nokia.Graphics.Imaging.{0}, Nokia.Graphics.Imaging, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime",
                    selectedFilter.Name);

                // Use reflection to create the filter class
                var sampleEffect = Type.GetType(type);
                if (sampleEffect != null)
                {
                    var filter = (IFilter) Activator.CreateInstance(sampleEffect, selectedFilter.Constructor);

                    ApplyBasicFilter(filter);
                }
            }
        }

        /// <summary>
        /// Apply the chosen filter
        /// </summary>
        /// <param name="sampleEffect"></param>
        private async void ApplyBasicFilter(IFilter sampleEffect)
        {
            if (App.ChosenPhoto == null || _rendering)
            {
                return;
            }
            _rendering = true;

            // Create a source to read the image from PhotoResult stream
            using (var source = new StreamImageSource(App.ChosenPhoto))
            using (var filters = new FilterEffect(source))
            {
                filters.Filters = new[] { sampleEffect };

                // Create a target where the filtered image will be rendered to
                var target = new WriteableBitmap((int) ImageControl.ActualWidth, (int) ImageControl.ActualHeight);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(filters, target))
                {
                    // Render the image with the filter
                    await renderer.RenderAsync();

                    // Set the output image to Image control as a source
                    ImageControl.Source = target;

                    App.ChosenPhoto.Position = 0;
                }
            }
            _rendering = false;
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            var task = new CameraCaptureTask();
            task.Completed += CaptureTask_Completed;
            task.Show();
        }
    }
}

