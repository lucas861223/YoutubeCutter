using System;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Threading.Tasks;
using YoutubeCutter.Models;
using YoutubeCutter.Contracts.ViewModels;
using YoutubeCutter.Core.Contracts.Services;
using YoutubeCutter.Core.Models;
using YoutubeCutter.Controls;
using System.Collections.Concurrent;

namespace YoutubeCutter.ViewModels
{
    public class DownloadsViewModel : ObservableObject, INavigationAware
    {
        private DownloadItem _selectedQueue;
        private DownloadItem _selectedDone;
        private DownloadItem _displayItem;
        public ObservableCollection<DownloadItem> Queue { get; } = new ObservableCollection<DownloadItem>();
        public ObservableCollection<DownloadItem> DoneList { get; } = new ObservableCollection<DownloadItem>();
        private static bool _hasThreadWorking = false;
        public ConcurrentQueue<DownloadItem> DownloadQueue { get; set; }

        public DownloadItem SelectedQueue
        {
            get { return _selectedQueue; }
            set { SetProperty(ref _selectedQueue, value); SetProperty(ref _displayItem, value); }
        }

        public DownloadItem SelectedDone
        {
            get { return _selectedDone; }
            set { SetProperty(ref _selectedDone, value); SetProperty(ref _displayItem, value); }
        }
        public DownloadItem DisplayItem
        {
            get { return _displayItem; }
        }
        public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();

        public DownloadsViewModel()
        {

        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                foreach (DownloadItem item in (DownloadItem[]) parameter) 
                {
                    Queue.Add(item);
                }
            }
            if (!_hasThreadWorking)
            {
                DownloadItemFromQueue();
            }
        }
        private async void DownloadItemFromQueue()
        {
            await Task.Run(() =>
            {
                
            });
        }
        public void OnNavigatedFrom()
        {
        }
    }
}
