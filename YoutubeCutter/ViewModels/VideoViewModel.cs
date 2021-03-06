using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using YoutubeCutter.Core.Helpers;
using YoutubeCutter.Core.Models;
using YoutubeCutter.Controls;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Helpers;
using YoutubeCutter.Models;


namespace YoutubeCutter.ViewModels
{
    public class VideoViewModel : ObservableObject, INavigationAware
    {
        private int _identifier;
        private int _identifierCount = 1;
        private int _clipErrorCount = 0;

        private string _youtubeID;
        private string _youtubeURL = "Youtube URL";
        private string _startTime = "00:00:00";
        private string _endTime = "00:00:00";
        private string _downloadPath;

        private VideoPageInfo.NotifyChangesFunction _notifyChanges;
        private VideoPageInfo.RemovePageFunction _removePage;
        private VideoPageInfo.MoveToDownloadFunction _moveToDownload;
        private ObservableCollection<ClipItem> _menuItems = new ObservableCollection<ClipItem>();
        private WebClient _webClient = WebClient.Instance;
        private ClipItem _selectedItem;
        private Time _duration = new Time();
        private VideoInformation _videoInformation = new();
        private Regex _youtubeRegex = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([^\?\&\#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _channelThumbnailRegex = new Regex(@"channelId[^a-zA-Z0-9-]+(?<channelID>[a-zA-Z0-9-_]+).*avatar[^=]+(?<thumbnail>https\:\/\/yt3.ggpht.com[^=]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Dictionary<string, List<ClipItem>> _filenameDictionary = new Dictionary<string, List<ClipItem>>();

        public bool IsAvaliableVideo { get; set; }
        public string YoutubeEmbedVideoURL { get; set; }
        public string StartTime { get { return _startTime; } set { value = value.Trim(); if (TimeUtil.IsFormattedTime(value)) { _startTime = value; OnPropertyChanged("StartTime"); } } }
        public string EndTime { get { return _endTime; } set { value = value.Trim(); if (TimeUtil.IsFormattedTime(value)) { _endTime = value; OnPropertyChanged("EndTime"); } } }
        public bool IsDownloadEnabled { get { return (bool)App.Current.Properties["IsValidFfmpeg"] && _clipErrorCount == 0; } }
        public bool IsVideoReady { get; set; } = false;
        public bool CanRemoveClips { get { return _menuItems.Count > 1; } }
        public string DownloadButtonToolTip
        {
            get
            {
                if (!(bool)App.Current.Properties["IsValidFfmpeg"])
                {
                    return "You need a valid Ffmpeg to download clips.";
                }
                else if (_clipErrorCount > 0)
                {
                    return "You need to sort out the errors in the clips first.";
                }
                return null;
            }
        }
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
        public ObservableCollection<ClipItem> MenuItems { get { return _menuItems; } }
        
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

        public void Validate()
        {
            int tmp = SelectedItem.IsValidClip ? 0 : 1;
            SelectedItem.StartTime = TimeUtil.ParseTimeFromString(StartTime);
            SelectedItem.EndTime = TimeUtil.ParseTimeFromString(EndTime);
            SelectedItem.Validate(_duration);
            _clipErrorCount -= tmp - (SelectedItem.IsValidClip ? 0 : 1);
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
        public void OnNavigatedFrom()
        {
        }
        public void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                VideoPageInfo pageInfo = parameter as VideoPageInfo;
                _identifier = pageInfo.Identifier;
                _notifyChanges = VideoPageInfo.NotifyFunction;
                _removePage = VideoPageInfo.RemovePage;
                _moveToDownload = VideoPageInfo.MoveToDownload;
            }
        }
        public async void GetVideoInformation()
        {
            await Task.Run(() =>
            {
                _webClient.GetVideoInfo(_videoInformation, _youtubeID);
                if (_videoInformation.AuthorURL != null)
                {
                    //make warning if no youtube-dl set 
                    var p = Process.Start(
                        new ProcessStartInfo(App.Current.Properties["YoutubedlPath"] as string, _youtubeURL + " --get-duration")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        }
                    );
                    p.WaitForExit();
                    string result = p.StandardOutput.ReadToEnd().TrimEnd();

                    //todo handle parsing error, https://www.youtube.com/watch?v=x8VYWazR5mE
                    if (TimeUtil.ParseIrregularTimeFromString(result, _duration))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            _downloadPath = DownloadManager.GetDownloadPath(_videoInformation.VideoTitle, _videoInformation.ChannelName);
                            IsVideoReady = true;
                            OnPropertyChanged("IsVideoReady");
                            AddClip();
                            SelectedItem = MenuItems[0];
                        });
                    }
                    _videoInformation.VideoID = _youtubeID;
                    string data = _webClient.GetHTML(_videoInformation.AuthorURL);
                    Match match = _channelThumbnailRegex.Match(data);
                    if (match.Success)
                    {
                        _videoInformation.ChannelThumbnailURL = match.Groups["thumbnail"].ToString() + "=s48-c-k-c0x00ffffff-no-rj-mo";
                        _videoInformation.ChannelID = match.Groups["channelID"].ToString();
                    }
                    _videoInformation.VideoThumbnailURL = "https://i.ytimg.com/vi/" + _youtubeID + "/mqdefault.jpg";
                    _videoInformation.VideoThumbnailLocation = DownloadManager.DownloadThumbnail(_videoInformation.VideoThumbnailURL, _videoInformation.VideoID, "thumbnail.jpg");
                    _videoInformation.ChannelThumbnailLocation = DownloadManager.DownloadThumbnail(_videoInformation.ChannelThumbnailURL, _videoInformation.ChannelID, "thumbnail.jpg");
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    _notifyChanges(_identifier, _videoInformation);
                });
                if (_videoInformation.AuthorURL != null)
                {
                    _ = DownloadManager.DownloadThumbnail("https://i.ytimg.com/vi/" + _youtubeID + "/maxresdefault.jpg", _videoInformation.VideoID, "hqthumbnail.jpg");
                }
            });
        }
        public void AddClip()
        {
            while (File.Exists(_downloadPath + "Clip " + _identifierCount + ".mp4"))
            {
                _identifierCount++;
            }
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
            _clipErrorCount -= clip.IsValidClip ? 0 : 1;
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
            SelectedItem = MenuItems[MenuItems.Count - 1];
            OnPropertyChanged("SelectedItem");
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
            int tmp = clip.IsValidClip ? 0 : 1;
            clip.Filename = filename;
            UpdateClipsWithFilename(clip.Filename);
            clip.DoesFileExist(File.Exists(_downloadPath + filename + ".mp4"));
            _clipErrorCount -= tmp - (clip.IsValidClip ? 0 : 1);
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
                _clipErrorCount -= tmp - (clip.IsValidClip ? 0 : 1);
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
