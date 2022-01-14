using Microsoft.Toolkit.Mvvm.ComponentModel;
using YoutubeCutter.Core.Models;
using System.Diagnostics;

namespace YoutubeCutter.Controls
{
    public class DownloadItem : ObservableObject
    {
        private string _bitrate = "N/A";
        public string Bitrate { get { return _bitrate; } set { SetProperty(ref _bitrate, value); } }
        public bool IsDownloading { get { return HasStartedDownloading && !_isDownloaded; } }
        private bool _hasStartedDownloading = false;
        public bool HasStartedDownloading { get { return _hasStartedDownloading; } set { _hasStartedDownloading = value; OnPropertyChanged("IsDownloading"); } }
        private int _progress = 0;
        public int Progress { get { return _progress; } set { SetProperty(ref _progress, value); } }
        public Process DownloadProcess { get; set; }
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

        private bool _showBottom = false;
        private bool _showTop = false;

        public bool ShowTop { get { return _showTop; } set { SetProperty(ref _showTop, value); } }
        public bool ShowBottom { get { return _showBottom; } set { SetProperty(ref _showBottom, value); } }
    }
}
