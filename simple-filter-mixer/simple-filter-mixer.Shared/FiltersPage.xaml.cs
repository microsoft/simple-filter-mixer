using System.Diagnostics;
using simple_filter_mixer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif

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
        private static List<object> _tempList = new List<object>();
        private static object _itemBeingEdited = null;

        public static bool FiltersChanged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public FiltersPage()
        {
            this.InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
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

            FilterGridView.DataContext = FilterDefinitions.FilterItemList;

            if (_tempList == null || FilterGridView.Items == null)
            {
                return;
            }

            FilterGridView.SelectionChanged -= OnGridViewSelectionChanged;

            // Restore the previous selection of filters
            foreach (var listItem in from filter in _tempList.Cast<FilterItem>()
                                     from listItem in FilterGridView.Items.Cast<FilterItem>() 
                                     where listItem.Name == filter.Name select listItem)
            {
                FilterGridView.SelectedItems.Add(listItem);
            }

            FilterGridView.SelectionChanged += OnGridViewSelectionChanged;

            if (_itemBeingEdited != null)
            {
                FilterGridView.ScrollIntoView(_itemBeingEdited);
                _itemBeingEdited = null;
            }

            CheckIfApplyButtonShouldBeEnabled();

#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += OnBackPressed;
#endif
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed -= OnBackPressed;
#endif

            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void OnGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterItem changedItem = null;
            bool selected = false;

            if (e.AddedItems.Count > 0)
            {
                _tempList.AddRange(e.AddedItems);
                changedItem = e.AddedItems[0] as FilterItem;
                selected = true;
            }
            else if (e.RemovedItems.Count > 0)
            {
                foreach (object item in e.RemovedItems)
                {
                    if (_tempList.Contains(item))
                    {
                        _tempList.Remove(item);
                    }
                }

                changedItem = e.RemovedItems[0] as FilterItem;
            }

            if (changedItem != null)
            {
                Debug.WriteLine("FiltersPage: OnGridViewSelectionChanged(): "
                    + changedItem.Name
                    + (selected ? " selected" : " deselected"));
            }

            CheckIfApplyButtonShouldBeEnabled();
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
            Debug.WriteLine("FiltersPage: OnApplyButtonClicked()");
            FiltersChanged = true;

            if (App.ChosenFilters != null)
            {
                App.ChosenFilters.Clear();
            }
            else
            {
                App.ChosenFilters = new List<FilterItem>();
            }

            foreach (FilterItem item in _tempList)
            {
                App.ChosenFilters.Add(item);
            }

            NavigationHelper.GoBack();
        }

#if WINDOWS_PHONE_APP
        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            Debug.WriteLine("FiltersPage: OnBackPressed()");
            _tempList.Clear();

            if (App.ChosenFilters == null)
            {
                return;
            }

            foreach (var listItem in from filter in App.ChosenFilters
                                     from listItem in FilterGridView.Items.Cast<FilterItem>()
                                     where listItem.Name == filter.Name
                                     select listItem)
            {

                _tempList.Add(listItem);
            }
        }
#endif

        private void OnAboutClicked(object sender, RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            Frame.Navigate(typeof(AboutPage));
#endif
        }

        /// <summary>
        /// Launch Settings page and adjust the filter parameters
        /// </summary>
        /// <param name="image"></param>
        private void SetFilterParameters(FilterGridItemControl itemControl)
        {
            if (itemControl != null)
            {
                var filterListObject = itemControl.FilterPreviewImage.DataContext as FilterItem;

                if (filterListObject != null)
                {
                    Debug.WriteLine("FiltersPage: SetFilterParameters(): " + filterListObject.Name);
                    _itemBeingEdited = itemControl;
                    this.Frame.Navigate(typeof (SettingsPage), filterListObject);
                    ApplyButton.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Compares the given lists. Note that this method expects that each
        /// filter in eather of the lists is unique (i.e. no two filters with
        /// same name in one list).
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns>True if the lists match, false otherwise.</returns>
        private bool FilterListsMatch(IList<FilterItem> listA, List<object> listB)
        {
            bool match = true;

            // Null list is considered the same as an empty one
            if ((listA == null || listA.Count == 0) && (listB == null || listB.Count == 0))
            {
                match = true;
            }
            else if (listA == null || listB == null || listA.Count != listB.Count)
            {
                match = false;
            }
            else
            {
                for (int i = 0; i < listA.Count; ++i)
                {
                    FilterItem itemA = listA[i] as FilterItem;
                    FilterItem itemB = null;
                    bool found = false;

                    for (int j = 0; j < listB.Count; ++j)
                    {
                        itemB = listB[i] as FilterItem;

                        if (itemA.Name.Equals(itemB.Name))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        match = false;
                        break;
                    }
                }
            }

            return match;
        }

        private void CheckIfApplyButtonShouldBeEnabled()
        {
            if (!ApplyButton.IsEnabled
                && (SettingsPage.SettingsChanged
                    || !FilterListsMatch(App.ChosenFilters, _tempList)))
            {
                ApplyButton.IsEnabled = true;
            }
            else if (ApplyButton.IsEnabled
                     && !SettingsPage.SettingsChanged
                     && FilterListsMatch(App.ChosenFilters, _tempList))
            {
                ApplyButton.IsEnabled = false;
            }
        }
    }
}
