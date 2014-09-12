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
    public class Imaging
    {
        private const string DebugTag = "Imaging: ";

        public EventHandler<bool> IsRenderingChanged;
        private bool _rendering; // Do not start rendering if we haven't finished previous rendering yet

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
        public static void GetFilters(List<IFilter> filters)
        {
            if (App.ChosenFilters == null)
            {
                return;
            }

            // Get the name of the filter
            foreach (FilterItem selectedFilterItem in App.ChosenFilters)
            {
                if (selectedFilterItem == null)
                {
                    continue;
                }

                if (selectedFilterItem.Filter == null)
                {
                    CreateFilter(selectedFilterItem);
                }
                else if (selectedFilterItem.Parameters != null)
                {
                    string temp = "";
                    SetFilterParameters(selectedFilterItem.Filter, selectedFilterItem.Parameters, out temp);
                }

                filters.Add(selectedFilterItem.Filter);
            }
        }

        /// <summary>
        /// Constructs the IFilter instance of the given FilterItem based on
        /// its properties. Note: No sanity checks!
        /// </summary>
        /// <param name="filterItem"></param>
        public static void CreateFilter(FilterItem filterItem)
        {
            filterItem.Filter = CreateFilter(filterItem.Name, filterItem.Constructor, filterItem.Parameters);
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
                Debug.WriteLine(DebugTag + "CreateFilter(): Invalid class name!");
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
                Debug.WriteLine(DebugTag + "CreateFilter(): Failed to get the filter type!");
                return null;
            }

            var filter = (IFilter)Activator.CreateInstance(filterType, constructorArguments);

            // Apply changed parameter values if any
            if (filterParameters != null)
            {
                string temp = "";
                SetFilterParameters(filter, filterParameters, out temp);
            }

            return filter;
        }

        /// <summary>
        /// For convenience.
        /// </summary>
        /// <param name="filterItem"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool SetFilterParameters(FilterItem filterItem, out string errorMessage)
        {
            return SetFilterParameters(filterItem.Filter, filterItem.Parameters, out errorMessage);
        }

        /// <summary>
        /// Sets the given parameter values for the given filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterParameters"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool SetFilterParameters(IFilter filter,
                                               Dictionary<string, object> filterParameters,
                                               out string errorMessage)
        {
            errorMessage = "";

            if (filter == null || filterParameters == null || filterParameters.Count == 0)
            {
                Debug.WriteLine(DebugTag + "SetFilterParameters(): No filter or parameters given!");
                errorMessage = "Invalid arguments: No filter or parameters given!";
                return false;
            }

            bool success = true;
            PropertyInfo propertyInfo = null;
            var filterType = filter.GetType();
            string propertyTypeName = "";

            try
            {
                foreach (var parameter in filterParameters)
                {
                    propertyInfo = filterType.GetRuntimeProperty(parameter.Key);

                    if (propertyInfo == null || propertyInfo.Name != parameter.Key)
                    {
                        continue;
                    }

                    Debug.WriteLine(DebugTag + "SetFilterParameters(): Setting property: " + parameter.Key + " (" + propertyInfo.PropertyType + ") == " + parameter.Value);
                    propertyTypeName = propertyInfo.PropertyType.ToString().ToLower();

                    switch (propertyTypeName)
                    {
                        case "system.double":
                            propertyInfo.SetValue(filter, Convert.ToDouble(parameter.Value));
                            break;
                        case "system.string":
                            propertyInfo.SetValue(filter, parameter.Value.ToString());
                            break;
                        case "system.boolean":
                            propertyInfo.SetValue(filter, parameter.Value);
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
                            Debug.WriteLine(DebugTag + "SetFilterParameters(): Type " + propertyTypeName + " not handled!");
                            success = false;
                            errorMessage = "No implementation for handling type " + propertyTypeName + ".";
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                success = false;
                errorMessage = "Setting property value: " + propertyInfo.Name
                    + " (" + propertyTypeName + ") failed with message: " + e.Message;
            }

            return success;
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
