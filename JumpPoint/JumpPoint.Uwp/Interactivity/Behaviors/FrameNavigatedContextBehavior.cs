﻿using JumpPoint.ViewModels;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Extensions;
using NittyGritty.Uwp.Services;
using NittyGritty.Utilities;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class FrameNavigatedContextBehavior : Behavior<Frame>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerWheelChanged += AssociatedObject_PointerWheelChanged;
                AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
                AssociatedObject.Navigated += AssociatedObject_Navigated;
            }
        }

        private void AssociatedObject_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void AssociatedObject_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (AssociatedObject.DataContext is TabViewModel tabViewModel)
            {
                if (((NavigationService)tabViewModel.NavigationHelper.NavigationService).Context.Content is TabbedShell)
                {
                    // This is the first time that the frame will be used for a tab
                    ((NavigationService)tabViewModel.NavigationHelper.NavigationService).Context = AssociatedObject;
                    if (AssociatedObject.Content is Page page)
                    {
                        EventUtilities.RegisterEvent<Page, TypedEventHandler<FrameworkElement, DataContextChangedEventArgs>, DataContextChangedEventArgs>(
                            page,
                            h => page.DataContextChanged += h,
                            h => page.DataContextChanged -= h,
                            h => (o, e) => h(o, e),
                            (subscriber, s, e) => tabViewModel.Context = page.DataContext as ShellContextViewModelBase);
                    }
                    else
                    {
                        tabViewModel.NavigationHelper.ToPathType(tabViewModel.InitialParameter.AppPath, tabViewModel.InitialParameter.Parameter);
                    }
                }
                else if (((NavigationService)tabViewModel.NavigationHelper.NavigationService).Context.Content is Page page && page.DataContext is ShellContextViewModelBase)
                {
                    // A new frame is generated for an existing tab, this happens when Tabs are being rearranged by the user
                    var previousFrame = ((NavigationService)tabViewModel.NavigationHelper.NavigationService).Context;

                    // Retain navigation history and content
                    AssociatedObject.BackStack.AddRange(previousFrame.BackStack.Select(b => new PageStackEntry(b.SourcePageType, b.Parameter, b.NavigationTransitionInfo)));
                    AssociatedObject.ForwardStack.AddRange(previousFrame.ForwardStack.Select(f => new PageStackEntry(f.SourcePageType, f.Parameter, f.NavigationTransitionInfo)));
                    var content = previousFrame.Content;
                    previousFrame.Content = null;
                    AssociatedObject.Content = content;
                    ((NavigationService)tabViewModel.NavigationHelper.NavigationService).Context = AssociatedObject;
                }
                AssociatedObject.Visibility = tabViewModel.Key.Equals(SelectedTab?.Key) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void AssociatedObject_Navigated(object sender, NavigationEventArgs args)
        {
            if (args.Content is Page page && AssociatedObject.DataContext is TabViewModel tabViewModel)
            {
                EventUtilities.RegisterEvent<Page, TypedEventHandler<FrameworkElement, DataContextChangedEventArgs>, DataContextChangedEventArgs>(
                    page,
                    h => page.DataContextChanged += h,
                    h => page.DataContextChanged -= h,
                    h => (o, e) => h(o, e),
                    (subscriber, s, e) => tabViewModel.Context = page.DataContext as ShellContextViewModelBase);
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerWheelChanged -= AssociatedObject_PointerWheelChanged;
                AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
                AssociatedObject.Navigated -= AssociatedObject_Navigated;
            }
        }



        public TabViewModel SelectedTab
        {
            get { return (TabViewModel)GetValue(SelectedTabProperty); }
            set { SetValue(SelectedTabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabProperty =
            DependencyProperty.Register("SelectedTab", typeof(TabViewModel), typeof(FrameNavigatedContextBehavior), new PropertyMetadata(null, OnSelectedTabChanged));

        private static void OnSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameNavigatedContextBehavior behavior && behavior.AssociatedObject != null && behavior.AssociatedObject.DataContext is TabViewModel tab)
            {
                behavior.AssociatedObject.Visibility = tab.Key.Equals(behavior.SelectedTab?.Key) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
