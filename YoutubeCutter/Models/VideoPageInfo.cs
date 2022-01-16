using System.Collections.ObjectModel;

using YoutubeCutter.Controls;
using YoutubeCutter.Core.Models;

namespace YoutubeCutter.Models
{
    public class VideoPageInfo
    {
        public int Identifier { get; set; }

        public delegate void NotifyChangesFunction(int identifier, VideoInformation information);
        public delegate void MoveToDownloadFunction(int identifier, ObservableCollection<ClipItem> clips);
        public delegate void RemovePageFunction(int identifier);

        public static NotifyChangesFunction NotifyFunction { get; set; }
        public static RemovePageFunction  RemovePage{ get; set; }
        public static MoveToDownloadFunction MoveToDownload { get; set; }
    }
}
