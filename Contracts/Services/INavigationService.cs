﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Controls;

namespace YoutubeCutter.Contracts.Services
{
    public interface INavigationService
    {
        event EventHandler<string> Navigated;

        bool CanGoBack { get; }

        void Initialize(Frame shellFrame);

        bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false);

        bool NavigateTo(string pageKey, ObservableObject viewModel, object parameter = null, bool clearNavigation = false);

        void GoBack();

        void UnsubscribeNavigation();

        void CleanNavigation();
    }
}
