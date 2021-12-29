using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Helpers
{
    class DownloadManager
    {
        private static WebClient _webClient = WebClient.Instance;
        private static string _CacheLocation = System.AppDomain.CurrentDomain.BaseDirectory + "/Cache";

        public static string DownloadThumbnail()
        {
            return "";
        }

    }
}
