using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;

namespace YoutubeCutter.Controls
{
    class VideosHamburgerMenuGlyphItem : HamburgerMenuGlyphItem
    {
        public static readonly DependencyProperty VideoThumbnailProperty =
        DependencyProperty.Register("VideoThumbnailProperty",
            typeof(BitmapImage),
            typeof(VideosHamburgerMenuGlyphItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public BitmapImage VideoThumbnail
        {
            get { return (BitmapImage)this.GetValue(VideoThumbnailProperty); }
            set { this.SetValue(VideoThumbnailProperty, value); }
        }

        public static readonly DependencyProperty VideoTitleProperty =
        DependencyProperty.Register("VideoTitleProperty",
            typeof(string),
            typeof(VideosHamburgerMenuGlyphItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public string VideoTitle
        {
            get { return (string)this.GetValue(VideoTitleProperty); }
            set { this.SetValue(VideoTitleProperty, value); }
        }

        public static readonly DependencyProperty ChannelNameProperty =
        DependencyProperty.Register("ChannelNameProperty",
            typeof(string),
            typeof(VideosHamburgerMenuGlyphItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));

        public string ChannelName
        {
            get { return (string)this.GetValue(ChannelNameProperty); }
            set { this.SetValue(ChannelNameProperty, value); }
        }

        public static readonly DependencyProperty ChannelThumbnailProperty =
        DependencyProperty.Register("ChannelThumbnailProperty",
            typeof(BitmapImage),
            typeof(VideosHamburgerMenuGlyphItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => { }));
        public BitmapImage ChannelThumbnail
        {
            get { return (BitmapImage)this.GetValue(ChannelThumbnailProperty); }
            set { this.SetValue(ChannelThumbnailProperty, value); }
        }
    }
}
