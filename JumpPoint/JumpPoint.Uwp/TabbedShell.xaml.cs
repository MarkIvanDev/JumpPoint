﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using NittyGritty.Commands;
using NittyGritty.Uwp.Services;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp
{
    public sealed partial class TabbedShell : PageBase
    {
        private SystemNavigationManagerPreview systemNavPreview;

        public TabbedShell()
        {
            this.InitializeComponent();

            // Setup the TitleBar
            UpdateTitleBarLayout();
        }

        public TabbedShellViewModel ViewModel => DataContext as TabbedShellViewModel;

        #region Title Bar
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreAppTitleBar = null)
        {
            var coreTitleBar = coreAppTitleBar ?? CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            leftInset.MinWidth = coreTitleBar.SystemOverlayLeftInset;
            rightInset.MinWidth = coreTitleBar.SystemOverlayRightInset;
            leftInset.MinHeight = rightInset.MinHeight = coreTitleBar.Height;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(rightInset);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            panedTabView.UpdateTabColumnWidth();
        }

        #endregion

        #region Notifications

        public ObservableCollection<string> Problems { get; } = new ObservableCollection<string>();

        private void ExceptionManagement(NotificationMessage<Exception> message)
        {
            if (message.Content is UnauthorizedAccessException uae)
            {
                //notifyGrantAccess.IsOpen = true;
            }
            else
            {
                Problems.Add(message.Notification);
                flyoutProblems.ShowAt(btnProblems);
            }
        }

        private async void AllowAccess_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
        }

        private void ClearProblems_Click(object sender, RoutedEventArgs e)
        {
            flyoutProblems.Hide();
            Problems.Clear();
        }

        #endregion

        #region Ribbon
        DropShadow ribbonShadow = null;
        SpriteVisual ribbonShadowVisual = null;
        private void OnRibbonLoaded(object sender, RoutedEventArgs e)
        {
            var ribbon = (FrameworkElement)sender;
            var compositor = ElementCompositionPreview.GetElementVisual(ribbon).Compositor;

            ribbonShadow = compositor.CreateDropShadow();
            ribbonShadow.Color = Colors.Black;
            ribbonShadow.BlurRadius = 9;
            ribbonShadow.Opacity = 1;
            ribbonShadow.Offset = new Vector3(0, 1, 0);
            ribbonShadow.Mask = null;

            ribbonShadowVisual = compositor.CreateSpriteVisual();
            ribbonShadowVisual.Shadow = ribbonShadow;
            ribbonShadowVisual.Size = new Vector2((float)ribbon.ActualWidth, (float)ribbon.ActualHeight);
            ElementCompositionPreview.SetElementChildVisual(borderShadow, ribbonShadowVisual);
            ribbon.SizeChanged += OnRibbonSizeChanged;
        }

        private void OnRibbonSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ribbonShadowVisual != null && sender is FrameworkElement element)
            {
                ribbonShadowVisual.Size = new Vector2((float)element.ActualWidth, (float)element.ActualHeight);
            }
        }
        #endregion

        #region Navigation

        private RelayCommand<PageStackEntry> _JumpBack;
        public RelayCommand<PageStackEntry> JumpBackCommand => _JumpBack ?? (_JumpBack = new RelayCommand<PageStackEntry>(
            (entry) =>
            {
                if (entry != null)
                {
                    var frame = ((NavigationService)ViewModel.CurrentTab.NavigationHelper.NavigationService).Context;
                    var backStack = frame.BackStack.ToList();
                    var forwardStack = frame.ForwardStack.ToList();
                    var index = backStack.IndexOf(entry);
                    if (index != -1)
                    {
                        var currentItem = backStack[index];
                        var itemsToMove = Enumerable.Range(index + 1, backStack.Count - index - 1).Select(i => backStack[i]).ToList();
                        itemsToMove.Reverse();

                        // Populate forward stack
                        var currentEntry = new PageStackEntry(frame.CurrentSourcePageType, ViewModel.CurrentTab.Context.PathInfo.Parameter.ToJson(), null);
                        forwardStack.Add(currentEntry);
                        foreach (var item in itemsToMove)
                        {
                            forwardStack.Add(item);
                        }

                        // Clean back stack
                        foreach (var item in itemsToMove)
                        {
                            backStack.Remove(item);
                        }
                        backStack.Remove(currentItem);

                        frame.Navigate(currentItem.SourcePageType, currentItem.Parameter);
                        frame.BackStack.Clear();
                        foreach (var item in backStack)
                        {
                            frame.BackStack.Add(item);
                        }
                        frame.ForwardStack.Clear();
                        foreach (var item in forwardStack)
                        {
                            frame.ForwardStack.Add(item);
                        }
                    }
                }
            }));

        private RelayCommand<PageStackEntry> _JumpForward;
        public RelayCommand<PageStackEntry> JumpForwardCommand => _JumpForward ?? (_JumpForward = new RelayCommand<PageStackEntry>(
            (entry) =>
            {
                if (entry != null)
                {
                    var frame = ((NavigationService)ViewModel.CurrentTab.NavigationHelper.NavigationService).Context;
                    var backStack = frame.BackStack.ToList();
                    var forwardStack = frame.ForwardStack.ToList();
                    var index = forwardStack.IndexOf(entry);
                    if (index != -1)
                    {
                        var currentItem = forwardStack[index];
                        var itemsToMove = Enumerable.Range(index + 1, forwardStack.Count - index - 1).Select(i => forwardStack[i]).ToList();
                        itemsToMove.Reverse();

                        // Populate back stack
                        var currentEntry = new PageStackEntry(frame.CurrentSourcePageType, ViewModel.CurrentTab.Context.PathInfo.Parameter.ToJson(), null);
                        backStack.Add(currentEntry);
                        foreach (var item in itemsToMove)
                        {
                            backStack.Add(item);
                        }

                        // Clean forward stack
                        foreach (var item in itemsToMove)
                        {
                            forwardStack.Remove(item);
                        }
                        forwardStack.Remove(currentItem);

                        frame.Navigate(currentItem.SourcePageType, currentItem.Parameter);
                        frame.BackStack.Clear();
                        foreach (var item in backStack)
                        {
                            frame.BackStack.Add(item);
                        }
                        frame.ForwardStack.Clear();
                        foreach (var item in forwardStack)
                        {
                            frame.ForwardStack.Add(item);
                        }
                    }
                }
            }));

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage<Exception>>(this, MessengerTokens.ExceptionManagement, ExceptionManagement);
            systemNavPreview = SystemNavigationManagerPreview.GetForCurrentView();
            systemNavPreview.CloseRequested += OnCloseRequested;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage<Exception>>(this, MessengerTokens.ExceptionManagement, ExceptionManagement);
            if (systemNavPreview != null)
            {
                systemNavPreview.CloseRequested -= OnCloseRequested;
            }
            base.OnNavigatedFrom(e);
        }

        private async void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();
            if (ViewModel.Tabs.Count > 1)
            {
                var result = await ServiceLocator.DialogService.ShowMessage($"You are about to close {ViewModel.Tabs.Count} tabs. Are you sure you want to continue?",
                "Confirm exit", "Close", "Cancel", ServiceLocator.AppSettings.Theme);
                if (!result)
                {
                    e.Handled = true;
                }
            }
            deferral.Complete();
        }

        public void ProcessParameter(string parameter)
        {
            ViewModel.ProcessParameter(parameter);
        }

        private void toggleMenu_Click(object sender, RoutedEventArgs e)
        {
            panedTabView.IsPaneOpen = !panedTabView.IsPaneOpen;
        }

        private async void OnBreadcrumbChildrenLoaded(object sender, RoutedEventArgs e)
        {
            var crumb = ((ListView)sender).Tag as Breadcrumb;
            await ViewModel.BreadcrumbChildren.LoadCommand.TryExecute(crumb);
        }

        private void OnBreadcrumbChildrenUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.BreadcrumbChildren.CancelCommand.TryExecute();
        }

        private void OnBreadcrumbChildItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.CurrentTab.NavigationHelper.ToItem(e.ClickedItem as JumpPointItem);
        }

    }
}
