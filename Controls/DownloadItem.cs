using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;

namespace YoutubeCutter.Controls
{
    public class DownloadItem
    {
        public bool ShowTopRectangle { get; set; } = false;
        public bool ShowBottomRectangle { get; set; } = false;
        public Time Duration { get { return EndTime - StartTime; } }
        public string Filename { get; set; }
        public string YoutubeURL { get; set; }
        public string VideoTitle { get; set; }
        public string ChannelName { get; set; }
        public string Directory { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }
        public string VideoThumbnail { get; set; }
        public string ChannelThumbnail { get; set; }
        public bool IsDownloaded { get; set; }
    }
}
