using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;

namespace YoutubeCutter.Controls
{
    public class ClipItem : ObservableObject
    {
        public string Filename { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

        private bool _isValidClip = true;
        public bool IsValidClip { get { return _isValidClip; } set { _isValidClip = value; OnPropertyChanged("IsValidClip"); } }

        private string _informationMessage;
        public string InformationMessage { get { return _informationMessage; } set { _informationMessage = value; OnPropertyChanged("InformationMessage"); } }

        public void Validate(Time maxTime)
        {
            Time clipLength = EndTime - StartTime;
            if (clipLength.Hour < 0 || clipLength.Minute < 0 || clipLength.Second < 0)
            {
                IsValidClip = false;
                InformationMessage = "End time is earlier than Start time";
            }
            else if (clipLength.Hour == 0 && clipLength.Minute == 0 && clipLength.Second == 0)
            {
                IsValidClip = false;
                InformationMessage = "Clip Length is 0 second";
            }
            else if (TimeUtil.ConvertToSeconds(EndTime) > TimeUtil.ConvertToSeconds(maxTime))
            {
                IsValidClip = false;
                InformationMessage = "End time longer than video length";
            }
            else
            {
                IsValidClip = true;
                InformationMessage = TimeUtil.TimeToString(StartTime) + " ~ " + TimeUtil.TimeToString(EndTime) + "\n" + "Duration: " + TimeUtil.TimeToString(clipLength);
            }
        }
    }
}
