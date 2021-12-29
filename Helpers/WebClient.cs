using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.IO;
using System.Collections.Generic;

using YoutubeCutter.Models;

namespace YoutubeCutter.Helpers
{
    class WebClient
    {
        private string _noembed = "https://noembed.com/embed?url=https://www.youtube.com/watch?v=";
        private HttpClient _httpClient;
        private WebClient()
        {
            _httpClient = new HttpClient();
        }
        private static WebClient _instance = null;
        public static WebClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WebClient();
                }
                return _instance;
            }
        }

        public void GetVideoInfo(VideoInformation information, string videoID)
        {
            Dictionary<string, object> videoInformation = RequestAndGetJson(_noembed + videoID);
            if (videoInformation.ContainsKey("title"))
            {
                information.VideoTitle = videoInformation["title"].ToString();
                information.ChannelName = videoInformation["author_name"].ToString();
                information.VideoThumbnailURL = videoInformation["thumbnail_url"].ToString();
                information.AuthorURL = videoInformation["author_url"].ToString();
            }
            //bad video
        }

        private Dictionary<string, object> RequestAndGetJson(string url)
        {
            Task<string> response = _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(response.Result);
        }
        
        public string GetHTML(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return data;
            }
            return "404";
        }

        public void DownloadImage(string url, string filename)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                using (var fileStream = File.OpenWrite(filename))
                {
                    var bytes = new byte[4096];
                    var read = 0;
                    do
                    {
                        if (stream == null)
                        {
                            continue;
                        }
                        read = stream.Read(bytes, 0, bytes.Length);
                        fileStream.Write(bytes, 0, read);
                    } while (read != 0);
                }
            }
        }
    }
}
