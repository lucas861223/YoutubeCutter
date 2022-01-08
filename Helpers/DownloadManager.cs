using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Helpers
{
    class DownloadManager
    {
        private static WebClient _webClient = WebClient.Instance;
        private static string _cacheLocation = AppDomain.CurrentDomain.BaseDirectory + "Cache";

        public static string DownloadThumbnail(string url, string youtubeID)
        {
            if (!Directory.Exists(_cacheLocation + "\\" + youtubeID))
            {
                Directory.CreateDirectory(_cacheLocation + "\\" + youtubeID);
                _webClient.DownloadImage(url, _cacheLocation + "\\" + youtubeID + "\\thumbnail.jpg");
            }
            return _cacheLocation + "\\" + youtubeID + "\\thumbnail.jpg";
        }

        public static void MakeChacheFolder()
        {

            Directory.CreateDirectory(_cacheLocation);

        }

        public static string GetDownloadPath(string videoTitle, string channelName)
        {
            string path = (string)App.Current.Properties["DownloadPath"];
            if ((bool)App.Current.Properties["CategorizeByDate"])
            {
                path += DateTime.Today.ToString("yyyy-MM-dd") + "\\";
            }
            if ((bool)App.Current.Properties["CategorizeByChannel"])
            {
                path += channelName + "\\";
            }
            if ((bool)App.Current.Properties["CategorizeByVideo"])
            {
                path += videoTitle + "\\";
            }
            return path;
        }

        public static void ClearCacheFolder()
        {
            try
            {
                Directory.Delete(_cacheLocation, true);
            }
            catch (DirectoryNotFoundException)
            {
                
            }

        }

    }
}
