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

namespace YoutubeCutter.ViewModels
{
    public class VideoViewModel : ObservableObject, INavigationAware
    {
        private Regex _youtubeRegex = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([^\?\&\#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string _youtubeID;
        private string _youtubeURL;
        private WebClient _webClient = WebClient.Instance;
        private string[] _videoInformation;
        public bool IsAvaliableVideo { get; set; }
        //thumbnail https://img.youtube.com/vi/<ID>/0.jpg
        public string YoutubeEmbedVideoURL { get; set; }

        private int _identifier;
        private VideoPageInfo.NotifyChangesFunction _notifyChanges;
        public string YoutubeVideoURL
        {
            get
            {
                return _youtubeURL;
            }
            set
            {
                _youtubeURL = value.Trim();
                Match match = _youtubeRegex.Match(value);
                IsAvaliableVideo = false;
                if (match.Success)
                {   
                    _videoInformation = _webClient.GetVideoInfo(match.Groups[1].ToString());
                    if (_videoInformation[0] != "")
                    {
                        IsAvaliableVideo = true;
                        _youtubeID = match.Groups[1].ToString();
                        _youtubeURL = "https://www.youtube.com/watch?v=" + _youtubeID;
                        YoutubeEmbedVideoURL = "https://www.youtube.com/embed/" + _youtubeID;
                        _notifyChanges(_identifier, _videoInformation[0],_videoInformation[1], _videoInformation[2]);
                    }
                }
                OnPropertyChanged("YoutubeEmbedVideoURL");
                OnPropertyChanged("IsAvaliableVideo");
            }
        }

        public void OnNavigatedFrom()
        {
            //add download to manager
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
                _notifyChanges = pageInfo.function;
            }
            
        }
    }
}
