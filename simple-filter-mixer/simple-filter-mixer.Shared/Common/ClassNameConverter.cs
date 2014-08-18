using System;

using Windows.UI.Xaml.Data;
using simple_filter_mixer.DataModel;

namespace simple_filter_mixer.Common
{
    public class ClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                var fqn = ((FilterListObject)value).Name;
                int index = fqn.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase);
                value = fqn.Substring(index + 1);
            }
            else
            {
                value = "No filter";
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}

