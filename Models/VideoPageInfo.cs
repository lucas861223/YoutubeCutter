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
        public static NotifyChangesFunction NotifyFunction { get; set; }

        public delegate void SaveWorkProgressFunction(int identifier, ViewModels.VideoViewModel viewModel);
        public static SaveWorkProgressFunction SaveFunction { get; set; }

        public delegate void UpdateVideoPageInfo(int identifier, VideoPageInfo pageInfo);
        public static UpdateVideoPageInfo UpdatePageInfoFunction { get; set; }

        public delegate void RemovePageFunction(int identifier);
        public static RemovePageFunction  RemovePage{ get; set; }

        public delegate void MoveToDownloadFunction(int identifier, ObservableCollection<ClipItem> clips);
        public static MoveToDownloadFunction MoveToDownload { get; set; }
    }
}
