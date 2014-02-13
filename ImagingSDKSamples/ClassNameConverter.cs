using System;
using System.Globalization;
using System.Windows.Data;

namespace ImagingSDKSamples
{
    public class ClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var fqn = ((FilterListObject)value).Name;
                int index = fqn.LastIndexOf(".", StringComparison.InvariantCulture);
                value = fqn.Substring(index + 1);
            }
            else
            {
                value = "No filter";
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
