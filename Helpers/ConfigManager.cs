using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;
using System.IO;
using System.Text.Json;

namespace YoutubeCutter.Helpers
{
    class ConfigManager
    {
        public static AppConfig getDefaultSetting()
        {
            AppConfig newConfig = new AppConfig();
            newConfig.FontSize = 12;
            newConfig.YoutubedlPath = "Location of youtube-dl.exe";
            newConfig.Language = Languages.English;
            newConfig.FfmpegPath = "Location of ffmpeg.exe";
            return newConfig;
        }

        public static AppConfig getAppConfigFromApp()
        {
            AppConfig appConfig = new AppConfig();
            appConfig.FontSize = (int)App.Current.Properties["FontSize"];
            appConfig.YoutubedlPath = (string)App.Current.Properties["YoutubedlPath"];
            appConfig.Language = (Languages)App.Current.Properties["Language"];
            appConfig.FfmpegPath = (string)App.Current.Properties["FfmpegPath"];
            return appConfig;
        }
        public static void ApplyAppConfig(AppConfig appConfig)
        {
            if (!App.Current.Properties.Contains("FontSize"))
            {
                App.Current.Properties.Add("FontSize", appConfig.FontSize);
            }
            else
            {
                App.Current.Properties["FontSize"] = appConfig.FontSize;
            }
            if (!App.Current.Properties.Contains("YoutubedlPath"))
            {
                App.Current.Properties.Add("YoutubedlPath", appConfig.YoutubedlPath);
            }
            else
            {
                App.Current.Properties["YoutubedlPath"] = appConfig.YoutubedlPath;
            }
            if (!App.Current.Properties.Contains("Language"))
            {
                App.Current.Properties.Add("Language", appConfig.Language);
            }
            else
            {
                App.Current.Properties["Language"] = appConfig.Language;
            }
            if (!App.Current.Properties.Contains("FfmpegPath"))
            {
                App.Current.Properties.Add("FfmpegPath", appConfig.FfmpegPath);
            }
            else
            {
                App.Current.Properties["FfmpegPath"] = appConfig.FfmpegPath;
            }

        }

        public static void InitializeConfig()
        {
            string settingFilePath = AppConfig.DEFAULT_SETTING_PATH;
            AppConfig appConfig;
            if (!File.Exists(settingFilePath))
            {
                appConfig = ConfigManager.getDefaultSetting();
            }
            else
            {
                appConfig = LoadSettingFromFile();
            }
            ConfigManager.ApplyAppConfig(appConfig);
        }

        public static void SaveSettingsWithAppConfig(AppConfig appConfig)
        {
            File.WriteAllText(AppConfig.DEFAULT_SETTING_PATH, JsonSerializer.Serialize(appConfig));
        }

        private static AppConfig LoadSettingFromFile()
        {
            return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(AppConfig.DEFAULT_SETTING_PATH));
        }

        public static void SaveSettings()
        {
            SaveSettingsWithAppConfig(getAppConfigFromApp());
        }
    }
}
