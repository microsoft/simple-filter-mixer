using System;
using System.Collections;
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
        private static readonly StorageFileImageSource MaskSource = InitializeFile().Result;
        // Used when needed a second image for the filter
        private static readonly ColorImageSource ColorSource = new ColorImageSource(new Windows.Foundation.Size(640, 480),
            Windows.UI.Color.FromArgb(255, 255, 0, 0));

        private static readonly List<SplitToneRange> SplitList = new List<SplitToneRange> {
                new SplitToneRange(100, 150, Windows.UI.Color.FromArgb(255, 155, 145, 138)),
                new SplitToneRange(160, 169, Windows.UI.Color.FromArgb(255, 155, 230, 142)),
                new SplitToneRange(170, 172, Windows.UI.Color.FromArgb(255, 155, 130, 49)),
                new SplitToneRange(175, 180, Windows.UI.Color.FromArgb(255, 255, 245, 238))
            };

        #region Filters
        // List of filters and their constructors
        private readonly List<FilterListObject> _filterList = new List<FilterListObject>
        {
            new FilterListObject { Name="AntiqueFilter"},
            new FilterListObject { Name="AutoEnhanceFilter", Constructor = new object[]{true, true}},
            new FilterListObject { Name="AutoLevelsFilter"},
            new FilterListObject { Name="BlendFilter", Constructor = new object[]{ColorSource, BlendFunction.Colorburn, 0.8}},
            new FilterListObject { Name="BlurFilter", Constructor = new object[]{100}},
            new FilterListObject { Name="BrightnessFilter", Constructor = new object[]{0.8}},
            new FilterListObject { Name="CartoonFilter", Constructor = new object[]{true}},
            new FilterListObject { Name="ChromaKeyFilter", Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 255, 255), 0.5}},
            new FilterListObject { Name="ColorAdjustFilter", Constructor = new object[]{0.1, -1, 0.5}},
            new FilterListObject { Name="ColorBoostFilter", Constructor = new object[]{15}},
            new FilterListObject { Name="ColorSwapFilter", Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0),
                Windows.UI.Color.FromArgb(255, 0, 255, 0), 0.2, true, true}},
            new FilterListObject { Name="ColorizationFilter", Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2, 0.1}},
            new FilterListObject { Name="ContrastFilter", Constructor = new object[]{-0.8}},
            new FilterListObject { Name="CropFilter", Constructor = new object[]{new Windows.Foundation.Rect(0, 0, 500, 500)}},
            new FilterListObject { Name="CurvesFilter"},
            new FilterListObject { Name="DespeckleFilter", Constructor = new object[]{DespeckleLevel.High}},
            new FilterListObject { Name="EmbossFilter", Constructor = new object[]{0.5}},
            new FilterListObject { Name="ExposureFilter", Constructor = new object[]{ExposureMode.Natural, 0.8}},
            new FilterListObject { Name="FlipFilter", Constructor = new object[]{FlipMode.Horizontal}}, 
            new FilterListObject { Name="FogFilter"},
            new FilterListObject { Name="FoundationFilter", Constructor = new object[]{new Windows.Foundation.Rect(0, 0, 700, 700)}},
            new FilterListObject { Name="GrayscaleFilter"},
            new FilterListObject { Name="GrayscaleNegativeFilter"},
            new FilterListObject { Name="HueSaturationFilter", Constructor = new object[]{0.7, 0.7}},
            new FilterListObject { Name="ImageFusionFilter", Constructor = new object[]{ColorSource, MaskSource, false}},
            new FilterListObject { Name="LevelsFilter", Constructor = new object[]{0.6, 0.1, 0.4}},
            new FilterListObject { Name="LocalBoostAutomaticFilter"},
            new FilterListObject { Name="LocalBoostFilter", Constructor = new object[]{3, 0.5, 0.5, 0.4}},
            new FilterListObject { Name="LomoFilter", Constructor = new object[]{0.7, 0.4, LomoVignetting.High, LomoStyle.Yellow}},
            new FilterListObject { Name="MagicPenFilter"},
            new FilterListObject { Name="MilkyFilter"},
            new FilterListObject { Name="MirrorFilter"},
            new FilterListObject { Name="MonoColorFilter", Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2}},
            new FilterListObject { Name="MoonlightFilter", Constructor = new object[]{23}},
            new FilterListObject { Name="NegativeFilter"},
            new FilterListObject { Name="NoiseFilter", Constructor = new object[]{NoiseLevel.Medium}},
            new FilterListObject { Name="OilyFilter"},
            new FilterListObject { Name="PaintFilter", Constructor = new object[]{2}},
            new FilterListObject { Name="PosterizeFilter", Constructor = new object[]{5}},
            new FilterListObject { Name="ReframingFilter", Constructor = new object[]{new Windows.Foundation.Rect(0,0,300,300), 30, new Windows.Foundation.Point(0.5,0.5)}},
            new FilterListObject { Name="RotationFilter", Constructor = new object[]{45}},
            new FilterListObject { Name="SepiaFilter"},
            new FilterListObject { Name="SharpnessFilter", Constructor = new object[]{7}},
            new FilterListObject { Name="SketchFilter", Constructor = new object[]{SketchMode.Color}},
            new FilterListObject { Name="SolarizeFilter", Constructor = new object[]{0.5}},
            new FilterListObject { Name="SplitToneFilter", Constructor = new object[]{SplitList}},
            new FilterListObject { Name="SpotlightFilter", Constructor = new object[]{new Windows.Foundation.Point(400, 300), 1200, 0.5}},
            new FilterListObject { Name="StampFilter", Constructor = new object[]{5, 0.7}},
            new FilterListObject { Name="TemperatureAndTintFilter", Constructor = new object[]{-0.5, 1}},
            new FilterListObject { Name="VignettingFilter", Constructor = new object[]{0.2, Windows.UI.Color.FromArgb(255, 10, 10, 10)}},
            new FilterListObject { Name="WarpFilter", Constructor = new object[]{WarpEffect.HappyFool, 0.7}},
            new FilterListObject { Name="WatercolorFilter", Constructor = new object[]{0.7, 0.1}},
            new FilterListObject { Name="WhiteBalanceFilter", Constructor = new object[]{WhitePointCalculationMode.Maximum}},
            new FilterListObject { Name="WhiteboardEnhancementFilter", Constructor = new object[]{WhiteboardEnhancementMode.Hard}}
        };
        #endregion

        private bool _rendering; // Do not start rendering if we haven't finished previous rendering yet

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            FilterBox.SummaryForSelectedItemsDelegate = MultiSelection;
        }

        /// <summary>
        /// Handle how to show the multiple selection in ListPicker
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        private string MultiSelection(IList selectedItems)
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                return selectedItems.Count == 1
                    ? ((FilterListObject) selectedItems[0]).Name
                    : "Multiple filters selected";
            }
            else
            {
                return "No filter selected";
            }
        }

        /// <summary>
        /// Load mask image
        /// </summary>
        /// <returns></returns>
        private static async Task<StorageFileImageSource> InitializeFile()
        {
            const string imageFile = @"Assets\mask.jpg";
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(imageFile);
            return new StorageFileImageSource(file);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
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

            if (FilterBox.SelectedItems == null || FilterBox.SelectedItems.Count == 0)
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
                var target = new WriteableBitmap((int)ImageControl.ActualWidth, (int)ImageControl.ActualHeight);

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
            if (App.ChosenPhoto == null)
            {
                return;
            }

            var filters = new List<IFilter>();

            // Get the name of the filter
            foreach (FilterListObject selectedFilter in FilterBox.SelectedItems)
            {
                if (selectedFilter == null)
                {
                    continue;
                }

                // Format the fully qualified name of the class
                var type = string.Format(
                    "Nokia.Graphics.Imaging.{0}, Nokia.Graphics.Imaging, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime",
                    selectedFilter.Name);

                // Use reflection to create the filter class
                var sampleEffect = Type.GetType(type);
                if (sampleEffect != null)
                {
                    var filter = (IFilter)Activator.CreateInstance(sampleEffect, selectedFilter.Constructor);
                    filters.Add(filter);
                }
            }
            ApplyBasicFilter(filters);

        }

        /// <summary>
        /// Apply the chosen filter
        /// </summary>
        /// <param name="sampleEffect"></param>
        private async void ApplyBasicFilter(List<IFilter> sampleEffect)
        {
            if (App.ChosenPhoto == null || _rendering)
            {
                return;
            }
            _rendering = true;
            try
            {
                // Create a source to read the image from PhotoResult stream
                using (var source = new StreamImageSource(App.ChosenPhoto))
                using (var filters = new FilterEffect(source))
                {
                    filters.Filters = sampleEffect.ToArray();

                    // Create a target where the filtered image will be rendered to
                    var target = new WriteableBitmap((int)ImageControl.ActualWidth, (int)ImageControl.ActualHeight);

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
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

