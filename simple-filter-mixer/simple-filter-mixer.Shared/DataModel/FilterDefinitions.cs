using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace simple_filter_mixer.DataModel
{
    class FilterDefinitions
    {
        // Used when needed a second image for the filter
        public static readonly List<SplitToneRange> SplitList = new List<SplitToneRange> {
            new SplitToneRange(100, 150, Windows.UI.Color.FromArgb(255, 155, 145, 138)),
            new SplitToneRange(160, 169, Windows.UI.Color.FromArgb(255, 155, 230, 142)),
            new SplitToneRange(170, 172, Windows.UI.Color.FromArgb(255, 155, 130, 49)),
            new SplitToneRange(175, 180, Windows.UI.Color.FromArgb(255, 255, 245, 238))
        };

        public static readonly List<FilterItem> FilterItemList = new List<FilterItem>
        {
            new FilterItem("AntiqueFilter"),
            new FilterItem("AutoEnhanceFilter") { Constructor = new object[] {true, true } },
            new FilterItem("AutoLevelsFilter"),
            new FilterItem("BlendFilter") { Constructor = new object[] { GetColorSource(), BlendFunction.Colorburn, 0.8 } },
            new FilterItem("BlurFilter") { Constructor = new object[] { 100 } },
            new FilterItem("BrightnessFilter") { Constructor = new object[] { 0.8 } },
            new FilterItem("CartoonFilter") { Constructor = new object[] { true } },
            new FilterItem("ChromaKeyFilter") { Constructor = new object[] { Windows.UI.Color.FromArgb(255, 255, 255, 255), 0.5 } },
            new FilterItem("ColorAdjustFilter") { Constructor = new object[] { 0.1, -1, 0.5 } },
            new FilterItem("ColorBoostFilter") { Constructor = new object[] { 15 } },
            new FilterItem("ColorSwapFilter") { Constructor = new object[] { Windows.UI.Color.FromArgb(255, 255, 0, 0), Windows.UI.Color.FromArgb(255, 0, 255, 0), 0.2, true, true } },
            new FilterItem("ColorizationFilter") { Constructor = new object[] { Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2, 0.1 } },
            new FilterItem("ContrastFilter") { Constructor = new object[] {-0.8 } },
            new FilterItem("CropFilter") { Constructor = new object[] { new Windows.Foundation.Rect(0, 0, 500, 500) } },
            new FilterItem("CurvesFilter"),
            new FilterItem("DespeckleFilter") { Constructor = new object[] { DespeckleLevel.High } },
            new FilterItem("EmbossFilter") { Constructor = new object[] { 0.5 } },
            new FilterItem("ExposureFilter") { Constructor = new object[] { ExposureMode.Natural, 0.8 } },
            new FilterItem("FlipFilter") { Constructor = new object[] { FlipMode.Horizontal } }, 
            new FilterItem("FogFilter"),
            new FilterItem("FoundationFilter") { Constructor = new object[] { new Windows.Foundation.Rect(0, 0, 700, 700) } },
            new FilterItem("GrayscaleFilter"),
            new FilterItem("GrayscaleNegativeFilter"),
            new FilterItem("HueSaturationFilter") { Constructor = new object[] { 0.7, 0.7 } },
            new FilterItem("ImageFusionFilter") { Constructor = new object[] { GetColorSource(), GetMaskSource(), false } },
            new FilterItem("LevelsFilter") { Constructor = new object[] { 0.6, 0.1, 0.4 } },
            new FilterItem("LocalBoostAutomaticFilter"),
            new FilterItem("LocalBoostFilter") { Constructor = new object[] { 3, 0.5, 0.5, 0.4 } },
            new FilterItem("LomoFilter") { Constructor = new object[] { 0.7, 0.4, LomoVignetting.High, LomoStyle.Yellow } },
            new FilterItem("MagicPenFilter"),
            new FilterItem("MilkyFilter"),
            new FilterItem("MirrorFilter"),
            new FilterItem("MonoColorFilter") { Constructor = new object[] { Windows.UI.Color.FromArgb(255, 255, 0, 0), 0.2 } },
            new FilterItem("MoonlightFilter") { Constructor = new object[] { 23 } },
            new FilterItem("NegativeFilter"),
            new FilterItem("NoiseFilter") { Constructor = new object[] { NoiseLevel.Medium } },
            new FilterItem("OilyFilter"),
            new FilterItem("PaintFilter") { Constructor = new object[] { 2 } },
            new FilterItem("PosterizeFilter") { Constructor = new object[] { 5 } },
            new FilterItem("ReframingFilter") { Constructor = new object[] { new Windows.Foundation.Rect(0,0,300,300), 30, new Windows.Foundation.Point(0.5,0.5) } },
            new FilterItem("RotationFilter") { Constructor = new object[] { 45 } },
            new FilterItem("SepiaFilter"),
            new FilterItem("SharpnessFilter") { Constructor = new object[] { 7 } },
            new FilterItem("SketchFilter") { Constructor = new object[] { SketchMode.Color } },
            new FilterItem("SolarizeFilter") { Constructor = new object[] { 0.5 } },
            //new FilterItem("SplitToneFilter") { Constructor = new object[] { SplitList } },
            new FilterItem("SpotlightFilter") { Constructor = new object[] { new Windows.Foundation.Point(400, 300), 1200, 0.5 } },
            new FilterItem("StampFilter") { Constructor = new object[] { 5, 0.7 } },
            new FilterItem("TemperatureAndTintFilter") { Constructor = new object[] { -0.5, 1 } },
            new FilterItem("VignettingFilter") { Constructor = new object[] { 0.2, Windows.UI.Color.FromArgb(255, 10, 10, 10) } },
            new FilterItem("WarpFilter") { Constructor = new object[] { WarpEffect.HappyFool, 0.7 } },
            new FilterItem("WatercolorFilter") { Constructor = new object[] { 0.7, 0.1 } },
            new FilterItem("WhiteBalanceFilter") { Constructor = new object[] { WhitePointCalculationMode.Maximum } },
            new FilterItem("WhiteboardEnhancementFilter") { Constructor = new object[] { WhiteboardEnhancementMode.Hard } }
        };

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
}
