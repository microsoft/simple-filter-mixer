using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace simple_filter_mixer.DataModel
{
    public class FilterListObject
    {
        public string Name { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public object[] Constructor { get; set; }

        /// <summary>
        /// Filter parameters set in the SettingsPage
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        public FilterListObject()
        {
        }

        public FilterListObject(string filterName)
        {
            Name = filterName;

            var assetFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets").GetAwaiter().GetResult();
            var file = assetFolder.GetFileAsync(Name + ".jpg").GetAwaiter().GetResult();
            Thumbnail = new BitmapImage();
            var stream = file.OpenAsync(FileAccessMode.Read).GetAwaiter().GetResult();
            Thumbnail.SetSource(stream);
        }
    }
}
