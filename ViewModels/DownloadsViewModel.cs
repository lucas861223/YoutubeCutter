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

namespace YoutubeCutter.ViewModels
{
    public class DownloadsViewModel : ObservableObject, INavigationAware
    {
        private DownloadItem _selectedQueue;
        private DownloadItem _selectedDone;
        private DownloadItem _displayItem;
        private static bool _hasThreadWorking = false;
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

        }
        public void OnNavigatedTo(object parameter)
        {
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
                //todo pause/cancel download
                //todo preserve state between app session?
                DownloadItem item;
                while (!DownloadQueue.IsEmpty)
                {
                    if (DownloadQueue.TryDequeue(out item))
                    {
                        _ = Directory.CreateDirectory(item.Directory);
                        var p = Process.Start(
                            new ProcessStartInfo((string)App.Current.Properties["YoutubedlPath"], item.YoutubeURL  + " -g -f best")
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardError = true,
                                RedirectStandardOutput = true,
                            }
                        );
                        p.WaitForExit();
                        string downloadURL = p.StandardOutput.ReadToEnd().TrimEnd();
                        p = Process.Start(
                            new ProcessStartInfo((string)App.Current.Properties["FfmpegPath"],
                                                   "-ss " + TimeUtil.TimeToString(item.StartTime) + " -i \"" + downloadURL + "\" -t " + TimeUtil.TimeToString(item.Duration) + " \"" +
                                                   item.Directory.Replace("\\\\", "\\") + item.Filename + ".mp4" + "\" -y")
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardError = true,
                                RedirectStandardOutput = true,
                            }
                        );
                        StreamReader progress = p.StandardError;
                        //todo handle errors
                        string line;
                        while ((line = progress.ReadLine()) != null)
                        {
                            //todo update progress bar
                            Debug.WriteLine(line);
                        }
                        item.IsDownloaded = true;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Queue.Remove(item);
                            DoneList.Add(item);
                        });
                    }
                }
                _hasThreadWorking = false;
            });
        }
        public void OnNavigatedFrom()
        {
        }

    }
}
