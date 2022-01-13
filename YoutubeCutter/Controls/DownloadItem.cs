using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;
using YoutubeCutter.Core.Models;

namespace YoutubeCutter.Controls
{
    public class DownloadItem : ObservableObject
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
        private bool _isDownloaded = false;
        public bool IsDownloaded { get { return _isDownloaded; } set { SetProperty(ref _isDownloaded, value); } }
    }
}
