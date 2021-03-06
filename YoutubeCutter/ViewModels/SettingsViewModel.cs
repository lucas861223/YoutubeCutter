using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using YoutubeCutter.Core.Models;
using YoutubeCutter.Contracts.Services;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;


namespace YoutubeCutter.ViewModels
{
    // TODO WTS: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
    public class SettingsViewModel : ObservableObject, INavigationAware
    {
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;

        private bool _categorizeByChannel;
        private bool _categorizeByVideo;
        private bool _categorizeByDate;
        private string _versionDescription;
        private string _youtubeDLPath;
        private string _ffmpegPath;

        private AppTheme _theme;
        private Languages _language;

        public int FontSize { get; set; }
        public bool IsInvalidYoutubeDL { get; set; }
        public bool IsInvalidFfmpeg { get; set; }
        public bool IsVerifyingYoutubeDL { get; set; } = false;
        public bool IsVerifyingFfmpeg { get; set; } = false;
        public bool ShowNamingWarning { get { return CategorizeByChannel || CategorizeByVideo; } }
        public bool CategorizeByChannel
        {
            get
            {
                return _categorizeByChannel;
            }
            set
            {
                _categorizeByChannel = value;
                OnPropertyChanged("CurrentFormat");
                OnPropertyChanged("ShowNamingWarning");
                App.Current.Properties["CategorizeByChannel"] = _categorizeByChannel;
            }
        }
        public bool CategorizeByVideo
        {
            get
            {
                return _categorizeByVideo;
            }
            set
            {
                _categorizeByVideo = value;
                OnPropertyChanged("CurrentFormat");
                OnPropertyChanged("ShowNamingWarning");
                App.Current.Properties["CategorizeByVideo"] = _categorizeByVideo;
            }
        }
        public bool CategorizeByDate
        {
            get
            {
                return _categorizeByDate;
            }
            set
            {
                _categorizeByDate = value;
                OnPropertyChanged("CurrentFormat");
                OnPropertyChanged("ShowNamingWarning");
                App.Current.Properties["CategorizeByDate"] = _categorizeByDate;
            }
        }
        public string Test { get; set; }
        public string DownloadPath { get; set; }
        public string YoutubeDLPath
        {
            get { return _youtubeDLPath; }
            set { _youtubeDLPath = value; OnPropertyChanged("YoutubeDLPath"); App.Current.Properties["YoutubedlPath"] = _youtubeDLPath; }
        }
        public string CurrentFormat
        {
            get
            {
                string format = $"{Properties.Resources.SettingsDownloadBaseFormat}";
                if (CategorizeByDate)
                {
                    format += DateTime.Today.ToString("yyyy-MM-dd") + "\\";
                }
                if (CategorizeByChannel)
                {
                    format += $"{Properties.Resources.SettingsChannelName}";
                }
                if (CategorizeByVideo)
                {
                    format += $"{Properties.Resources.SettingsVideoTitle}";
                }
                return format + $"{Properties.Resources.SettingsClipName}";
            }
        }
        public string FFmpegPath
        {
            get { return _ffmpegPath; }
            set { _ffmpegPath = value; OnPropertyChanged("FFmpegPath"); App.Current.Properties["FfmpegPath"] = FFmpegPath; }
        }
        public string VersionDescription
        {
            get { return _versionDescription; }
            set { SetProperty(ref _versionDescription, value); }
        }
        public Languages Language
        {
            get { return _language; }
            set
            {
                {
                    _language = value;
                    CultureResources.ChangeCulture(_language);
                    App.Current.Properties["Language"] = _language;
                    OnPropertyChanged("CurrentFormat");
                }
            }
        }
        public AppTheme Theme
        {
            get { return _theme; }
            set { SetProperty(ref _theme, value); }
        }

        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;
        private ICommand _getYoutubeDLCommand;
        private ICommand _getFfmpegommand;
        public ICommand GetYoutubeDLCommand => _getYoutubeDLCommand ?? (_getYoutubeDLCommand = new RelayCommand(GetYoutubeDL));
        public ICommand GetFfMpegCommand => _getFfmpegommand ?? (_getFfmpegommand = new RelayCommand(GetFffmpeg));
        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));
        public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new RelayCommand(OnPrivacyStatement));

        public SettingsViewModel(IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService)
        {
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
            Language = (Languages)App.Current.Properties["Language"];
            YoutubeDLPath = (string)App.Current.Properties["YoutubedlPath"];
            FFmpegPath = (string)App.Current.Properties["FfmpegPath"];
            FontSize = (int)App.Current.Properties["FontSize"];
            CategorizeByChannel = (bool)App.Current.Properties["CategorizeByChannel"];
            CategorizeByDate = (bool)App.Current.Properties["CategorizeByDate"];
            CategorizeByVideo = (bool)App.Current.Properties["CategorizeByVideo"];
            DownloadPath = (string)App.Current.Properties["DownloadPath"];
            IsInvalidFfmpeg = !(bool)App.Current.Properties["IsValidFfmpeg"];
            IsInvalidYoutubeDL = !(bool)App.Current.Properties["IsValidYoutubeDL"];
        }

        private async void GetYoutubeDL()
        {
            Microsoft.Win32.OpenFileDialog dlg = this.OpenFileDialog("youtube-dl");
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                IsVerifyingYoutubeDL = true;
                YoutubeDLPath = dlg.FileName;
                OnPropertyChanged("IsVerifyingYoutubeDL");
                IsInvalidYoutubeDL = false;
                OnPropertyChanged("IsInvalidYoutubeDL");
                App.Current.Properties["IsValidYoutubeDL"] = !IsInvalidYoutubeDL;
                await Task.Run(() =>
                {

                    var p = Process.Start(
                        new ProcessStartInfo(YoutubeDLPath, "https://www.youtube.com/watch?v=jNQXAC9IVRw --get-duration")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        }
                    );
                    p.WaitForExit();
                    string cmdResult = p.StandardOutput.ReadToEnd().TrimEnd();
                    IsInvalidYoutubeDL = !cmdResult.StartsWith("19");
                    IsVerifyingYoutubeDL = false;
                    OnPropertyChanged("IsVerifyingYoutubeDL");
                    OnPropertyChanged("IsInvalidYoutubeDL");
                    App.Current.Properties["IsValidYoutubeDL"] = !IsInvalidYoutubeDL;
                });
            }
        }
        private async void GetFffmpeg()
        {
            Microsoft.Win32.OpenFileDialog dlg = this.OpenFileDialog("ffmpeg");
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                IsVerifyingFfmpeg = true;
                FFmpegPath = dlg.FileName;
                OnPropertyChanged("IsVerifyingFfmpeg");
                IsInvalidFfmpeg = false;
                OnPropertyChanged("IsInvalidFfmpeg");
                App.Current.Properties["IsValidFfmpeg"] = !IsInvalidFfmpeg;
                await Task.Run(() =>
                {
                    var p = Process.Start(
                        new ProcessStartInfo(FFmpegPath, "-version")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        }
                    );
                    p.WaitForExit();
                    string cmdResult = p.StandardOutput.ReadToEnd().TrimEnd();
                    IsInvalidFfmpeg = !cmdResult.StartsWith("ffmpeg version");
                    IsVerifyingFfmpeg = false;
                    OnPropertyChanged("IsVerifyingFfmpeg");
                    OnPropertyChanged("IsInvalidFfmpeg");
                    App.Current.Properties["IsValidFfmpeg"] = !IsInvalidFfmpeg;
                });
            }
        }
        private Microsoft.Win32.OpenFileDialog OpenFileDialog(string defaultFileName)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = defaultFileName; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            return dlg;
        }
        public void OnNavigatedTo(object parameter)
        {
        }
        public void OnNavigatedFrom()
        {
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
