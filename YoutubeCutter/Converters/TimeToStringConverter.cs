using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using YoutubeCutter.Core.Models;
using YoutubeCutter.Core.Helpers;

namespace YoutubeCutter.Converters
{
    public sealed class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Time x = value as Time;
            if (x is Time)
            {
                return TimeUtil.TimeToString(x);
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Time result = new Time();
            if (value is string)
            {
                TimeUtil.ParseTimeFromString(value as string, result);
            }
            return result;
        }
    }
}
