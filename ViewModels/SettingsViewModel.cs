﻿using System;
using System.Windows.Input;

using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using YoutubeCutter.Contracts.Services;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Models;

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
        public Languages Language { get; set; }
        public string Test { get; set; }

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
            Test = AppConfig.DEFAULT_SETTING_PATH;
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser((string)App.Current.Properties["PrivacyStatement"]);

        public static void InitializeSettings()
        {
            string settingFilePath = AppConfig.DEFAULT_SETTING_PATH;
            AppConfig appConfig;
            if (!File.Exists(settingFilePath))
            {
                appConfig = AppConfig.getDefaultSetting();
            }
            else
            {
                appConfig = LoadSettingFromFile();
            }
            appConfig.ApplyAppConfig();
        }
        public static void SaveSettings()
        {
            File.WriteAllText(AppConfig.DEFAULT_SETTING_PATH, JsonSerializer.Serialize(AppConfig.getAppConfigFromApp()));
        }

        private static AppConfig LoadSettingFromFile()
        {
            return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(AppConfig.DEFAULT_SETTING_PATH));
        }
    }
}
