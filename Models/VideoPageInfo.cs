using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Models
{
    public class VideoPageInfo
    {
        public int Identifier { get; set; }

        public string YoutubeURL { get; set; }

        public delegate void NotifyChangesFunction(int identifier, VideoInformation information);

        public static NotifyChangesFunction NotifyFunction { get; set; }

        public string EmbedYoutubeURL { get; set; }

        public delegate void SaveWorkProgressFunction(VideoPageInfo pageInfo);

        public static SaveWorkProgressFunction SaveFunction { get; set; }
    }
}
