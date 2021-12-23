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

        public string FfmepgPath { get; set; }

        public Languages Language { get; set; }

        public static AppConfig getDefaultSetting()
        {
            AppConfig newConfig = new AppConfig();
            newConfig.FontSize = 12;
            newConfig.YoutubedlPath = "";
            newConfig.Language = Languages.English;
            newConfig.FfmepgPath = "";
            return newConfig;
        }

        public static AppConfig getAppConfigFromApp()
        {
            AppConfig appConfig = new AppConfig();
            appConfig.FontSize = (int)App.Current.Properties["FontSize"];
            appConfig.YoutubedlPath = (string)App.Current.Properties["YoutubedlPath"];
            appConfig.Language = (Languages)App.Current.Properties["Language"];
            appConfig.FfmepgPath = (string)App.Current.Properties["FfmepgPath"];
            return appConfig;
        }
        public void ApplyAppConfig()
        {
            if (!App.Current.Properties.Contains("FontSize"))
            {
                App.Current.Properties.Add("FontSize", this.FontSize);
            }
            else
            {
                App.Current.Properties["FontSize"] = this.FontSize;
            }
            if (!App.Current.Properties.Contains("YoutubedlPath"))
            {
                App.Current.Properties.Add("YoutubedlPath", this.YoutubedlPath);
            }
            else
            {
                App.Current.Properties["YoutubedlPath"] = this.YoutubedlPath;
            }
            if (!App.Current.Properties.Contains("Language"))
            {
                App.Current.Properties.Add("Language", this.Language);
            }
            else
            {
                App.Current.Properties["Language"] = this.Language;
            }
            if (!App.Current.Properties.Contains("FfmepgPath"))
            {
                App.Current.Properties.Add("FfmepgPath", this.FfmepgPath);
            }
            else
            {
                App.Current.Properties["FfmepgPath"] = this.FfmepgPath;
            }

        }

    }
}
