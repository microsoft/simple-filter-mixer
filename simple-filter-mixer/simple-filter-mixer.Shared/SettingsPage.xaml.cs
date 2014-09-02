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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace simple_filter_mixer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private NavigationHelper navigationHelper;
        private FilterListObject filterInEdit;
        private Dictionary<string, object> filterParameters = new Dictionary<string, object>();

        public SettingsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

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
                filterInEdit = e.Parameter as FilterListObject;

                if (filterInEdit != null)
                {
                    ExtractProperties();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            GetValues();
        }

        /// <summary>
        /// Check all properties available and create UX controls to adjust them
        /// </summary>
        private void ExtractProperties()
        {
            var type = string.Format(
                "Nokia.Graphics.Imaging.{0}, Nokia.Graphics.Imaging, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime",
                filterInEdit.Name);

            // Use reflection to create the filter class
            var sampleEffect = Type.GetType(type);
            var typeInfo = sampleEffect.GetTypeInfo();

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

            // Get the constructor with most params
            foreach (var property in typeInfo.DeclaredProperties)
            {
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
                            Margin = new Thickness(10, 10, 0, 0)
                        };

                        OptionPanel.Children.Add(item1);
                        filterParameters.Add(property.Name, item1);
                        break;
                    case "int32":
                        var item2i = CreateTextBox(property, "TextBox");
                        OptionPanel.Children.Add(item2i);
                        filterParameters.Add(property.Name, item2i);
                        break;
                    case "double":
                        var item2d = CreateTextBox(property, "TextBox", true);
                        OptionPanel.Children.Add(item2d);
                        filterParameters.Add(property.Name, item2d);
                        break;
                    case "blurregionshape":
                        var item3 = new CheckBox
                        {
                            Name = property.Name + "CheckBox",
                            Content = "Default: Rectangle, Checked: Ellipse",
                            FontSize = 24,
                            Margin = new Thickness(10, 10, 0, 0)
                        };

                        OptionPanel.Children.Add(item3);
                        filterParameters.Add(property.Name, item3);
                        break;
                    case "rect":
                        var item4 = CreateTextBoxGroup(property, "X, Y, Width, Height", new List<string> { "X", "Y", "W", "H" }, 1);
                        filterParameters.Add(property.Name, item4);
                        break;
                    case "color":
                        var item5 = CreateTextBoxGroup(property, "A, R, G, B (decimal)", new List<string> { "A", "R", "G", "B" }, 2);
                        filterParameters.Add(property.Name, item5);
                        break;
                    case "point":
                        var item6 = CreateTextBoxGroup(property, "X, Y", new List<string> { "X", "Y" }, 3);
                        filterParameters.Add(property.Name, item6);
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
        /// <param name="type">1 for rect, 2 for color, 3 for point</param>
        /// <returns></returns>
        private List<object> CreateTextBoxGroup(PropertyInfo property, string header, List<string> names, int type)
        {
            OptionPanel.Children.Add(new TextBlock { Text = header, FontSize = 24, Margin = new Thickness(10, 10, 0, 0) });
            var colGrid = new Grid();

            int i = 0;

            var list = new List<object> { type };
            foreach (var name in names)
            {
                colGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
                var item = CreateTextBox(property, name + "TextBox");
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
        /// <param name="digits"></param>
        /// <returns></returns>
        private static TextBox CreateTextBox(PropertyInfo property, string name, bool digits = false)
        {
            var scope = new InputScope();
            var scopeName = new InputScopeName { NameValue = InputScopeNameValue.Number };

            scope.Names.Add(scopeName);

            var box = new TextBox
            {
                Name = property.Name + name,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 24,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                InputScope = scope,
                Text = digits ? "0.0" : "0"
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
                foreach (var filterListObject in App.ChosenFilters)
                {
                    if (filterListObject.Name == filterInEdit.Name)
                    {
                        filterListObject.Parameters = new Dictionary<string, object>();

                        foreach (var filterParameter in filterParameters)
                        {
                            GetValue(filterParameter, filterListObject);
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Invalid parameter values for the filter entered.");
            }

        }

        private static void GetValue(KeyValuePair<string, object> filterParameter, FilterListObject filterListObject)
        {
            Type original = filterParameter.Value.GetType();
            switch (original.Name.ToLower())
            {
                case "textbox":
                    filterListObject.Parameters.Add(filterParameter.Key, ((TextBox) filterParameter.Value).Text);
                    break;
                case "checkbox":
                    filterListObject.Parameters.Add(filterParameter.Key, ((CheckBox) filterParameter.Value).IsChecked);
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
                        filterListObject.Parameters.Add(filterParameter.Key, rect);
                    }

                    // Color list
                    if (listType == 2)
                    {
                        byte a = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[1]).Text));
                        byte r = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[2]).Text));
                        byte g = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[3]).Text));
                        byte b = Convert.ToByte((((TextBox) ((List<object>) filterParameter.Value)[4]).Text));
                        var col = Color.FromArgb(a, r, g, b);

                        filterListObject.Parameters.Add(filterParameter.Key, col);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
