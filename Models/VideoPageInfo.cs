using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Models
{
    class VideoPageInfo
    {
        public int Identifier { get; set; }

        public delegate void NotifyChangesFunction(int identifier, string videoName, string channelname, string thumbnailURL);

        public NotifyChangesFunction function { get; set; }

    }
}
