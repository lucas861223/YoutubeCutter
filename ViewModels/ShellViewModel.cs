using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

using MahApps.Metro.Controls;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using YoutubeCutter.Contracts.Services;
using YoutubeCutter.Properties;

using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using YoutubeCutter.Controls;

namespace YoutubeCutter.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private HamburgerMenuItem _selectedMenuItem;
        private HamburgerMenuItem _selectedOptionsMenuItem;
        private RelayCommand _goBackCommand;
        private ICommand _menuItemInvokedCommand;
        private ICommand _optionsMenuItemInvokedCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;
        private int _alreadyAvaliableIdentifier = 0;
        private int _identifierCount = 1;
        private WebClient _webClient = WebClient.Instance;
        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get
            {
                return _isPaneOpen;
            }
            set
            {
                _isPaneOpen = value;
                OnPropertyChanged("IsPaneOpen");
            }
        }

        public HamburgerMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { SetProperty(ref _selectedMenuItem, value); }
        }

        public HamburgerMenuItem SelectedOptionsMenuItem
        {
            get { return _selectedOptionsMenuItem; }
            set { SetProperty(ref _selectedOptionsMenuItem, value); }
        }

        // TODO WTS: Change the icons and titles for all HamburgerMenuItems here.
        public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = Resources.ShellMainPage, Glyph = "\uE8A5", TargetPageType = typeof(MainViewModel) },
            new HamburgerMenuGlyphItem() { Label = Resources.ShellDownloadsPage, Glyph = "\uE8A5", TargetPageType = typeof(DownloadsViewModel) },
            new HamburgerMenuGlyphItem() { Label = Resources.ShellAddNewDownload, Glyph = "\uE8A5"},
        };

        public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = Resources.ShellSettingsPage, Glyph = "\uE713", TargetPageType = typeof(SettingsViewModel) }
        };

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack, CanGoBack));

        public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new RelayCommand(OnMenuItemInvoked));

        public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new RelayCommand(OnOptionsMenuItemInvoked));

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

        public ShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            VideoPageInfo.NotifyFunction = NotifyChanges;
            VideoPageInfo.RemovePage = RemoveVideoPage;
            VideoPageInfo.MoveToDownload = MoveToDownload;
            ViewModelResolver.AddViewModel(typeof(DownloadsViewModel).FullName, new DownloadsViewModel());
        }

        private void OnLoaded()
        {
            _navigationService.Navigated += OnNavigated;
        }

        private void OnUnloaded()
        {
            _navigationService.Navigated -= OnNavigated;
        }

        private bool CanGoBack()
            => _navigationService.CanGoBack;

        private void OnGoBack()
            => _navigationService.GoBack();

        private void OnMenuItemInvoked()
        {
            NavigateTo(SelectedMenuItem.TargetPageType);
        }

        private void OnOptionsMenuItemInvoked()
            => NavigateTo(SelectedOptionsMenuItem.TargetPageType);

        private void NavigateTo(Type targetViewModel)
        {
            if (targetViewModel == typeof(VideoViewModel))
            {
                //VideoPageInfo pageInfo = new VideoPageInfo();
                //((VideosHamburgerMenuItem)SelectedMenuItem).loadPageInfo(pageInfo);
                _navigationService.NavigateTo(targetViewModel.FullName, ViewModelResolver.GetViewModel(Convert.ToString(((VideosHamburgerMenuItem)SelectedMenuItem).Identifier)));
            }
            else if (targetViewModel == null)
            {
                VideoPageInfo pageInfo = new VideoPageInfo();
                VideosHamburgerMenuItem item = null;
                if (_alreadyAvaliableIdentifier != 0)
                {
                    item = FindItemWithIdentifier(_alreadyAvaliableIdentifier);
                    if (item != null)
                    {
                        SelectedMenuItem = item;
                        _navigationService.NavigateTo(typeof(VideoViewModel).FullName, ViewModelResolver.GetViewModel(Convert.ToString(item.Identifier)));
                    }
                }
                if (item == null)
                {
                    pageInfo.Identifier = _identifierCount++;
                    _alreadyAvaliableIdentifier = pageInfo.Identifier;
                    //shellemptydownloadpage
                    MenuItems.Add(new VideosHamburgerMenuItem()
                    {
                        Label = Resources.ShellDownloadsPage,
                        Identifier = pageInfo.Identifier,
                        ChannelName = "",
                        TargetPageType = typeof(VideoViewModel),
                    });
                    SelectedMenuItem = MenuItems[MenuItems.Count - 1];
                    VideoViewModel viewModel = new VideoViewModel();
                    ViewModelResolver.AddViewModel(Convert.ToString(pageInfo.Identifier), viewModel);
                    _navigationService.NavigateTo(typeof(VideoViewModel).FullName, viewModel, pageInfo);
                }
            }
            else if (ViewModelResolver.GetViewModel(targetViewModel.FullName) != null)
            {
                _navigationService.NavigateTo(targetViewModel.FullName, ViewModelResolver.GetViewModel(targetViewModel.FullName));
            }
            else
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }
        private VideosHamburgerMenuItem FindItemWithIdentifier(int identifier)
        {
            for (int i = 3; i < MenuItems.Count; i++)
            {
                if (((VideosHamburgerMenuItem)MenuItems[i]).Identifier == identifier)
                {
                    return (VideosHamburgerMenuItem)MenuItems[i];
                }
            }
            return null;
        }
        private void OnNavigated(object sender, string viewModelName)
        {
            var item = MenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            //if (item != null)
            //{
            //    SelectedMenuItem = item;
            //}
            if (item == null)
            {
                SelectedOptionsMenuItem = OptionMenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            }

            GoBackCommand.NotifyCanExecuteChanged();
        }
        private void NotifyChanges(int identifier, VideoInformation videoInformation)
        {
            VideosHamburgerMenuItem item = FindItemWithIdentifier(identifier); ;
            if (item != null)
            {
                if (videoInformation.VideoTitle != null)
                {
                    item.VideoTitle = videoInformation.VideoTitle;
                    item.YoutubeURL = "https://www.youtube.com/watch?v=" + videoInformation.VideoID;
                    item.ChannelName = videoInformation.ChannelName;
                    item.VideoThumbnail = videoInformation.VideoThumbnailLocation;
                    item.ChannelThumbnail = videoInformation.ChannelThumbnailLocation;
                    item.ToolTip = videoInformation.VideoTitle + "\n" + videoInformation.ChannelName;
                    if (_alreadyAvaliableIdentifier == identifier)
                    {
                        _alreadyAvaliableIdentifier = 0;
                    }
                }
                else
                {
                    item.Label = Resources.ShellDownloadsPage;
                    item.ChannelName = null;
                    item.VideoThumbnail = "";
                    item.ChannelThumbnail = "";
                    item.EmbedYoutubeURL = null;
                    item.YoutubeURL = null;
                    _alreadyAvaliableIdentifier = identifier;
                }
            }
        }
        private void MoveToDownload(int identifier, ObservableCollection<ClipItem> clips)
        {
            DownloadItem[] downloadItems = new DownloadItem[clips.Count];
            VideosHamburgerMenuItem videoMenuItem = FindItemWithIdentifier(identifier);
            string downloadPath = DownloadManager.GetDownloadPath(videoMenuItem.VideoTitle, videoMenuItem.ChannelName);
            for (int i = 0; i < clips.Count; i++)
            {
                downloadItems[i] = new DownloadItem();
                downloadItems[i].Filename = clips.ElementAt(i).Filename;
                downloadItems[i].VideoTitle = videoMenuItem.VideoTitle;
                downloadItems[i].ChannelName = videoMenuItem.ChannelName;
                downloadItems[i].YoutubeURL = videoMenuItem.YoutubeURL;
                downloadItems[i].StartTime = clips.ElementAt(i).StartTime;
                downloadItems[i].EndTime = clips.ElementAt(i).EndTime;
                downloadItems[i].Directory = downloadPath;
                downloadItems[i].ChannelThumbnail = videoMenuItem.ChannelThumbnail;
                downloadItems[i].VideoThumbnail = videoMenuItem.VideoThumbnail.Replace("thumbnail", "hqthumbnail");
            }
            MenuItems.Remove(videoMenuItem);
            ViewModelResolver.RemoveViewModel(Convert.ToString(identifier));
            SelectedMenuItem = MenuItems[1];
            _navigationService.NavigateTo(typeof(DownloadsViewModel).FullName, ViewModelResolver.GetViewModel(typeof(DownloadsViewModel).FullName), downloadItems);
        }
        private void RemoveVideoPage(int identifier)
        {
            VideosHamburgerMenuItem item = FindItemWithIdentifier(identifier);
            int index = MenuItems.IndexOf(item);
            if ((SelectedMenuItem as VideosHamburgerMenuItem).Identifier == identifier)
            {
                if (index > 3)
                {
                    SelectedMenuItem = MenuItems[index - 1];
                }
                else if (MenuItems.Count > 4)
                {
                    SelectedMenuItem = MenuItems[4];
                }
                else
                {
                    SelectedMenuItem = MenuItems[0];
                }
            }
            ViewModelResolver.RemoveViewModel(Convert.ToString(identifier));
            MenuItems.RemoveAt(index);
        }
    }
}
