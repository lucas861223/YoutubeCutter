using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;
using YoutubeCutter.Helpers;

using YoutubeCutter.Models;
using YoutubeCutter.ViewModels;
using YoutubeCutter.Core.Models;

namespace YoutubeCutter.Controls
{
    class VideosHamburgerMenuItem : HamburgerMenuItem
    {
        public string[] MenuItems { get; set; }
        public Time Duration { get; set; }
        public string YoutubeURL { get; set; }
        public string EmbedYoutubeURL { get; set; }
        public int Identifier { get; set; }
        public static readonly DependencyProperty VideoThumbnailProperty =
        DependencyProperty.Register("VideoThumbnailProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public string VideoThumbnail
        {
            get { return (string)this.GetValue(VideoThumbnailProperty); }
            set { this.SetValue(VideoThumbnailProperty, value); }
        }

        public static readonly DependencyProperty VideoTitleProperty =
        DependencyProperty.Register("VideoTitleProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public string VideoTitle
        {
            get { return (string)this.GetValue(VideoTitleProperty); }
            set { this.SetValue(VideoTitleProperty, value); }
        }

        public static readonly DependencyProperty ChannelNameProperty =
        DependencyProperty.Register("ChannelNameProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public string ChannelName
        {
            get { return (string)this.GetValue(ChannelNameProperty); }
            set { this.SetValue(ChannelNameProperty, value); }
        }

        public static readonly DependencyProperty ChannelThumbnailProperty =
        DependencyProperty.Register("ChannelThumbnailProperty",
            typeof(string),
            typeof(VideosHamburgerMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));
        public string ChannelThumbnail
        {
            get { return (string)this.GetValue(ChannelThumbnailProperty); }
            set { this.SetValue(ChannelThumbnailProperty, value); }
        }
    }
}
