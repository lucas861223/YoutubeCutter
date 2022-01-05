using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using YoutubeCutter.Controls;

namespace YoutubeCutter.Models
{
    public class VideoInformation
    {
        public string VideoTitle { set; get; }
        public string ChannelName { set; get; }
        public string AuthorURL { set; get; }
        public string VideoID { set; get; }
        public string ChannelID { set; get; }
        public string VideoThumbnailURL { set; get; }
        public string ChannelThumbnailURL { set; get; }
        public string VideoThumbnailLocation { set; get; }
        public string ChannelThumbnailLocation { set; get; }
    }
}
