using MahApps.Metro.Controls;

using System.Windows;

using YoutubeCutter.Core.Models;

namespace YoutubeCutter.Controls
{
    class VideosHamburgerMenuItem : HamburgerMenuItem
    {
        public static readonly DependencyProperty VideoThumbnailProperty =
        DependencyProperty.Register("VideoThumbnailProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public static readonly DependencyProperty VideoTitleProperty =
        DependencyProperty.Register("VideoTitleProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public static readonly DependencyProperty ChannelNameProperty =
        DependencyProperty.Register("ChannelNameProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public static readonly DependencyProperty ChannelThumbnailProperty =
        DependencyProperty.Register("ChannelThumbnailProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public int Identifier { get; set; }
        public string YoutubeURL { get; set; }
        public Time Duration { get; set; }
        public string VideoThumbnail
        {
            get { return (string)this.GetValue(VideoThumbnailProperty); }
            set { this.SetValue(VideoThumbnailProperty, value); }
        }
        public string VideoTitle
        {
            get { return (string)this.GetValue(VideoTitleProperty); }
            set { this.SetValue(VideoTitleProperty, value); }
        }
        public string ChannelName
        {
            get { return (string)this.GetValue(ChannelNameProperty); }
            set { this.SetValue(ChannelNameProperty, value); }
        }
        public string ChannelThumbnail
        {
            get { return (string)this.GetValue(ChannelThumbnailProperty); }
            set { this.SetValue(ChannelThumbnailProperty, value); }
        }
    }
}
