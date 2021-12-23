using System;
using YoutubeCutter.Services;
using YoutubeCutter.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace YoutubeCutter.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            SettingsViewModel.InitializeSettings();
        }
    }
}
