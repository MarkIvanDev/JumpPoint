﻿using JumpPoint.Platform;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : NGPage, INotifyPropertyChanged
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TabParameter parameter)
            {
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Settings, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

        private async void OnRunAtStartupToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            if (toggle.IsOn)
            {
                await ServiceLocator.AppSettings.EnableRunAtStartup();
            }
            else
            {
                await ServiceLocator.AppSettings.DisableRunAtStartup();
            }
        }

        public Visibility GetStatusDescriptionVisibility(StartupStatus status)
        {
            switch (status)
            {
                case StartupStatus.Disabled:
                case StartupStatus.Enabled:
                    return Visibility.Collapsed;

                case StartupStatus.DisabledByUser:
                case StartupStatus.DisabledByPolicy:
                case StartupStatus.EnabledByPolicy:
                default:
                    return Visibility.Visible;
            }
        }
    }
}
