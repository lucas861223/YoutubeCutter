using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;


namespace YoutubeCutter.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is bool?)
            {
                bool? tmp = (bool?)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            if (parameter == null)
            {
                return bValue ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return !bValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Visible;
            }
            else
            {
                return false;
            }
        }
    }
}
