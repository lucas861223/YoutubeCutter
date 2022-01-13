using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Controls;

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
