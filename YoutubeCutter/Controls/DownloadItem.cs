using Microsoft.Toolkit.Mvvm.ComponentModel;
using YoutubeCutter.Core.Models;
using System.Diagnostics;

namespace YoutubeCutter.Controls
{
    public class DownloadItem : ObservableObject
    {
        public bool IsDownloading { get { return HasStartedDownloading && !_isDownloaded; } }
        private bool _hasStartedDownloading = false;
        public bool HasStartedDownloading { get { return _hasStartedDownloading; } set { _hasStartedDownloading = value; OnPropertyChanged("IsDownloading"); } }
        public int Progress { get; set; }
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
    }
}
