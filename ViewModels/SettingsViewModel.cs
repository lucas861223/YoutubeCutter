using System;
using System.Windows.Input;
using System.ComponentModel;

using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using YoutubeCutter.Contracts.Services;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;

using System.IO;
using System.Text.Json;


namespace YoutubeCutter.ViewModels
{
    // TODO WTS: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
    public class SettingsViewModel : ObservableObject, INavigationAware
    {
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private AppTheme _theme;
        private string _versionDescription;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;
        public bool IsInvalidYoutubeDL { get; set; }
        public bool IsInvalidFfmpeg { get; set; }
        private Languages _language;
        public Languages Language
        {
            get { return _language; }
            set
            {
                {
                    _language = value;
                    CultureResources.ChangeCulture(_language);
                }
            }
        }
        public string Test { get; set; }
        private string _youtubeDLPath;
        public string YoutubeDLPath
        {
            get { return _youtubeDLPath; }
            set
            {
                _youtubeDLPath = value;
                IsInvalidYoutubeDL = !_youtubeDLPath.EndsWith("\\youtube-dl.exe");
                App.Current.Properties["YoutubedlPath"] = _youtubeDLPath;
                OnPropertyChanged("YoutubeDLPath");
                OnPropertyChanged("IsInvalidYoutubeDL");
            }
        }
        private string _ffmpegPath;
        public string FFmpegPath
        {
            get { return _ffmpegPath; }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                _ffmpegPath = value;
                IsInvalidFfmpeg = !_ffmpegPath.EndsWith("\\ffmpeg.exe");
                OnPropertyChanged("FFmpegPath");
                OnPropertyChanged("IsInvalidFfmpeg");
            }
        }
        public int FontSize { get; set; }

        public AppTheme Theme
        {
            get { return _theme; }
            set { SetProperty(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { SetProperty(ref _versionDescription, value); }
        }

        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));

        public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new RelayCommand(OnPrivacyStatement));

        public SettingsViewModel(IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService)
        {
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
        }

        public void OnNavigatedTo(object parameter)
        {
            VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
            Language = (Languages)App.Current.Properties["Language"];
            YoutubeDLPath = (string)App.Current.Properties["YoutubedlPath"];
            FFmpegPath = (string)App.Current.Properties["FfmpegPath"];
            FontSize = (int)App.Current.Properties["FontSize"];
        }

        public void OnNavigatedFrom()
        {
            App.Current.Properties["Language"] = Language;
            App.Current.Properties["YoutubedlPath"] = YoutubeDLPath;
            App.Current.Properties["FfmpegPath"] = FFmpegPath;
            App.Current.Properties["FontSize"] = FontSize;
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser((string)App.Current.Properties["PrivacyStatement"]);
    }
}
