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

        public delegate void NotifyChangesFunction(int identifier, VideoInformation information);

        public NotifyChangesFunction function { get; set; }

    }
}
