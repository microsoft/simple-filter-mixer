using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Nokia.Graphics.Imaging;

namespace simple_filter_mixer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Imaging imaging = new Imaging();
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.ChosenPhoto == null)
            {
                try
                {
                    Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

                    App.ChosenPhoto = await StorageFile.GetFileFromPathAsync(installedLocation.Path + @"\Assets\Default.jpg");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw;
                }

            }

            var filters = new List<IFilter>();

            Imaging.CreateFilters(filters);
            ImageControl.Source = await imaging.ApplyBasicFilter(filters);
        }

        private async void OnPhotoClicked(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            // Filter to include a sample subset of file types. 
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".png");

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;

            App.ChosenPhoto = await picker.PickSingleFileAsync();

            if (App.ChosenPhoto != null)
            {
                await imaging.RenderPlainPhoto(ImageControl);
            }
        }
        private void OnFiltersClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FiltersPage));
        }

        private void OnAboutClicked(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(AboutPage));
        }

    }
}
