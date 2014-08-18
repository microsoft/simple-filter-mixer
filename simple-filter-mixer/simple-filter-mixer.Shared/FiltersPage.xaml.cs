//using Nokia.Graphics.Imaging;

using System.Diagnostics;
using simple_filter_mixer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using simple_filter_mixer.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace simple_filter_mixer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FiltersPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public FiltersPage()
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
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            this.navigationHelper.OnNavigatedTo(e);

            FilterView.DataContext = Imaging.FilterList;

            if (App.ChosenFilters == null || FilterView.Items == null) 
                return;

            // Restore the previous selection of filters
            foreach (var listItem in from filter in App.ChosenFilters 
                                     from listItem in FilterView.Items.Cast<FilterListObject>() 
                                     where listItem.Name == filter.Name select listItem)
            {
                FilterView.SelectedItems.Add(listItem);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        
        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var img = sender as Image;

            SetFilterParameters(img);
        }

        private void UIElement_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var img = sender as Image;

            SetFilterParameters(img);
        }

        /// <summary>
        /// Launch Settings page and adjust the filter parameters
        /// </summary>
        /// <param name="img"></param>
        private void SetFilterParameters(Image img)
        {
            if (img != null)
            {
                var obj = img.DataContext as FilterListObject;
                if (obj != null)
                {
                    Debug.WriteLine(obj.Name + " holded");
                    this.Frame.Navigate(typeof (SettingsPage), obj);
                }
            }
        }
    }
}
