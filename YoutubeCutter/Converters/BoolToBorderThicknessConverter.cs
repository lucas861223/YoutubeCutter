using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;


namespace YoutubeCutter.Converters
{
    public sealed class BoolToBorderThicknessConverter : IValueConverter
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
                return bValue ? 0 : 2;
            }
            else
            {
                return !bValue ? 0 : 2;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (int) value > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
