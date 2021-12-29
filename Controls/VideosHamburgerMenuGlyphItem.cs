using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;

namespace YoutubeCutter.Controls
{
    class VideosHamburgerMenuGlyphItem : HamburgerMenuGlyphItem
    {
        public string ThumbnailURL { get; set; }
        public string VideoTitle { get; set; }
        public string ChannelName { get; set; }
        public string ChannelThumbnail { get; set; }
    }
}
