using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using simple_filter_mixer.Common;
using simple_filter_mixer.DataModel;
using System.Reflection;
using Nokia.Graphics.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace simple_filter_mixer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private enum TextBoxGroupType
        {
            RectType,
            ColorType,
            PointType
        };

        private NavigationHelper _navigationHelper;
        private FilterItem _filterItemBeingEdited;
        private Dictionary<string, object> _filterParameters = new Dictionary<string, object>();

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        public static bool SettingsChanged
        {
            get;
            set;
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                _filterItemBeingEdited = e.Parameter as FilterItem;

                if (_filterItemBeingEdited != null)
                {
                    if (_filterItemBeingEdited.Filter == null)
                    {
                        Imaging.CreateFilter(_filterItemBeingEdited);
                    }

                    ExtractProperties();
                }
            }

            SettingsChanged = true; // TODO: Check if the settings were actually changed
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            GetValues();
            Imaging.SetFilterParameters(_filterItemBeingEdited);
        }

        /// <summary>
        /// Check all properties available and create UX controls to adjust them
        /// </summary>
        private void ExtractProperties()
        {
            OptionPanel.Children.Add(new TextBlock
            {
                Text = "Please check valid value ranges for each parameter from Nokia Imaging SDK API reference at: ",
                TextWrapping = TextWrapping.WrapWholeWords,
                FontSize = 20,
                Margin = new Thickness(10, 10, 0, 0)
            });

            OptionPanel.Children.Add(new HyperlinkButton
            {
                FontSize = 24,
                Content = "http://developer.nokia.com",
                NavigateUri = new Uri("http://developer.nokia.com/resources/library/Imaging_API_Ref"),
                Margin = new Thickness(0, 10, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var type = string.Format(
                "Nokia.Graphics.Imaging.{0}, Nokia.Graphics.Imaging, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime",
                _filterItemBeingEdited.Name);

            // Use reflection to create the filter class
            var effectType = Type.GetType(type);
            var effectTypeInfo = effectType.GetTypeInfo();
            PropertyInfo propertyInfo = null;

            // Get the constructor with most params
            foreach (var property in effectTypeInfo.DeclaredProperties)
            {
                propertyInfo = effectType.GetRuntimeProperty(property.Name);
                var propertyValue = propertyInfo.GetValue(_filterItemBeingEdited.Filter);
                System.Diagnostics.Debug.WriteLine("SettingsPage: ExtractProperties(): " + propertyInfo + " == " + propertyValue);
                string name = property.PropertyType.Name.ToLower();

                if (name != "bool" && name != "boolean")
                {
                    OptionPanel.Children.Add(new TextBlock
                    {
                        Text = property.Name,
                        FontSize = 24,
                        Margin = new Thickness(10, 10, 10, 10)
                    });
                }

                switch (name)
                {
                    case "boolean":
                    case "bool":
                        var item1 = new CheckBox
                        {
                            Name = property.Name + "CheckBox",
                            Content = property.Name,
                            FontSize = 24,
                            Margin = new Thickness(10, 10, 0, 0),
                            IsChecked = (bool)propertyValue
                        };

                        OptionPanel.Children.Add(item1);
                        _filterParameters.Add(property.Name, item1);
                        break;
                    case "int32":
                        var item2i = CreateTextBox(property, "TextBox", propertyValue);
                        OptionPanel.Children.Add(item2i);
                        _filterParameters.Add(property.Name, item2i);
                        break;
                    case "double":
                        var item2d = CreateTextBox(property, "TextBox", propertyValue, true);
                        OptionPanel.Children.Add(item2d);
                        _filterParameters.Add(property.Name, item2d);
                        break;
                    case "blurregionshape":
                        var item3 = new CheckBox
                        {
                            Name = property.Name + "CheckBox",
                            Content = "Default: Rectangular, Checked: Elliptical",
                            FontSize = 24,
                            Margin = new Thickness(10, 10, 0, 0),
                            IsChecked = ((BlurRegionShape)propertyValue) == BlurRegionShape.Elliptical
                        };

                        OptionPanel.Children.Add(item3);
                        _filterParameters.Add(property.Name, item3);
                        break;
                    case "rect":
                        var item4 = CreateTextBoxGroup(property, "X, Y, Width, Height", new List<string> { "X", "Y", "W", "H" }, propertyValue.ToString().Split(','), TextBoxGroupType.RectType);
                        _filterParameters.Add(property.Name, item4);
                        break;
                    case "color":
                        object[] colorValues = new object[4];
                        string colorString = propertyValue.ToString().Remove(0, 1); // Remove '#'

                        if (colorString.Length == 6 || colorString.Length == 8)
                        {
                            int objectIndex = 0;

                            if (colorString.Length < 8)
                            {
                                colorValues[0] = 255;
                                objectIndex = 1;
                            }

                            for (int i = 0; i < colorString.Length; i += 2)
                            {
                                colorValues[objectIndex] = int.Parse(colorString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                                objectIndex++;
                            }
                        }

                        var item5 = CreateTextBoxGroup(property, "A, R, G, B (decimal)", new List<string> { "A", "R", "G", "B" }, colorValues, TextBoxGroupType.ColorType);
                        _filterParameters.Add(property.Name, item5);
                        break;
                    case "point":
                        var item6 = CreateTextBoxGroup(property, "X, Y", new List<string> { "X", "Y" }, propertyValue.ToString().Split(','), TextBoxGroupType.PointType);
                        _filterParameters.Add(property.Name, item6);
                        break;
                    default:
                        break;
                }
            }

            OptionPanel.InvalidateArrange();
        }

        /// <summary>
        /// Create the group of all required textbox next to each other within a grid. 
        /// Type parameter defines if the values will present rect or color
        /// </summary>
        /// <param name="property"></param>
        /// <param name="header"></param>
        /// <param name="names"></param>
        /// <param name="values"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<object> CreateTextBoxGroup(PropertyInfo property, string header, List<string> names, object[] values, TextBoxGroupType type)
        {
            OptionPanel.Children.Add(new TextBlock { Text = header, FontSize = 24, Margin = new Thickness(10, 10, 0, 0) });
            var colGrid = new Grid();
            int i = 0;
            var list = new List<object> { type };
            bool valuesOk = (values != null && names.Count == values.Length);

            if (!valuesOk)
            {
                System.Diagnostics.Debug.WriteLine("SettingsPage: CreateTextBoxGroup(): Values are not OK:" + (values == null ? " null" : "\n"));

                foreach (object valueObject in values)
                {
                    System.Diagnostics.Debug.WriteLine("\t" + valueObject);
                }
            }

            foreach (var name in names)
            {
                colGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
                var item = CreateTextBox(property, name + "TextBox", (valuesOk ? values[i] : null));
                Grid.SetColumn(item, i);
                i++;
                colGrid.Children.Add(item);
                list.Add(item);
            }

            OptionPanel.Children.Add(colGrid);

            return list;
        }

        /// <summary>
        /// Create a single textbox control
        /// </summary>
        /// <param name="property"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        private static TextBox CreateTextBox(PropertyInfo property, string name, object value, bool digits = false)
        {
            var scope = new InputScope();
            var scopeName = new InputScopeName { NameValue = InputScopeNameValue.Number };

            scope.Names.Add(scopeName);
            string valueString = digits ? "0.0" : "0";

            if (value != null && value.ToString().Length > 0)
            {
                valueString = value.ToString();
            }

            var box = new TextBox
            {
                Name = property.Name + name,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 24,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                InputScope = scope,
                Text = valueString
            };

            return box;
        }

        /// <summary>
        /// Get chosen values from the dynamically created controls
        /// </summary>
        private void GetValues()
        {
            try
            {
                foreach (var filterItem in FilterDefinitions.FilterItemList)
                {
                    if (filterItem.Name == _filterItemBeingEdited.Name)
                    {
                        filterItem.Parameters = new Dictionary<string, object>();

                        foreach (var filterParameter in _filterParameters)
                        {
                            GetValue(filterParameter, filterItem);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SettingsPage: GetValues(): Invalid parameter values for the filter entered: " + e.ToString());
            }

        }

        private static void GetValue(KeyValuePair<string, object> filterParameter, FilterItem filterItem)
        {
            Type valueType = filterParameter.Value.GetType();

            switch (valueType.Name.ToLower())
            {
                case "textbox":
                    filterItem.Parameters.Add(filterParameter.Key, ((TextBox) filterParameter.Value).Text);
                    break;
                case "checkbox":
                    filterItem.Parameters.Add(filterParameter.Key, ((CheckBox) filterParameter.Value).IsChecked);
                    break;
                case "list`1":
                    int listType = Convert.ToInt32(((List<object>) filterParameter.Value)[0]);

                    // Rect list
                    if (listType == 1)
                    {
                        int x = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[1]).Text);
                        int y = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[2]).Text);
                        int w = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[3]).Text);
                        int h = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[4]).Text);
                        var rect = new Rect(x, y, w, h);
                        filterItem.Parameters.Add(filterParameter.Key, rect);
                    }

                    // Color list
                    if (listType == 2)
                    {
                        byte a = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[1]).Text));
                        byte r = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[2]).Text));
                        byte g = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[3]).Text));
                        byte b = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[4]).Text));
                        var col = Color.FromArgb(a, r, g, b);

                        filterItem.Parameters.Add(filterParameter.Key, col);
                    }
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("SettingsPage: GetValue(): Type " + valueType.Name + " not handled!");
                    break;
            }
        }
    }
}
