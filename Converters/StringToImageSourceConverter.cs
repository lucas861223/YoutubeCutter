using System.Windows;
using System.Globalization;
using System.Windows.Data;
using System;
using System.Windows.Media.Imaging;
using YoutubeCutter.Properties;
using System.IO;

namespace YoutubeCutter.Converters
{
    public sealed class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (value == null || value as string == "")
            {
                if (parameter != null)
                {
                    if (parameter as string == "Channel")
                    {
                        var bitmap = Resources.YoutubeChannelDefaultIcon;
                        var memoryStream = new MemoryStream();
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                        memoryStream.Position = 0;

                        image.StreamSource = memoryStream;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                    }
                }
            } 
            else
            {
                image.UriSource = new Uri(value as string);
            }
            image.EndInit();
            return image;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage && value != null)
            {
                return ((BitmapImage)value).UriSource.AbsoluteUri;
            }
            return "";
        }
    }
}
