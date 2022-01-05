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
            VideoPageInfo.SaveFunction = SaveWorkProgress;
            VideoPageInfo.UpdatePageInfoFunction = UpdatePageInfo;
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
                VideoPageInfo pageInfo = new VideoPageInfo();
                ((VideosHamburgerMenuItem)SelectedMenuItem).loadPageInfo(pageInfo);
                _navigationService.NavigateTo(targetViewModel.FullName, pageInfo);
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
                        item.loadPageInfo(pageInfo);
                        SelectedMenuItem = item;
                        _navigationService.NavigateTo(typeof(VideoViewModel).FullName, pageInfo);
                    }
                }
                if (item == null)
                {
                    pageInfo.Identifier = _identifierCount++;
                    _alreadyAvaliableIdentifier = pageInfo.Identifier;
                    //shellemptydownloadpage
                    MenuItems.Add(new VideosHamburgerMenuItem() { Label = Resources.ShellDownloadsPage, Identifier = pageInfo.Identifier, ChannelName = "", TargetPageType = typeof(VideoViewModel) });
                    SelectedMenuItem = MenuItems[MenuItems.Count - 1];
                    _navigationService.NavigateTo(typeof(VideoViewModel).FullName, pageInfo);
                }
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
        public void SaveWorkProgress(VideoPageInfo pageInfo)
        {
            VideosHamburgerMenuItem item = FindItemWithIdentifier(pageInfo.Identifier);
            item.EmbedYoutubeURL = pageInfo.EmbedYoutubeURL;
            item.YoutubeURL = pageInfo.YoutubeURL;
            item.Duration = pageInfo.Duration;
            item.DownloadURL = pageInfo.DownloadURL;
            item.MenuItems = pageInfo.MenuItems;
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
        private void UpdatePageInfo(int identifier, VideoPageInfo pageInfo)
        {
            VideosHamburgerMenuItem item = FindItemWithIdentifier(identifier);
            if (item != null)
            {
                item.Duration = pageInfo.Duration;
                item.DownloadURL = pageInfo.DownloadURL;
                item.MenuItems = pageInfo.MenuItems;
            }
        }
    }
}
