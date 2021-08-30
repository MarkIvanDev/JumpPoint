﻿using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;
using NittyGritty.Uwp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Hosted
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatbotPage : NGPage
    {
        public ChatbotPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<ChatbotViewModel>();

            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(new Size(400, 400));
            appView.TryResizeView(new Size(400, 640));

            // Setup the TitleBar
            UpdateTitleBarLayout();
        }

        #region Title Bar
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreAppTitleBar = null)
        {
            var coreTitleBar = coreAppTitleBar ?? CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppDrag);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;
            Window.Current.Activated += Current_Activated;
        }

        private void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                AppTitleBar.Opacity = 1;
            }
            else
            {
                AppTitleBar.Opacity = 0.75;
            }
        }

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitleBar.Opacity = 0.75;
            }
            else
            {
                AppTitleBar.Opacity = 1;
            }
        }

        #endregion

        public ChatbotViewModel ViewModel => DataContext as ChatbotViewModel;
    }
}