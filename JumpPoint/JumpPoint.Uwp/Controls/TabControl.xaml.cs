using JumpPoint.ViewModels;
using NittyGritty.Uwp.Services;
using System;
using System.Collections.Generic;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class TabControl : UserControl
    {
        public TabControl()
        {
            this.InitializeComponent();
        }


        public TabViewModel ViewModel
        {
            get { return (TabViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TabViewModel), typeof(TabControl), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tab && tab.ViewModel != null)
            {
                (tab.ViewModel.NavigationHelper.NavigationService as NavigationService).Context = tab.mainFrame;
            }
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                ViewModel.Context = page.DataContext as ShellContextViewModelBase;
            }
        }
    }
}
