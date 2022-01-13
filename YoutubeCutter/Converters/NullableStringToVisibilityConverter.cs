using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;

namespace YoutubeCutter.Converters
{
    public sealed class NullableStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string x = value as string;
            if (x == null || x == "")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return "";
            }
            else
            {
                return null;
            }
        }
    }
}
