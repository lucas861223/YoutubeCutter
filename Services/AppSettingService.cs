using System;

using YoutubeCutter.Contracts.Services;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Models;

namespace YoutubeCutter.Services
{
    public class AppSettingService : IAppSettingService
    {
        public AppSettingService()
        {
        }

        public void InitializeSettingsWithDefault()
        {
            if (!App.Current.Properties.Contains("FontSize"))
            {
                App.Current.Properties.Add("FontSize", 12);
            } else
            {
                App.Current.Properties["FontSize"] = Convert.ToInt32((Int64)App.Current.Properties["Language"]);
            }
            if (!App.Current.Properties.Contains("YoutubedlPath"))
            {
                App.Current.Properties.Add("YoutubedlPath", "youtube-dl.exe");
            }
            if (!App.Current.Properties.Contains("Language"))
            {
                App.Current.Properties.Add("Language", Languages.English);
            }
            else if (App.Current.Properties["Language"] is Int64)
            {
                App.Current.Properties["Language"] = (Languages)Convert.ToInt32((Int64)App.Current.Properties["Language"]);
            }
            if (!App.Current.Properties.Contains("FfmpegPath"))
            {
                App.Current.Properties.Add("FfmpegPath", "ffmpeg.exe");
            }
            if (!App.Current.Properties.Contains("CategorizeByVideo"))
            {
                App.Current.Properties.Add("CategorizeByVideo", false);
            }
            if (!App.Current.Properties.Contains("CategorizeByChannel"))
            {
                App.Current.Properties.Add("CategorizeByChannel", false);
            }
            if (!App.Current.Properties.Contains("CategorizeByDate"))
            {
                App.Current.Properties.Add("CategorizeByDate", false);
            }
            if (!App.Current.Properties.Contains("DownloadPath"))
            {
                App.Current.Properties.Add("DownloadPath", AppDomain.CurrentDomain.BaseDirectory + "Download\\");
            }
            if (!App.Current.Properties.Contains("IsValidFfmpeg"))
            {
                App.Current.Properties.Add("IsValidFfmpeg", false);
            }
            if (!App.Current.Properties.Contains("IsValidYoutubeDL"))
            {
                App.Current.Properties.Add("IsValidYoutubeDL", false);
            }
        }
    }
}
