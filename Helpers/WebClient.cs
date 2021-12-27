﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

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

        public string[] GetVideoInfo(string videoID)
        {
            Dictionary<string, object> videoInformation = RequestAndGetJson(_noembed + videoID);
            if (videoInformation.ContainsKey("title"))
            {
                return new string[] { videoInformation["title"] as string, videoInformation["author_name"] as string, videoInformation["thumbnail_url"] as string };
            }
            //bad video
            return new string[] { "", "", "" };
        }

        private Dictionary<string, object> RequestAndGetJson(string url)
        {
            Task<string> response = _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(response.Result);
        }
    }
}