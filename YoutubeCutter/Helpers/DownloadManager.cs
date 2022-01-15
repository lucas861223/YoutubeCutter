using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace YoutubeCutter.Helpers
{
    class DownloadManager
    {
        public static List<Process> DownloadProcesses = new List<Process>();
        private static WebClient _webClient = WebClient.Instance;
        private static string _cacheLocation = AppDomain.CurrentDomain.BaseDirectory + "Cache";
        public static string DownloadThumbnail(string url, string youtubeID, string filename)
        {
            if (!File.Exists(_cacheLocation + "\\" + youtubeID + "\\" + filename))
            {
                Directory.CreateDirectory(_cacheLocation + "\\" + youtubeID);
                _webClient.DownloadImage(url, _cacheLocation + "\\" + youtubeID + "\\" + filename);
            }
            return _cacheLocation + "\\" + youtubeID + "\\" + filename;
        }
        public static void StopAllDownloads()
        {
            foreach (Process process in DownloadProcesses)
            {
                process.Kill();
            }
        }
        public static void MakeChacheFolder()
        {

            Directory.CreateDirectory(_cacheLocation);

        }
        public static string GetDownloadPath(string videoTitle, string channelName)
        {
            string downloadPath = (string)App.Current.Properties["DownloadPath"];
            if ((bool)App.Current.Properties["CategorizeByDate"])
            {
                downloadPath += DateTime.Today.ToString("yyyy-MM-dd") + "\\";
            }
            if ((bool)App.Current.Properties["CategorizeByChannel"])
            {
                string fixedChannelName = channelName;
                foreach (char character in Controls.ClipItem.IllegalCharacters)
                {
                    fixedChannelName = fixedChannelName.Replace(character + "", "");
                }
                if (String.IsNullOrEmpty(fixedChannelName.Trim()))
                {
                    fixedChannelName = "UntitledChannel";
                }
                downloadPath += fixedChannelName + "\\";
            }
            if ((bool)App.Current.Properties["CategorizeByVideo"])
            {
                string fixedVideoName = videoTitle;
                foreach (char character in Controls.ClipItem.IllegalCharacters)
                {
                    fixedVideoName = fixedVideoName.Replace(character + "", "");
                }
                if (String.IsNullOrEmpty(fixedVideoName.Trim()))
                {
                    fixedVideoName = "UntitledVideo";
                }
                downloadPath += fixedVideoName + "\\";
            }
            return downloadPath;
        }
        public static void ClearCacheFolder()
        {
            try
            {
                Directory.Delete(_cacheLocation, true);
            }
            catch (DirectoryNotFoundException)
            {

            }
        }
        public static Process DownloadVideoWithFfmpeg(string startTime, string videoURL, string audioURL, string duration, string outputFile)
        {
            Debug.WriteLine((string)App.Current.Properties["FfmpegPath"] + " " + "-ss " + startTime + " -i \"" + videoURL + "\" -i \"" + audioURL + "\" -t "
                                                                                   + duration + " -c copy \"" + outputFile + "\" -y");
            Process p = Process.Start(
                new ProcessStartInfo((string)App.Current.Properties["FfmpegPath"], "-ss " + startTime + " -i \"" + videoURL + "\" -i \"" + audioURL + "\" -t "
                                                                                   + duration + " -c copy \"" + outputFile + "\" -y")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            );
            DownloadProcesses.Add(p);
            return p;
        }
    }
}
