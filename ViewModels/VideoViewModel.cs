using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeCutter.Contracts.ViewModels;

namespace YoutubeCutter.ViewModels
{
    public class VideoViewModel : ObservableObject, INavigationAware
    {
        private Regex _youtubeRegex = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([^\?\&\#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string _youtubeID;
        private string _youtubeURL;
        //thumbnail https://img.youtube.com/vi/<ID>/0.jpg
        public string YoutubeEmbedVideoURL { get; set; }
        public string YoutubeVideoURL
        {
            get
            {
                return _youtubeURL;
            }
            set
            {
                _youtubeURL = value.Trim();
                Match match = _youtubeRegex.Match(value);
                if (match.Success)
                {
                    //todo add get videoname via youtube api
                    _youtubeURL = "https://www.youtube.com/watch?v=" + match.Groups[1];
                    GetYoutubeVideoInformation(_youtubeURL);
                    _youtubeID = match.Groups[1].ToString();
                    YoutubeEmbedVideoURL = "https://www.youtube.com/embed/" + match.Groups[1];
                }
                OnPropertyChanged("YoutubeEmbedVideoURL");
            }
        }

        public string GetYoutubeVideoInformation(string ID)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = App.Current.Properties["YoutubedlPath"] as string;
            cmd.StartInfo.Arguments = _youtubeURL + " -e";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            _youtubeURL = cmd.StandardOutput.ReadToEnd();
            return "";
        }
        public void OnNavigatedFrom()
        {
            //add download to manager
        }

        public void OnNavigatedTo(object parameter)
        {
            //tood to think
        }
    }
}
