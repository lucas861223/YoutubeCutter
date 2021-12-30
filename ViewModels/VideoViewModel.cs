using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Helpers;
using YoutubeCutter.Models;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Controls;

namespace YoutubeCutter.ViewModels
{
    public class VideoViewModel : ObservableObject, INavigationAware
    {
        private Regex _youtubeRegex = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([^\?\&\#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _channelThumbnailRegex = new Regex(@"channelId[^a-zA-Z0-9-]+(?<channelID>[a-zA-Z0-9-_]+).*avatar[^=]+(?<thumbnail>https\:\/\/yt3.ggpht.com[^=]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string _youtubeID;
        private string _youtubeURL;
        private WebClient _webClient = WebClient.Instance;
        private VideoInformation _videoInformation = new();
        public bool IsAvaliableVideo { get; set; }
        public string YoutubeEmbedVideoURL { get; set; }

        private int _identifier;
        private VideoPageInfo.NotifyChangesFunction _notifyChanges;
        private VideoPageInfo.SaveWorkProgressFunction _saveProgress;
        public string YoutubeVideoURL
        {
            get
            {
                return _youtubeURL;
            }
            set
            {
                if (_youtubeURL != value.Trim())
                {
                    _videoInformation = new VideoInformation();
                    _youtubeURL = value.Trim();
                    IsAvaliableVideo = false;
                    if (_youtubeURL == "" || _youtubeURL == "Youtube URL")
                    {
                        _youtubeURL = "Youtube URL";
                        _notifyChanges(_identifier, _videoInformation);
                    }
                    else
                    {
                        Match match = _youtubeRegex.Match(_youtubeURL);
                        if (match.Success)
                        {
                            IsAvaliableVideo = true;
                            _youtubeID = match.Groups[1].ToString();
                            _youtubeURL = "https://www.youtube.com/watch?v=" + _youtubeID;
                            YoutubeEmbedVideoURL = "https://www.youtube.com/embed/" + _youtubeID;
                            GetVideoInformation();
                        }
                        OnPropertyChanged("YoutubeEmbedVideoURL");
                        OnPropertyChanged("YoutubeVideoURL");
                    }
                    OnPropertyChanged("IsAvaliableVideo");
                }
            }
        }

        public void OnNavigatedFrom()
        {
            //add download to manager
            VideoPageInfo pageInfo = new VideoPageInfo();
            pageInfo.Identifier = _identifier;
            pageInfo.EmbedYoutubeURL = _youtubeURL == "Youtube URL" ? null : YoutubeEmbedVideoURL;
            pageInfo.YoutubeURL = _youtubeURL;
            _saveProgress(pageInfo);
        }

        public void OnNavigatedTo(object parameter)
        {
            //tood to think
            IsAvaliableVideo = false;
            _youtubeURL = "Youtube URL";
            if (parameter != null)
            {
                VideoPageInfo pageInfo = parameter as VideoPageInfo;
                _identifier = pageInfo.Identifier;
                _notifyChanges = VideoPageInfo.NotifyFunction;
                _saveProgress = VideoPageInfo.SaveFunction;
                YoutubeEmbedVideoURL = pageInfo.EmbedYoutubeURL;
                if (pageInfo.YoutubeURL != null)
                {
                    _youtubeURL = pageInfo.YoutubeURL;
                }
                IsAvaliableVideo = pageInfo.EmbedYoutubeURL != null;
                OnPropertyChanged("YoutubeEmbedVideoURL");
                OnPropertyChanged("YoutubeURL");
                OnPropertyChanged("IsAvaliableVideo");
            }
        }
        public async void GetVideoInformation()
        {
            await Task.Run(() =>
            {
                _webClient.GetVideoInfo(_videoInformation, _youtubeID);
                if (_videoInformation.AuthorURL != null)
                {
                    _videoInformation.VideoID = _youtubeID;
                    string data = _webClient.GetHTML(_videoInformation.AuthorURL);
                    Match match = _channelThumbnailRegex.Match(data);
                    if (match.Success)
                    {
                        _videoInformation.ChannelThumbnailURL = match.Groups["thumbnail"].ToString() + "=s48-c-k-c0x00ffffff-no-rj-mo";
                        _videoInformation.ChannelID = match.Groups["channelID"].ToString();
                    }
                    _videoInformation.VideoThumbnailURL = "https://i.ytimg.com/vi/" + _youtubeID + "/mqdefault.jpg";
                    _videoInformation.VideoThumbnailLocation = DownloadManager.DownloadThumbnail(_videoInformation.VideoThumbnailURL, _videoInformation.VideoID);
                    _videoInformation.ChannelThumbnailLocation = DownloadManager.DownloadThumbnail(_videoInformation.ChannelThumbnailURL, _videoInformation.ChannelID);
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    _notifyChanges(_identifier, _videoInformation);
                });
            });

        }
    }
}
