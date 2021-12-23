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
