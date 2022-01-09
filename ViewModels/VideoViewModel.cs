using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Helpers;
using YoutubeCutter.Controls;
using YoutubeCutter.Models;
using System.Globalization;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

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
        public bool TimeBoxClicked = false;
        private int _identifier;
        private VideoPageInfo.NotifyChangesFunction _notifyChanges;
        private VideoPageInfo.SaveWorkProgressFunction _saveProgress;
        private VideoPageInfo.UpdateVideoPageInfo _updatePageInfo;
        private VideoPageInfo.RemovePageFunction _removePage;
        private VideoPageInfo.MoveToDownloadFunction _moveToDownload;
        private ObservableCollection<ClipItem> _menuItems = new ObservableCollection<ClipItem>();
        private string _downloadURL;
        private Time _duration = new Time();
        private string _startTime = "00:00:00";
        public string StartTime { get { return _startTime; } set { value = value.Trim(); if (TimeUtil.IsFormattedTime(value)) { _startTime = value; OnPropertyChanged("StartTime"); } } }
        private string _endTime = "00:00:00";
        public string EndTime { get { return _endTime; } set { value = value.Trim(); if (TimeUtil.IsFormattedTime(value)) { _endTime = value; OnPropertyChanged("EndTime"); } } }
        private string _downloadPath;
        private ICommand _toEndCommand;
        private ICommand _toStartCommand;
        private ICommand _validateCommand;
        private ICommand _addClipCommand;
        private ICommand _changeSelectedItemCommand;
        private ICommand _checkFilenameCommand;
        private ICommand _enterCommand;
        private ICommand _removeClipCommand;
        private ICommand _removePageCommand;
        private ICommand _downloadCommand;
        public ICommand DownloadCommand => _downloadCommand ?? (_downloadCommand = new RelayCommand(Download));
        public ICommand RemovePageCommand => _removePageCommand ?? (_removePageCommand = new RelayCommand(RemovePage));
        public ICommand RemoveClipCommand => _removeClipCommand ?? (_removeClipCommand = new RelayCommand<RoutedEventArgs>(RemoveClip));
        public ICommand EnterCommand => _enterCommand ?? (_enterCommand = new RelayCommand<KeyEventArgs>(Enter));
        public ICommand ToEndCommand => _toEndCommand ?? (_toEndCommand = new RelayCommand(ToEnd));
        public ICommand CheckFilenameCommand => _checkFilenameCommand ?? (_checkFilenameCommand = new RelayCommand<RoutedEventArgs>(CheckFilename));
        public ICommand ToStartCommand => _toStartCommand ?? (_toStartCommand = new RelayCommand(ToStart));
        public ICommand ValidateCommand => _validateCommand ?? (_validateCommand = new RelayCommand(Validate));
        public ICommand ChangeSelectedItemCommand => _changeSelectedItemCommand ?? (_changeSelectedItemCommand = new RelayCommand<RoutedEventArgs>(ChangeSelectedItem));
        public ICommand AddClipCommand => _addClipCommand ?? (_addClipCommand = new RelayCommand(AddClip));
        private int _identifierCount = 1;
        public bool IsDownloadEnabled { get { return (bool)App.Current.Properties["IsValidFfmpeg"] && ClipErrorCount == 0; } }
        public string DownloadButtonToolTip
        {
            get
            {
                if (!(bool)App.Current.Properties["IsValidFfmpeg"])
                {
                    return "You need a valid Ffmpeg to download clips.";
                }
                else if (ClipErrorCount > 0)
                {
                    return "You need to sort out the errors in the clips first.";
                }
                return null;
            }
        }
        private int ClipErrorCount = 0;
        private ClipItem _selectedItem;
        public ObservableCollection<ClipItem> MenuItems { get { return _menuItems; } }
        public bool CanRemoveClips { get { return _menuItems.Count > 1; } }
        public ClipItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    StartTime = TimeUtil.TimeToString(_selectedItem.StartTime);
                    EndTime = TimeUtil.TimeToString(_selectedItem.EndTime);
                    OnPropertyChanged("EndTime");
                    OnPropertyChanged("StartTime");
                }
                OnPropertyChanged("SelectedItem");
            }
        }
        public void Validate()
        {
            int tmp = SelectedItem.IsValidClip ? 0 : 1;
            SelectedItem.StartTime = TimeUtil.ParseTimeFromString(StartTime);
            SelectedItem.EndTime = TimeUtil.ParseTimeFromString(EndTime);
            SelectedItem.Validate(_duration);
            ClipErrorCount -= tmp - (SelectedItem.IsValidClip ? 0 : 1);
            OnPropertyChanged("IsDownloadEnabled");
            OnPropertyChanged("DownloadButtonToolTip");
        }
        public void ToEnd()
        {
            _endTime = TimeUtil.TimeToString(_duration);
            OnPropertyChanged("EndTime");
            Validate();
        }
        public void ToStart()
        {
            _startTime = "00:00:00";
            OnPropertyChanged("StartTime");
            Validate();
        }
        public bool IsVideoReady { get; set; } = false;
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
                    IsVideoReady = false;
                    MenuItems.Clear();
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
                    OnPropertyChanged("IsVideoReady");
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
            pageInfo.Duration = _duration;
            pageInfo.DownloadURL = _downloadURL;
            pageInfo.MenuItems = new string[MenuItems.Count * 3];
            for (int i = 0; i < MenuItems.Count; i++)
            {
                pageInfo.MenuItems[i * 3] = MenuItems[i].Filename;
                pageInfo.MenuItems[i * 3 + 1] = TimeUtil.TimeToString(MenuItems[i].StartTime);
                pageInfo.MenuItems[i * 3 + 2] = TimeUtil.TimeToString(MenuItems[i].EndTime);
            }
            _saveProgress(pageInfo);
        }
        private Dictionary<string, List<ClipItem>> _filenameDictionary = new Dictionary<string, List<ClipItem>>();
        public void OnNavigatedTo(object parameter)
        {
            //tood to think
            _menuItems = new ObservableCollection<ClipItem>();
            IsAvaliableVideo = false;
            _youtubeURL = "Youtube URL";
            if (parameter != null)
            {
                VideoPageInfo pageInfo = parameter as VideoPageInfo;
                _identifier = pageInfo.Identifier;
                _notifyChanges = VideoPageInfo.NotifyFunction;
                _saveProgress = VideoPageInfo.SaveFunction;
                _updatePageInfo = VideoPageInfo.UpdatePageInfoFunction;
                _removePage = VideoPageInfo.RemovePage;
                _moveToDownload = VideoPageInfo.MoveToDownload;
                YoutubeEmbedVideoURL = pageInfo.EmbedYoutubeURL;
                if (pageInfo.YoutubeURL != null)
                {
                    _youtubeURL = pageInfo.YoutubeURL;
                    if (pageInfo.MenuItems != null && pageInfo.MenuItems.Length > 0)
                    {
                        IsVideoReady = true;
                        _duration = pageInfo.Duration;
                        _downloadURL = pageInfo.DownloadURL;
                        for (int i = 0; i < pageInfo.MenuItems.Length / 3; i++)
                        {
                            AddClip(pageInfo.MenuItems[i * 3], TimeUtil.ParseTimeFromString(pageInfo.MenuItems[i * 3 + 1]), TimeUtil.ParseTimeFromString(pageInfo.MenuItems[i * 3 + 2]));
                            ClipErrorCount += MenuItems[i].IsValidClip ? 0 : 1;
                        }
                        SelectedItem = MenuItems[0];
                        OnPropertyChanged("IsVideoReady");
                    }
                }
                IsAvaliableVideo = pageInfo.EmbedYoutubeURL != null;
                _downloadPath = pageInfo.DownloadPath;
                OnPropertyChanged("YoutubeEmbedVideoURL");
                OnPropertyChanged("YoutubeURL");
                OnPropertyChanged("IsAvaliableVideo");
                OnPropertyChanged("IsDownloadEnabled");
                OnPropertyChanged("DownloadButtonToolTip");
            }
        }
        public async void GetVideoInformation()
        {
            await Task.Run(() =>
            {
                _webClient.GetVideoInfo(_videoInformation, _youtubeID);
                if (_videoInformation.AuthorURL != null)
                {
                    var p = Process.Start(
                        new ProcessStartInfo(App.Current.Properties["YoutubedlPath"] as string, _youtubeURL + " -g --get-duration")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        }
                    );
                    p.WaitForExit();
                    string[] result = p.StandardOutput.ReadToEnd().TrimEnd().Split("\n");
                    _downloadURL = result[0];
                    TimeUtil.ParseIrregularTimeFromString(result[result.Length - 1], _duration);
                    //todo handle parsing error, https://www.youtube.com/watch?v=x8VYWazR5mE
                    VideoPageInfo pageInfo = new VideoPageInfo();
                    pageInfo.Duration = _duration;
                    pageInfo.DownloadURL = _downloadURL;
                    pageInfo.MenuItems = new string[] { "Clip " + _identifierCount, "00:00:00", TimeUtil.TimeToString(_duration) };
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        IsVideoReady = true;
                        OnPropertyChanged("IsVideoReady");
                        AddClip();
                        SelectedItem = MenuItems[0];
                        _updatePageInfo(_identifier, pageInfo);
                    });
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
                    _downloadPath = (string)App.Current.Properties["DownloadPath"];
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    _downloadPath = DownloadManager.GetDownloadPath(_videoInformation.VideoTitle, _videoInformation.ChannelName);
                    _notifyChanges(_identifier, _videoInformation);
                });
            });
        }
        public void AddClip()
        {
            AddClip("Clip " + _identifierCount, new Time { Hour = 0, Second = 0, Minute = 0 }, _duration);
        }
        public void RemoveClip(RoutedEventArgs eventArgs)
        {
            ClipItem clip = FindClipByIdentifier((eventArgs.Source as Button).Tag as string);
            _filenameDictionary[clip.Filename].Remove(clip);
            UpdateClipsWithFilename(clip.Filename);
            if (SelectedItem.Identifier == clip.Identifier)
            {
                int index = MenuItems.IndexOf(clip);
                if (index > 0)
                {
                    SelectedItem = MenuItems[index - 1];
                }
                else
                {
                    SelectedItem = MenuItems[1];
                }
            }
            MenuItems.Remove(clip);
            ClipErrorCount -= clip.IsValidClip ? 0 : 1;
            OnPropertyChanged("IsDownloadEnabled");
            OnPropertyChanged("DownloadButtonToolTip");
            OnPropertyChanged("CanRemoveClips");
        }
        private void AddClip(string filename, Time startTime, Time endTime)
        {
            MenuItems.Add(new ClipItem() { Identifier = "" + _identifierCount++, Filename = filename, EndTime = endTime, StartTime = startTime });
            MenuItems[MenuItems.Count - 1].Validate(_duration);
            if (!_filenameDictionary.ContainsKey(filename))
            {
                _filenameDictionary.Add(filename, new List<ClipItem>());
            }
            _filenameDictionary[filename].Add(MenuItems[MenuItems.Count - 1]);
            UpdateClipsWithFilename(filename);
            OnPropertyChanged("CanRemoveClips");
        }
        public void ChangeSelectedItem(RoutedEventArgs eventArgs)
        {
            SelectedItem = FindClipByIdentifier((eventArgs.Source as TextBox).Tag as string);
        }
        private ClipItem FindClipByIdentifier(string identifier)
        {
            foreach (ClipItem clip in MenuItems)
            {
                if (clip.Identifier == identifier)
                {
                    return clip;
                }
            }
            return null;
        }
        public void CheckFilename(RoutedEventArgs eventArgs)
        {
            UpdateClipFilename((eventArgs.Source as TextBox).Text.Trim(), FindClipByIdentifier((eventArgs.Source as TextBox).Tag as string));
        }
        private void UpdateClipFilename(string filename, ClipItem clip)
        {
            _filenameDictionary[clip.Filename].Remove(clip);
            UpdateClipsWithFilename(clip.Filename);
            if (!_filenameDictionary.ContainsKey(filename))
            {
                _filenameDictionary.Add(filename, new List<ClipItem>());
            }
            _filenameDictionary[filename].Add(clip);
            clip.Filename = filename;
            UpdateClipsWithFilename(clip.Filename);
            int tmp = clip.IsValidClip ? 0 : 1;
            clip.DoesFileExist(File.Exists(_downloadPath + filename));
            ClipErrorCount -= tmp - (clip.IsValidClip ? 0 : 1);
            OnPropertyChanged("IsDownloadEnabled");
            OnPropertyChanged("DownloadButtonToolTip");
        }
        private void UpdateClipsWithFilename(string filename)
        {
            bool isUnique = _filenameDictionary[filename].Count == 1;
            int tmp;
            foreach (ClipItem clip in _filenameDictionary[filename])
            {
                tmp = clip.IsValidClip ? 0 : 1;
                clip.IsFilenameUnique(isUnique);
                ClipErrorCount -= tmp - (clip.IsValidClip ? 0 : 1);
            }
        }
        public void Enter(KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter)
            {
                UpdateClipFilename((eventArgs.Source as TextBox).Text.Trim(), FindClipByIdentifier((eventArgs.Source as TextBox).Tag as string));
                Keyboard.ClearFocus();
            }
        }
        private void RemovePage()
        {
            _removePage(_identifier);
        }
        private void Download()
        {
            _moveToDownload(_identifier, MenuItems);
        }
    }
}
