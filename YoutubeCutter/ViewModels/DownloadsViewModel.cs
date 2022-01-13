using System;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Threading.Tasks;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Core.Contracts.Services;
using YoutubeCutter.Core.Models;
using YoutubeCutter.Controls;
using System.Collections.Concurrent;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.IO;
using YoutubeCutter.Core.Helpers;

namespace YoutubeCutter.ViewModels
{
    public class DownloadsViewModel : ObservableObject, INavigationAware
    {
        private DownloadItem _selectedQueue;
        private DownloadItem _selectedDone;
        private DownloadItem _displayItem;
        private static bool _hasThreadWorking = false;
        private bool _showProgress = false;
        private int _selectedTabIndex;
        //todo make draggable, or reorder-able
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _showProgress = value == 0; _selectedTabIndex = value; DisplayItem = _selectedTabIndex == 0 ? SelectedQueue : SelectedDone; }
        }
        public ObservableCollection<DownloadItem> Queue { get; } = new ObservableCollection<DownloadItem>();
        public ObservableCollection<DownloadItem> DoneList { get; } = new ObservableCollection<DownloadItem>();
        public ConcurrentQueue<DownloadItem> DownloadQueue { get; set; } = new ConcurrentQueue<DownloadItem>();
        public DownloadItem SelectedQueue
        {
            get { return _selectedQueue; }
            set { SetProperty(ref _selectedQueue, value); SetProperty(ref _displayItem, value); OnPropertyChanged("DisplayItem"); }
        }
        public DownloadItem SelectedDone
        {
            get { return _selectedDone; }
            set { SetProperty(ref _selectedDone, value); SetProperty(ref _displayItem, value); OnPropertyChanged("DisplayItem"); }
        }
        public DownloadItem DisplayItem
        {
            get { return _displayItem; }
            set { SetProperty(ref _displayItem, value); }
        }
        public DownloadsViewModel()
        {

        }

        private ICommand _openFolderCommand;
        public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new RelayCommand(OpenFolder));
        public void OnNavigatedTo(object parameter)
        {
            _showProgress = true;
            if (parameter != null)
            {
                foreach (DownloadItem item in (DownloadItem[])parameter)
                {
                    Queue.Add(item);
                    DownloadQueue.Enqueue(item);
                }
                if (!_hasThreadWorking)
                {
                    _hasThreadWorking = true;
                    DownloadItemFromQueue();
                }
            }

        }
        private async void DownloadItemFromQueue()
        {
            await Task.Run(() =>
            {
                //todo pause download
                //todo preserve state between app session?
                DownloadItem item;
                while (!DownloadQueue.IsEmpty)
                {
                    if (DownloadQueue.TryDequeue(out item))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            item.HasStartedDownloading = true;
                        });
                        Process p;
                        if (item.DownloadProcess == null)
                        {
                            _ = Directory.CreateDirectory(item.Directory);
                            p = Process.Start(
                                new ProcessStartInfo((string)App.Current.Properties["YoutubedlPath"], item.YoutubeURL + " -g -f bestvideo")
                                {
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardError = true,
                                    RedirectStandardOutput = true,
                                }
                            );
                            p.WaitForExit();
                            string bestVideoURL = p.StandardOutput.ReadToEnd().TrimEnd();
                            p = Process.Start(
                                new ProcessStartInfo((string)App.Current.Properties["YoutubedlPath"], item.YoutubeURL + " -g -f bestaudio")
                                {
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardError = true,
                                    RedirectStandardOutput = true,
                                }
                            );
                            p.WaitForExit();
                            string bestAudioURL = p.StandardOutput.ReadToEnd().TrimEnd();
                            p = DownloadManager.DownloadVideoWithFfmpeg(TimeUtil.TimeToString(item.StartTime),
                                                                                            bestVideoURL,
                                                                                            bestAudioURL,
                                                                                            TimeUtil.TimeToString(item.Duration),
                                                                                            item.Directory.Replace("\\\\", "\\") + item.Filename + ".mp4");
                            item.DownloadProcess = p;
                        }
                        else
                        {
                            p = item.DownloadProcess;
                        }
                        StreamReader progressReader = item.DownloadProcess.StandardError;
                        //todo handle errors
                        int duration = TimeUtil.ConvertToSeconds(item.Duration);
                        string line;
                        string time;
                        string bitrate;
                        int seconds;
                        int percentage;
                        int startIndex;
                        //todo figure out why is ffmpeg slower in process than cmd
                        while ((line = progressReader.ReadLine()) != null)
                        {
                            Debug.WriteLine(line);
                            if (_showProgress)
                            {
                                startIndex = line.IndexOf("time=");
                                if (startIndex > 0)
                                {
                                    time = line.Substring(startIndex + 5, 8);
                                    seconds = 3600 * Convert.ToInt32(time.Substring(0, 2)) + 60 * Convert.ToInt32(time.Substring(3, 2)) + Convert.ToInt32(time.Substring(6, 2));
                                    percentage = seconds * 100 / duration;
                                    startIndex = line.IndexOf("bitrate=") + 8;
                                    bitrate = line.Substring(startIndex, line.IndexOf("speed=") - startIndex);
                                    //format frame= 1866 fps= 11 q=40.0 Lsize=   43776kB time=00:01:18.33 bitrate=4577.9kbits/s speed=0.45x
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        item.Progress = percentage;
                                        item.Bitrate = bitrate;
                                    });
                                    progressReader.DiscardBufferedData();
                                }
                            }
                        }
                        progressReader.Dispose();
                        DownloadManager.DownloadProcesses.Remove(p);
                        if (App.Current != null)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                item.IsDownloaded = true;
                                if (item == DisplayItem)
                                {
                                    SelectedDone = item;
                                    SelectedTabIndex = 1;
                                    OnPropertyChanged("SelectedTabIndex");
                                }
                                Queue.Remove(item);
                                DoneList.Insert(0, item);
                            });
                        }
                    }
                }
                _hasThreadWorking = false;
            });
        }
        public void OnNavigatedFrom()
        {
            _showProgress = false;
        }

        public void OpenFolder()
        {
            if (Directory.Exists(DisplayItem.Directory))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = DisplayItem.Directory.Replace("\\\\", "\\"),
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
        }
    }
}
