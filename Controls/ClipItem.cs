using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using System.Windows.Input;

namespace YoutubeCutter.Controls
{
    public class ClipItem : ObservableObject
    {
        public string Identifier { get; set; }
        private string _filename;
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                OnPropertyChanged("Filename");
                _filenameHasBadCharacters = false;
                foreach (char character in IllegalCharacters)
                {
                    if (_filename.Contains(character))
                    {
                        _filenameHasBadCharacters = true;
                        break;
                    }
                }
                OnPropertyChanged("IsValidClip");
            }
        }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }
        public bool IsValidClip { get { return _isLengthPositive && _isEndWithinVideo && _isEndAfterStart && _isFilenameUnique && !_fileAlreadyExists && !_filenameHasBadCharacters; } }
        public static char[] IllegalCharacters = { '\\', '/', ':', '?', '*', '"', '>', '<', '|' };

        private bool _isLengthPositive = true;
        private bool _isEndWithinVideo = true;
        private bool _isEndAfterStart = true;
        private bool _isFilenameUnique = true;
        private bool _fileAlreadyExists = false;
        private bool _filenameHasBadCharacters = false;
        private string _informationMessage;
        public string InformationMessage
        {
            get
            {
                if (_filenameHasBadCharacters)
                {
                    return "Filename cannot contain \n" + String.Join(" ", IllegalCharacters);
                }
                else if (_fileAlreadyExists)
                {
                    return "This file already exists";
                }
                else if (!_isLengthPositive)
                {
                    return "Clip Length is 0 second";
                }
                else if (!_isEndWithinVideo)
                {
                    return "End time longer than video length";
                }
                else if (!_isEndAfterStart)
                {
                    return "End time is earlier than Start time";
                }
                else if (!_isFilenameUnique)
                {
                    return "Filename is already taken";
                }
                else
                {
                    return _informationMessage;
                }
            }
            set
            {
                _informationMessage = value;
            }
        }

        public void Validate(Time maxTime)
        {
            Time clipLength = EndTime - StartTime;
            _isEndAfterStart = !(clipLength.Hour < 0 || clipLength.Minute < 0 || clipLength.Second < 0);
            _isLengthPositive = !(clipLength.Hour == 0 && clipLength.Minute == 0 && clipLength.Second == 0);
            _isEndWithinVideo = TimeUtil.ConvertToSeconds(EndTime) <= TimeUtil.ConvertToSeconds(maxTime);
            InformationMessage = TimeUtil.TimeToString(StartTime) + " ~ " + TimeUtil.TimeToString(EndTime) + "\n" + "Duration: " + TimeUtil.TimeToString(clipLength);
            OnPropertyChanged("IsValidClip");
            OnPropertyChanged("InformationMessage");
        }

        public void IsFilenameUnique(bool isUnique)
        {
            _isFilenameUnique = isUnique;
            OnPropertyChanged("IsValidClip");
            OnPropertyChanged("InformationMessage");
        }

        public void DoesFileExist(bool doesFileExist)
        {
            _fileAlreadyExists = doesFileExist;
            OnPropertyChanged("IsValidClip");
            OnPropertyChanged("InformationMessage");
        }
    }
}