using System;
using System.Globalization;
using System.Windows.Data;

using YoutubeCutter.Models;

namespace YoutubeCutter.Converters
{
    public sealed class LanguagesToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Languages)
            {
                return (int)value;
            } else
            {
                return 0;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (Languages)value;
            } else
            {
                return Languages.English;
            }
        }
    }
}
