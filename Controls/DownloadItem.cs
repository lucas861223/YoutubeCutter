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
        public Time Duration { get { return StartTime - EndTime; } }
        public string Filename { get; set; }
        public string DownloadURL { get; set; }
        public string VideoTitle { get; set; }
        public string ChannelName { get; set; }
        public string Directory { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }
    }
}
