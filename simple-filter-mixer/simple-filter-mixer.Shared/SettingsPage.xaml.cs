using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Popups;

using Nokia.Graphics.Imaging;

using simple_filter_mixer.Common;
using simple_filter_mixer.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace simple_filter_mixer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private const string DebugTag = "SettingsPage: ";
        private readonly Thickness PropertyItemTopMargin = new Thickness(0, 24, 0, 0);
        private readonly Thickness PropertyInnerMargin = new Thickness(6, 6, 0, 0);
        private readonly Thickness RadioButtonMargin = new Thickness(6, 0, 0, 0);
        private readonly GridLength GridItemWidth = new GridLength(110);
        private const int PropertyNameFontSize = 22;
        private const int PropertyValueFontSize = 18;
        private const int TextBoxMargin = 12;
        private const int TextBoxWidth = 80;

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
                    FilterNameTextBlock.Text = _filterItemBeingEdited.Name;

                    if (_filterItemBeingEdited.Filter == null)
                    {
                        Imaging.CreateFilter(_filterItemBeingEdited);
                    }

                    ExtractProperties();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private async void OnApplyButtonClicked(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (!GetValues(out errorMessage))
            {
                MessageDialog errorMessageDialog = new MessageDialog(errorMessage, "Failed to retrieve a property");
                await errorMessageDialog.ShowAsync();
            }

            if (!Imaging.SetFilterParameters(_filterItemBeingEdited, out errorMessage))
            {
                MessageDialog errorMessageDialog = new MessageDialog(errorMessage, "Failed to apply a property");
                await errorMessageDialog.ShowAsync();
            }

            SettingsChanged = true; 
            NavigationHelper.GoBack();
        }

        /// <summary>
        /// Check all properties available and create UX controls to adjust them
        /// </summary>
        private void ExtractProperties()
        {
            string typeString = string.Format(Imaging.ImagingTypeStringStub, _filterItemBeingEdited.Name);

            // Use reflection to get the details of the filter class
            var effectType = Type.GetType(typeString);
            TypeInfo effectTypeInfo = effectType.GetTypeInfo();
            PropertyInfo propertyInfo = null;
            bool effectHasProperties = false;
            
            // Get the constructor with most params
            foreach (var property in effectTypeInfo.DeclaredProperties)
            {
                propertyInfo = effectType.GetRuntimeProperty(property.Name);
                var propertyValue = propertyInfo.GetValue(_filterItemBeingEdited.Filter);
                Debug.WriteLine(DebugTag + "ExtractProperties(): " + propertyInfo + " == " + propertyValue);
                string propertyName = property.Name;
                string propertyTypeName = property.PropertyType.Name;
                string propertyTypeNameToLower = propertyTypeName.ToLower();

                if (propertyTypeNameToLower != "bool" && propertyTypeNameToLower != "boolean")
                {
                    FilterPropertiesPanel.Children.Add(new TextBlock
                    {
                        Text = propertyName,
                        FontSize = PropertyNameFontSize,
                        Margin = PropertyItemTopMargin
                    });
                }

                switch (propertyTypeNameToLower)
                {
                    case "boolean":
                    case "bool":
                        var boolItem = new CheckBox
                        {
                            Name = propertyName + "CheckBox",
                            Content = propertyName,
                            FontSize = PropertyNameFontSize,
                            Margin = PropertyItemTopMargin,
                            IsChecked = (bool)propertyValue
                        };

                        FilterPropertiesPanel.Children.Add(boolItem);
                        _filterParameters.Add(propertyName, boolItem);
                        break;
                    case "int32":
                        var intItem = CreateTextBox(property, "TextBox", propertyValue);
                        FilterPropertiesPanel.Children.Add(intItem);
                        _filterParameters.Add(propertyName, intItem);
                        break;
                    case "double":
                        var doubleItem = CreateTextBox(property, "TextBox", propertyValue, true);
                        FilterPropertiesPanel.Children.Add(doubleItem);
                        _filterParameters.Add(propertyName, doubleItem);
                        break;
                    case "rect":
                        var rectItem = CreateTextBoxGroup(property, "X, Y, Width, Height", new List<string> { "X", "Y", "W", "H" }, propertyValue.ToString().Split(','), TextBoxGroupType.RectType);
                        _filterParameters.Add(propertyName, rectItem);
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

                        var colorItem = CreateTextBoxGroup(property, "A, R, G, B (decimal)", new List<string> { "A", "R", "G", "B" }, colorValues, TextBoxGroupType.ColorType);
                        _filterParameters.Add(propertyName, colorItem);
                        break;
                    case "point":
                        var pointItem = CreateTextBoxGroup(property, "X, Y", new List<string> { "X", "Y" }, propertyValue.ToString().Split(','), TextBoxGroupType.PointType);
                        _filterParameters.Add(propertyName, pointItem);
                        break;
                    default:
                        bool handled = false;
                        string errorMessage = "ExtractProperties(): Cannot handle " + propertyName;

                        if (propertyInfo.ToString().StartsWith(Imaging.ImagingLibraryNamespace))
                        {
                            try
                            {
                                typeString = string.Format(Imaging.ImagingTypeStringStub, propertyName);
                                Type propertyType = Type.GetType(typeString);
                                var enumItem = CreateRadioButtonGroup(propertyTypeName, Enum.GetValues(propertyType), propertyValue);
                                FilterPropertiesPanel.Children.Add(enumItem);
                                _filterParameters.Add(propertyName, enumItem);
                                handled = true;
                            }
                            catch (Exception e)
                            {
                                errorMessage += ": " + e.ToString();
                            }
                        }

                        if (!handled)
                        {
                            Debug.WriteLine(DebugTag + "ExtractProperties(): " + errorMessage);

                            FilterPropertiesPanel.Children.Add(new TextBlock
                            {
                                Margin = PropertyInnerMargin,
                                FontSize = PropertyValueFontSize,
                                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                                Text = "Implementation for modifying this property missing",
                            });
                        }

                        break;
                } // switch (propertyTypeNameToLower)

                effectHasProperties = true;
            }

            NoPropertiesTextBlock.Visibility = effectHasProperties ? Visibility.Collapsed : Visibility.Visible;
            ApplyButton.IsEnabled = effectHasProperties ? true : false;
            FilterPropertiesPanel.InvalidateArrange();
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
            FilterPropertiesPanel.Children.Add(
                new TextBlock {
                    Text = header,
                    FontSize = PropertyValueFontSize,
                    Margin = PropertyInnerMargin
                });

            var colGrid = new Grid();
            int i = 0;
            var list = new List<object> { type };
            bool valuesOk = (values != null && names.Count == values.Length);

            if (!valuesOk)
            {
                Debug.WriteLine(DebugTag + "CreateTextBoxGroup(): Values are not OK:" + (values == null ? " null" : ""));

                foreach (object valueObject in values)
                {
                    Debug.WriteLine("\t" + valueObject);
                }
            }

            foreach (var name in names)
            {
                colGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(TextBoxWidth + TextBoxMargin) });
                var item = CreateTextBox(property, name + "TextBox", (valuesOk ? values[i] : null));
                Grid.SetColumn(item, i);
                i++;
                colGrid.Children.Add(item);
                list.Add(item);
            }

            FilterPropertiesPanel.Children.Add(colGrid);

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
        private TextBox CreateTextBox(PropertyInfo property, string name, object value, bool digits = false)
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
                Margin = PropertyInnerMargin,
                Width = TextBoxWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                InputScope = scope,
                Text = valueString
            };

            return box;
        }

        private UIElement CreateRadioButtonGroup(string groupName, Array enumValues, object selectedValue)
        {
            StackPanel stackPanel = new StackPanel();

            foreach (var enumValue in enumValues)
            {
                RadioButton radioButton = new RadioButton()
                {
                    Margin = RadioButtonMargin,
                    GroupName = groupName,
                    FontSize = PropertyValueFontSize,
                    Content = enumValue.ToString()
                };

                stackPanel.Children.Add(radioButton);

                if (selectedValue != null && enumValue.ToString().Equals(selectedValue.ToString()))
                {
                    radioButton.IsChecked = true;
                }
            }

            return stackPanel;
        }

        /// <summary>
        /// Get chosen values from the dynamically created controls
        /// </summary>
        private bool GetValues(out string errorMessage)
        {
            bool success = true;
            errorMessage = "";

            try
            {
                _filterItemBeingEdited.Parameters = new Dictionary<string, object>();
                string temp = "";

                foreach (var filterParameter in _filterParameters)
                {
                    if (!GetValue(filterParameter, _filterItemBeingEdited, out temp))
                    {
                        success = false;
                        errorMessage = temp;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(DebugTag + "GetValues(): Error: " + e.ToString());
                success = false;
                errorMessage = e.ToString();
            }

            return success;
        }

        private static bool GetValue(KeyValuePair<string, object> filterParameter,
                                     FilterItem filterItem,
                                     out string errorMessage)
        {
            bool success = true;
            errorMessage = "";
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
                    TextBoxGroupType listType = (TextBoxGroupType)((List<object>)filterParameter.Value)[0];

                    if (listType == TextBoxGroupType.RectType)
                    {
                        int x = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[1]).Text);
                        int y = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[2]).Text);
                        int w = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[3]).Text);
                        int h = Convert.ToInt32(((TextBox) ((List<object>) filterParameter.Value)[4]).Text);
                        var rect = new Rect(x, y, w, h);
                        filterItem.Parameters.Add(filterParameter.Key, rect);
                    }
                    else if (listType == TextBoxGroupType.ColorType)
                    {
                        byte a = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[1]).Text));
                        byte r = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[2]).Text));
                        byte g = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[3]).Text));
                        byte b = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[4]).Text));
                        var color = Color.FromArgb(a, r, g, b);
                        filterItem.Parameters.Add(filterParameter.Key, color);
                    }
                    else
                    {
                        success = false;
                    }

                    break;
                case "stackpanel": // Radio buttons for enumeration
                    StackPanel container = (StackPanel)filterParameter.Value;

                    foreach (var child in container.Children)
                    {
                        if (child is RadioButton)
                        {
                            RadioButton radioButton = (RadioButton)child;

                            if ((bool)radioButton.IsChecked)
                            {
                                string typeString = string.Format(Imaging.ImagingTypeStringStub, filterParameter.Key);
                                Type type = Type.GetType(typeString);
                                Array enumValueArray = Enum.GetValues(type);

                                foreach (var enumValue in enumValueArray)
                                {
                                    if (enumValue.ToString().Equals(radioButton.Content))
                                    {
                                        filterItem.Parameters.Add(filterParameter.Key, enumValue);
                                        success = true;
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    break;
                default:
                    success = false;
                    break;
            }

            if (!success)
            {
                errorMessage = "No implementation for handling property " + filterParameter.Key
                    + " (UI control is of type " + valueType.Name + ").";
                Debug.WriteLine(DebugTag + "GetValue(): " + errorMessage);
            }

            return success;
        }
    }
}
