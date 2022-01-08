using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Models
{
    public class DownloadItem
    {
        public Time Duration { get; set; }
        public string Filename { get; set; }
        public string DownloadURL { get; set; }
        public string VideoTitle { get; set; }
        public string ChannelName { get; set; }
        public string Directory { get; set; }
    }
}
