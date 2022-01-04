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
    }
}
