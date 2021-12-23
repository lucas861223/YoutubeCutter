using YoutubeCutter.Models;

namespace YoutubeCutter.Contracts.Services
{
    public interface IAppSettingService
    {
        Languages GetCurrentLanguage();

        void SetLanguage(Languages language);

        int GetCurrentFontSize();

        void SetFontSize(int size);

        string GetLoggedInYoutubeID();

        void SetLoggedInYoutubeID(string ID);
    }
}
