using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;

namespace YoutubeCutter.Converters
{
    public sealed class NullableStringToVisibilityConverterInverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string x = value as string;
            if (x == null || x == "")
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return null; 
            }
            else
            {
                return "";
            }
        }
    }
}
