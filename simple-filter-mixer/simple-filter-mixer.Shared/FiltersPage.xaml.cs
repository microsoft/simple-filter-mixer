using System.Diagnostics;
using simple_filter_mixer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using simple_filter_mixer.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace simple_filter_mixer
{
    /// <summary>
    /// Page for selecting wanted filters.
    /// </summary>
    public sealed partial class FiltersPage : Page
    {
        private NavigationHelper _navigationHelper;
        private ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private Dictionary<object, bool> _changesList = new Dictionary<object, bool>();
        private static object _itemBeingEdited = null;
        private bool _applyFilterSelection = false;
        private bool _appChangingFilterSelection = false;
        private static bool _settingsPossiblyChanged = false;

        public static bool FiltersChanged
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this._defaultViewModel; }
        }

        public FiltersPage()
        {
            this.InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += this.NavigationHelper_LoadState;
            _navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
            if (App.ChosenFilters != null)
                App.ChosenFilters.Clear();
            else
            {
                App.ChosenFilters = new List<FilterListObject>();
            }

            foreach (FilterListObject item in FilterView.SelectedItems)
            {
                App.ChosenFilters.Add(item);
            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            FiltersChanged = false;
            FilterView.DataContext = Imaging.FilterList;

            if (App.ChosenFilters == null || FilterView.Items == null)
            {
                return;
            }

            _appChangingFilterSelection = true;

            // Restore the previous selection of filters
            foreach (var listItem in from filter in App.ChosenFilters 
                                     from listItem in FilterView.Items.Cast<FilterListObject>() 
                                     where listItem.Name == filter.Name select listItem)
            {
                FilterView.SelectedItems.Add(listItem);
            }

            _appChangingFilterSelection = false;
            _changesList.Clear();

            if (_itemBeingEdited != null)
            {
                FilterView.ScrollIntoView(_itemBeingEdited);
                _itemBeingEdited = null;
            }

            if (_settingsPossiblyChanged)
            {
                ApplyButton.IsEnabled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!_applyFilterSelection && _changesList.Count > 0)
            {
                _appChangingFilterSelection = true;
                bool wasSelected = false;

                foreach (object item in _changesList.Keys)
                {
                    if (_changesList.TryGetValue(item, out wasSelected))
                    {
                        if (wasSelected && FilterView.SelectedItems.Contains(item))
                        {
                            FilterView.SelectedItems.Remove(item);
                        }
                        else if (!wasSelected)
                        {
                            FilterView.SelectedItems.Add(item);
                        }
                    }
                }

                _changesList.Clear();
                _appChangingFilterSelection = false;
            }

            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void OnGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_appChangingFilterSelection)
            {
                return;
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (object item in e.AddedItems)
                {
                    AddChangeToDictionary(item, true);
                }
            }
            else if (e.RemovedItems.Count > 0)
            {
                foreach (object item in e.RemovedItems)
                {
                    AddChangeToDictionary(item, false);
                }
            }
        }

        private void OnFilterItemDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            FilterGridItemControl itemControl = sender as FilterGridItemControl;
            SetFilterParameters(itemControl);
        }

        private void OnFilterItemLongPressed(object sender, HoldingRoutedEventArgs e)
        {
            FilterGridItemControl itemControl = sender as FilterGridItemControl;
            SetFilterParameters(itemControl);
        }

        private void OnApplyButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _settingsPossiblyChanged = false;
            _applyFilterSelection = true;
            FiltersChanged = true;
            NavigationHelper.GoBack();
        }

        /// <summary>
        /// Launch Settings page and adjust the filter parameters
        /// </summary>
        /// <param name="image"></param>
        private void SetFilterParameters(FilterGridItemControl itemControl)
        {
            if (itemControl != null)
            {
                var filterListObject = itemControl.FilterPreviewImage.DataContext as FilterListObject;

                if (filterListObject != null)
                {
                    Debug.WriteLine("FiltersPage: SetFilterParameters(): " + filterListObject.Name);
                    _itemBeingEdited = itemControl;
                    this.Frame.Navigate(typeof (SettingsPage), filterListObject);
                    _settingsPossiblyChanged = true;
                    ApplyButton.IsEnabled = true;
                }
            }
        }

        private void AddChangeToDictionary(object item, bool wasSelected)
        {
            bool wasPreviouslySelected = false;

            if (_changesList.TryGetValue(item, out wasPreviouslySelected))
            {
                if ((wasPreviouslySelected && !wasSelected)
                    || (!wasPreviouslySelected && wasSelected))
                {
                    _changesList.Remove(item);
                }
            }
            else
            {
                _changesList.Add(item, wasSelected);
            }

            if (!ApplyButton.IsEnabled && _changesList.Count > 0)
            {
                ApplyButton.IsEnabled = true;
            }
            else if (ApplyButton.IsEnabled && !_settingsPossiblyChanged && _changesList.Count == 0)
            {
                ApplyButton.IsEnabled = false;
            }
        }
    }
}
