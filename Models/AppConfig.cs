﻿namespace YoutubeCutter.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; }

        public string AppPropertiesFileName { get; set; }

        public string PrivacyStatement { get; set; }

        public int FontSize { get; set; }

        public string SavePath { get; set; }

        public string YoutubedlPath { get; set; }

        public string FfmpegPath { get; set; }

        public Languages Language { get; set; }
        public bool CategorizeByVideo { get; set; }
        public bool CategorizeByChannel { get; set; }
        public bool CategorizeByDate { get; set; }
        public string DownloadPath { get; set; }
    }
}
