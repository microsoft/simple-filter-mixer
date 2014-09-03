using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Nokia.Graphics.Imaging;
using Windows.Storage;
using simple_filter_mixer.DataModel;


namespace simple_filter_mixer
{
    public class Helper
    {
        // Used when needed a second image for the filter
        public static readonly List<SplitToneRange> SplitList = new List<SplitToneRange> {
                new SplitToneRange(100, 150, Windows.UI.Color.FromArgb(255, 155, 145, 138)),
                new SplitToneRange(160, 169, Windows.UI.Color.FromArgb(255, 155, 230, 142)),
                new SplitToneRange(170, 172, Windows.UI.Color.FromArgb(255, 155, 130, 49)),
                new SplitToneRange(175, 180, Windows.UI.Color.FromArgb(255, 255, 245, 238))
            };

        public Helper()
        {
        }

        public static ColorImageSource GetColorSource()
        {
            return new ColorImageSource(new Windows.Foundation.Size(640, 480), Windows.UI.Color.FromArgb(255, 255, 0, 0));
        }

        public static StorageFileImageSource GetMaskSource()
        {
            const string imageFile = @"Assets\mask.jpg";
            StorageFile file = null;

            try
            {
                file = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(imageFile).AsTask().Result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new StorageFileImageSource(file);
        }
    }
    public class Imaging
    {
        public EventHandler<bool> IsRenderingChanged;
        private Helper help = new Helper();
        
        private bool _rendering; // Do not start rendering if we haven't finished previous rendering yet

        public static readonly List<FilterListObject> FilterList = new List<FilterListObject>
        {
            new FilterListObject("AntiqueFilter"),
            new FilterListObject("AutoEnhanceFilter") {Constructor = new object[]{true, true}},
            new FilterListObject("AutoLevelsFilter"),
            new FilterListObject("BlendFilter") { Constructor = new object[]{Helper.GetColorSource(), BlendFunction.Colorburn, 0.8}},
            new FilterListObject("BlurFilter") { Constructor = new object[]{100}},
            new FilterListObject("BrightnessFilter") { Constructor = new object[]{0.8}},
            new FilterListObject("CartoonFilter") { Constructor = new object[]{true}},
            new FilterListObject("ChromaKeyFilter") { Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 255, 255), 0.5}},
            new FilterListObject("ColorAdjustFilter") { Constructor = new object[]{0.1, -1, 0.5}},
            new FilterListObject("ColorBoostFilter") { Constructor = new object[]{15}},
            new FilterListObject("ColorSwapFilter") { Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0),
                Windows.UI.Color.FromArgb(255, 0, 255, 0), 0.2, true, true}},
            new FilterListObject("ColorizationFilter") {Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2, 0.1}},
            new FilterListObject("ContrastFilter") {Constructor = new object[]{-0.8}},
            new FilterListObject("CropFilter") { Constructor = new object[]{new Windows.Foundation.Rect(0, 0, 500, 500)}},
            new FilterListObject("CurvesFilter"),
            new FilterListObject("DespeckleFilter") {Constructor = new object[]{DespeckleLevel.High}},
            new FilterListObject("EmbossFilter") {Constructor = new object[]{0.5}},
            new FilterListObject("ExposureFilter") {Constructor = new object[]{ExposureMode.Natural, 0.8}},
            new FilterListObject("FlipFilter") {Constructor = new object[]{FlipMode.Horizontal}}, 
            new FilterListObject("FogFilter"),
            new FilterListObject("FoundationFilter") {Constructor = new object[] {new Windows.Foundation.Rect(0, 0, 700, 700)}},
            new FilterListObject("GrayscaleFilter"),
            new FilterListObject("GrayscaleNegativeFilter"),
            new FilterListObject("HueSaturationFilter") {Constructor = new object[]{0.7, 0.7}},
            new FilterListObject("ImageFusionFilter") {Constructor = new object[]{Helper.GetColorSource(), Helper.GetMaskSource(), false}},
            new FilterListObject("LevelsFilter") {Constructor = new object[]{0.6, 0.1, 0.4}},
            new FilterListObject("LocalBoostAutomaticFilter"),
            new FilterListObject("LocalBoostFilter") {Constructor = new object[]{3, 0.5, 0.5, 0.4}},
            new FilterListObject("LomoFilter") {Constructor = new object[]{0.7, 0.4, LomoVignetting.High, LomoStyle.Yellow}},
            new FilterListObject("MagicPenFilter"),
            new FilterListObject("MilkyFilter"),
            new FilterListObject("MirrorFilter"),
            new FilterListObject("MonoColorFilter") {Constructor = new object[]{Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2}},
            new FilterListObject("MoonlightFilter") {Constructor = new object[]{23}},
            new FilterListObject("NegativeFilter"),
            new FilterListObject("NoiseFilter") {Constructor = new object[]{NoiseLevel.Medium}},
            new FilterListObject("OilyFilter"),
            new FilterListObject("PaintFilter") {Constructor = new object[]{2}},
            new FilterListObject("PosterizeFilter") {Constructor = new object[]{5}},
            new FilterListObject("ReframingFilter") {Constructor = new object[]{new Windows.Foundation.Rect(0,0,300,300), 30, new Windows.Foundation.Point(0.5,0.5)}},
            new FilterListObject("RotationFilter") {Constructor = new object[]{45}},
            new FilterListObject("SepiaFilter"),
            new FilterListObject("SharpnessFilter") {Constructor = new object[]{7}},
            new FilterListObject("SketchFilter") {Constructor = new object[]{SketchMode.Color}},
            new FilterListObject("SolarizeFilter") {Constructor = new object[]{0.5}},
            //new FilterListObject("SplitToneFilter") {Constructor = new object[]{SplitList}},
            new FilterListObject("SpotlightFilter") {Constructor = new object[]{new Windows.Foundation.Point(400, 300), 1200, 0.5}},
            new FilterListObject("StampFilter") {Constructor = new object[]{5, 0.7}},
            new FilterListObject("TemperatureAndTintFilter") {Constructor = new object[]{-0.5, 1}},
            new FilterListObject("VignettingFilter") {Constructor = new object[]{0.2, Windows.UI.Color.FromArgb(255, 10, 10, 10)}},
            new FilterListObject("WarpFilter") {Constructor = new object[]{WarpEffect.HappyFool, 0.7}},
            new FilterListObject("WatercolorFilter") {Constructor = new object[]{0.7, 0.1}},
            new FilterListObject("WhiteBalanceFilter") {Constructor = new object[]{WhitePointCalculationMode.Maximum}},
            new FilterListObject("WhiteboardEnhancementFilter") {Constructor = new object[]{WhiteboardEnhancementMode.Hard}}
        };

        /// <summary>
        /// Load mask image
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFileImageSource> InitializeFile()
        {
            const string imageFile = @"Assets\mask.jpg";
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(imageFile);
            return new StorageFileImageSource(file);
        }

        /// <summary>
        /// Using reflection we create dynamically at runtime all the chosen filters and 
        /// apply the property values if the user has chosen any
        /// </summary>
        /// <param name="filters"></param>
        public static void CreateFilters(List<IFilter> filters)
        {
            if (App.ChosenFilters == null)
            {
                return;
            }

            // Get the name of the filter
            foreach (FilterListObject selectedFilter in App.ChosenFilters)
            {
                if (selectedFilter == null)
                {
                    continue;
                }

                var filter = CreateFilter(selectedFilter.Name, selectedFilter.Constructor, selectedFilter.Parameters);

                if (filter != null)
                {
                    filters.Add(filter);
                }
            }
        }

        /// <summary>
        /// Returns a new filter instance, which is created using reflection
        /// dynamically at runtime.
        /// </summary>
        /// <param name="filterClassName">Filter class name.</param>
        /// <param name="constructorArguments"></param>
        /// <param name="filterParameters">Filter parameters. Can be null.</param>
        /// <returns>A new filter instance or null in case of a failure.</returns>
        public static IFilter CreateFilter(string filterClassName, object[] constructorArguments, Dictionary<string, object> filterParameters = null)
        {
            if (filterClassName == null || filterClassName.Length == 0)
            {
                Debug.WriteLine("Imaging: CreateFilter(): Invalid class name!");
                return null;
            }

            // Format the fully qualified name of the class
            var type = string.Format(
                "Nokia.Graphics.Imaging.{0}, Nokia.Graphics.Imaging, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime",
                filterClassName);

            // Use reflection to create the filter class
            var filterType = Type.GetType(type);

            if (filterType == null)
            {
                Debug.WriteLine("Imaging: CreateFilter(): Failed to get the filter type!");
                return null;
            }

            var filter = (IFilter)Activator.CreateInstance(filterType, constructorArguments);

            // Apply changed parameter values if any
            if (filterParameters != null)
            {
                SetFilterParameters(ref filter, filterParameters);
            }

            return filter;
        }

        /// <summary>
        /// Sets the given parameter values for the given filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterParameters"></param>
        public static void SetFilterParameters(ref IFilter filter, Dictionary<string, object> filterParameters)
        {
            if (filterParameters == null || filterParameters.Count == 0)
            {
                Debug.WriteLine("SetFilterParameters(): No filter parameters given!");
                return;
            }

            PropertyInfo propertyInfo = null;
            var filterType = filter.GetType();
            string nameOfType = "";

            try
            {
                foreach (var parameter in filterParameters)
                {
                    propertyInfo = filterType.GetRuntimeProperty(parameter.Key);

                    if (propertyInfo == null || propertyInfo.Name != parameter.Key)
                    {
                        continue;
                    }

                    nameOfType = propertyInfo.PropertyType.ToString().ToLower();

                    switch (nameOfType)
                    {
                        case "system.double":
                            propertyInfo.SetValue(filter, Convert.ToDouble(parameter.Value));
                            break;
                        case "system.string":
                            propertyInfo.SetValue(filter, parameter.Value.ToString());
                            break;
                        case "system.boolean":
                            propertyInfo.SetValue(filter, parameter.Value.ToString());
                            break;
                        case "system.int32":
                            propertyInfo.SetValue(filter, Convert.ToInt32(parameter.Value));
                            break;
                        case "nokia.graphics.imaging.blurregionshape":
                            propertyInfo.SetValue(filter, Convert.ToInt32(parameter.Value));
                            break;
                        case "windows.foundation.rect":
                            propertyInfo.SetValue(filter, (Windows.Foundation.Rect)parameter.Value);
                            break;
                        case "windows.ui.color":
                            propertyInfo.SetValue(filter, (Windows.UI.Color)parameter.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                if (propertyInfo != null)
                {
                    Debug.WriteLine("SetFilterParameters(): Setting property value: "
                        + propertyInfo.Name + " (" + nameOfType + ") failed with message: " +
                        e.Message);
                }
                else
                {
                    Debug.WriteLine("SetFilterParameters(): Exception: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Apply the chosen filter(s)
        /// </summary>
        /// <param name="sampleEffect"></param>
        public async Task<WriteableBitmap> ApplyBasicFilter(List<IFilter> sampleEffect)
        {
            if (App.ChosenPhoto == null || _rendering)
            {
                return null;
            }

            _rendering = true;

            if (IsRenderingChanged != null)
            {
                IsRenderingChanged(this, true);
            }

            var props = await App.ChosenPhoto.Properties.GetImagePropertiesAsync();
            var target = new WriteableBitmap((int)props.Width, (int)props.Height);

            try
            {
                // Create a source to read the image from PhotoResult stream
                using (var source = new StorageFileImageSource(App.ChosenPhoto))
                using (var filters = new FilterEffect(source))
                {
                    filters.Filters = sampleEffect.ToArray();

                    // Create a new renderer which outputs WriteableBitmaps
                    using (var renderer = new WriteableBitmapRenderer(filters, target))
                    {
                        // Render the image with the filter
                        await renderer.RenderAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            _rendering = false;

            if (IsRenderingChanged != null)
            {
                IsRenderingChanged(this, false);
            }

            return target;
        }

        /// <summary>
        /// Render a photo without any filters
        /// </summary>
        /// <param name="imageControl"></param>
        /// <returns></returns>
        public async Task RenderPlainPhoto(Image imageControl)
        {
            using (var source = new StorageFileImageSource(App.ChosenPhoto))
            {
                var props = await App.ChosenPhoto.Properties.GetImagePropertiesAsync();

                // Create a target where the filtered image will be rendered to
                var target = new WriteableBitmap((int)props.Width, (int)props.Height);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(source, target))
                {
                    // Render the image with the filter(s)
                    await renderer.RenderAsync();

                    // Set the output image to Image control as a source
                    imageControl.Source = target;
                }
            }
        }
    }
}
