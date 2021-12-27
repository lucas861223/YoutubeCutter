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
                IsAvaliableVideo = false;
                if (_youtubeURL == "" || _youtubeURL == "Youtube URL")
                {
                    _youtubeURL = "Youtube URL";
                    _notifyChanges(_identifier, "", "", "");
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
                }
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
        public async void GetVideoInformation()
        {
            await Task.Run(() =>
            {
                _videoInformation = _webClient.GetVideoInfo(_youtubeID);
            });
            _notifyChanges(_identifier, _videoInformation[0], _videoInformation[1], _videoInformation[2]);
        }
    }
}
