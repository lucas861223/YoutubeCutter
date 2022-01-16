using Microsoft.Toolkit.Mvvm.ComponentModel;

using System;

using YoutubeCutter.Core.Models;
using YoutubeCutter.Core.Helpers;
using YoutubeCutter.Properties;

namespace YoutubeCutter.Controls
{
    public class ClipItem : ObservableObject
    {
        private bool _isLengthPositive = true;
        private bool _isEndWithinVideo = true;
        private bool _isEndAfterStart = true;
        private bool _isFilenameUnique = true;
        private bool _fileAlreadyExists = false;
        private bool _filenameHasBadCharacters = false;
        private string _informationMessage;
        private string _filename;

        public static char[] IllegalCharacters = { '\\', '/', ':', '?', '*', '"', '>', '<', '|' };
        public string Identifier { get; set; }
        public bool IsValidClip { get { return _isLengthPositive && _isEndWithinVideo && _isEndAfterStart && _isFilenameUnique && !_fileAlreadyExists && !_filenameHasBadCharacters; } }
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
        public string InformationMessage
        {
            get
            {
                if (_filenameHasBadCharacters)
                {
                    return $"{Resources.ClipItemIllegalCharacter}" + "\n" + String.Join(" ", IllegalCharacters);
                }
                else if (_fileAlreadyExists)
                {
                    return $"{Resources.ClipItemFileAlreadyExists}";
                }
                else if (!_isLengthPositive)
                {
                    return $"{Resources.ClipItemZeroLength}";
                }
                else if (!_isEndWithinVideo)
                {
                    return $"{Resources.ClipItemEndLongerThanVideo}";
                }
                else if (!_isEndAfterStart)
                {
                    return $"{Resources.ClipItemEndTimeEarlier}";
                }
                else if (!_isFilenameUnique)
                {
                    return $"{Resources.ClipItemFileNameTaken}";
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
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

        public void Validate(Time maxTime)
        {
            Time clipLength = EndTime - StartTime;
            _isEndAfterStart = !(clipLength.Hour < 0 || clipLength.Minute < 0 || clipLength.Second < 0);
            _isLengthPositive = !(clipLength.Hour == 0 && clipLength.Minute == 0 && clipLength.Second == 0);
            _isEndWithinVideo = TimeUtil.ConvertToSeconds(EndTime) <= TimeUtil.ConvertToSeconds(maxTime);
            InformationMessage = TimeUtil.TimeToString(StartTime) + " ~ " + TimeUtil.TimeToString(EndTime) + "\n" + $"{Resources.ClipItemDuration} " + TimeUtil.TimeToString(clipLength);
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