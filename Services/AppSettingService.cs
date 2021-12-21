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

        void IAppSettingService.InitializeSettings()
        {
            if (!App.Current.Properties.Contains("FontSize"))
            {
                App.Current.Properties.Add("FontSize", 12);
            }
            if (!App.Current.Properties.Contains("Language"))
            {
                App.Current.Properties.Add("Language", Languages.English);
            }
            if (!App.Current.Properties.Contains("ID"))
            {
                App.Current.Properties.Add("ID", null);
            }
        }

        int IAppSettingService.GetCurrentFontSize()
        {
            throw new NotImplementedException();
        }

        Languages IAppSettingService.GetCurrentLanguage()
        {
            throw new NotImplementedException();
        }

        string IAppSettingService.GetLoggedInYoutubeID()
        {
            throw new NotImplementedException();
        }

        void IAppSettingService.SetFontSize(int size)
        {
            throw new NotImplementedException();
        }

        void IAppSettingService.SetLanguage(Languages language)
        {
            throw new NotImplementedException();
        }

        void IAppSettingService.SetLoggedInYoutubeID(string ID)
        {
            throw new NotImplementedException();
        }
    }
}
