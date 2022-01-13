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
        //todo make draggable, or reorder-able
        //todo show progress bar
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
        }
        public DownloadsViewModel()
        {
            Queue.Add(new DownloadItem { HasStartedDownloading=true, VideoTitle="test", EndTime=new Time(), StartTime=new Time(), Progress=50 });
        }

        private ICommand _openFolderCommand;
        public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new RelayCommand(OpenFolder));
        public void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                _showProgress = true;
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
                                new ProcessStartInfo((string)App.Current.Properties["YoutubedlPath"], item.YoutubeURL + " -g -f best")
                                {
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardError = true,
                                    RedirectStandardOutput = true,
                                }
                            );
                            p.WaitForExit();
                            string downloadURL = p.StandardOutput.ReadToEnd().TrimEnd();
                            p = DownloadManager.DownloadVideoWithFfmpeg(TimeUtil.TimeToString(item.StartTime),
                                                                                            downloadURL,
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
                        string line;
                        while ((line = progressReader.ReadLine()) != null)
                        {
                            Debug.WriteLine(line);
                            //todo update progress bar
                            if (_showProgress)
                            {
                                //format frame= 1866 fps= 11 q=40.0 Lsize=   43776kB time=00:01:18.33 bitrate=4577.9kbits/s speed=0.45x
                            }
                        }
                        progressReader.Dispose();
                        DownloadManager.DownloadProcesses.Remove(p);
                        if (App.Current != null)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                item.IsDownloaded = true;
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
