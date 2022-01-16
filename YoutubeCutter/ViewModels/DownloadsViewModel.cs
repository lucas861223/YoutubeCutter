using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using YoutubeCutter.Core.Helpers;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Controls;
using YoutubeCutter.Helpers;

namespace YoutubeCutter.ViewModels
{
    public class DownloadsViewModel : ObservableObject, INavigationAware
    {
        private static bool _hasThreadWorking = false;

        private int _selectedTabIndex;
        private int _movingIndex;
        private bool _isMoving = false;
        private bool _showProgress = false;

        private DownloadItem _selectedQueue;
        private DownloadItem _selectedDone;
        private DownloadItem _displayItem;
        private ListViewItem _movingItem;
        private ListViewItem _currentItem;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _showProgress = value == 0; _selectedTabIndex = value; DisplayItem = _selectedTabIndex == 0 ? SelectedQueue : SelectedDone; }
        }
        public ObservableCollection<DownloadItem> Queue { get; } = new ObservableCollection<DownloadItem>();
        public ObservableCollection<DownloadItem> DoneList { get; } = new ObservableCollection<DownloadItem>();
        public DownloadItem SelectedQueue
        {
            get { return _selectedQueue; }
            set { if (!_isMoving) { SetProperty(ref _selectedQueue, value); SetProperty(ref _displayItem, value); OnPropertyChanged("DisplayItem"); } }
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

        private ICommand _itemMouseDownCommand;
        private ICommand _dropCommand;
        private ICommand _openFolderCommand;
        private ICommand _dragEnterCommand;
        public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new RelayCommand(OpenFolder));
        public ICommand ItemMouseDownCommand => _itemMouseDownCommand ?? (_itemMouseDownCommand = new RelayCommand<ListViewItem>(ItemMouseDown));
        public ICommand DropCommand => _dropCommand ?? (_dropCommand = new RelayCommand<DragEventArgs>(Drop));
        public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand<ListViewItem>(DragEnter));

        public DownloadsViewModel()
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            _showProgress = true;
            if (parameter != null)
            {
                foreach (DownloadItem item in (DownloadItem[])parameter)
                {
                    Queue.Add(item);
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
                while (Queue.Count > 0)
                {
                    item = Queue[0];
                    if (App.Current != null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            item.HasStartedDownloading = true;
                        });
                    }
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
                    //Maybe the frame data is usable for pausing
                    //but that means the program must process each line
                    //so it knows the latest data
                    while ((line = progressReader.ReadLine()) != null)
                    {
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
                            DoneList.Insert(0, item);
                            if (item == DisplayItem)
                            {
                                SelectedDone = item;
                                SelectedTabIndex = 1;
                                OnPropertyChanged("SelectedTabIndex");
                            }
                            Queue.Remove(item);
                        });
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
        private void ItemMouseDown(ListViewItem e)
        {
            SelectedQueue = e.DataContext as DownloadItem;
            OnPropertyChanged("SelectedQueue");
            DownloadItem dc = e.DataContext as DownloadItem;
            if (!dc.HasStartedDownloading)
            {
                _movingIndex = Queue.IndexOf(e.DataContext as DownloadItem);
                _movingItem = e;
                DragDrop.DoDragDrop(_movingItem, _movingItem.DataContext, DragDropEffects.Move);
            }
        }
        private void Drop(DragEventArgs e)
        {
            if (_currentItem != _movingItem && _currentItem != null)
            {
                int index = Queue.IndexOf(_currentItem.DataContext as DownloadItem);
                (_currentItem.DataContext as DownloadItem).ShowBottom = false;
                (_currentItem.DataContext as DownloadItem).ShowTop = false;
                Queue.Move(_movingIndex, index);
                _currentItem = null;
                SelectedQueue = Queue[index];
            }
        }
        private void DragEnter(ListViewItem e)
        {
            if (_currentItem != null)
            {
                (_currentItem.DataContext as DownloadItem).ShowBottom = false;
                (_currentItem.DataContext as DownloadItem).ShowTop = false;
            }
            DownloadItem dc = e.DataContext as DownloadItem;
            if (!dc.HasStartedDownloading)
            {
                _currentItem = e;
                if (e != _movingItem)
                {
                    if (Mouse.GetPosition(_movingItem).Y > Mouse.GetPosition(e).Y)
                    {
                        if (!dc.ShowBottom)
                        {
                            dc.ShowBottom = true;
                        }
                    }
                    else
                    {
                        if (!dc.ShowTop && !dc.HasStartedDownloading)
                        {
                            dc.ShowTop = true;
                        }
                    }
                }
            }
            else
            {
                _currentItem = null;
            }
        }
    }
}
