namespace YoutubeCutter.Models
{
    public class AppConfig
    {
        public static string DEFAULT_SETTING_PATH = System.AppDomain.CurrentDomain.BaseDirectory + "settings.json";
        public string ConfigurationsFolder { get; set; }

        public string AppPropertiesFileName { get; set; }

        public string PrivacyStatement { get; set; }

        public int FontSize { get; set; }

        public string SavePath { get; set; }

        public string YoutubedlPath { get; set; }

        public string FfmpegPath { get; set; }

        public Languages Language { get; set; }
    }
}
