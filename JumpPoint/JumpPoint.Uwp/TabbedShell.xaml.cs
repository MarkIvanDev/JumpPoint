using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using Microsoft.UI.Xaml.Controls;
using NittyGritty.Services;
using NittyGritty.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabbedShell : NGPage
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
                "Confirm exit", "Close", "Cancel");
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

        //private void ManageTabs(GenericMessage<NewTabParameter> message)
        //{
        //    var tabData = ViewModelLocator.GetNewTab();

        //    var tabItemTemplate = this.Resources["TabItemTemplate"] as DataTemplate;
        //    var tabItem = tabItemTemplate.GetElement(new Windows.UI.Xaml.ElementFactoryGetArgs() { Data = tabData }) as TabViewItem;
        //    tabItem.DataContext = tabData;
        //    ((NavigationService)tabData.NavigationHelper.NavigationService).Context = tabItem.Content as Frame;

        //    ViewModel.Tabs.Add(tabData);
        //    tabView.TabItems.Add(tabItem);

        //    tabData.NavigationHelper.ToPathType(message.Content.AppPath, message.Content.Parameter);
        //}

        //private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        //{
        //    if (args.Tab.DataContext is TabViewModel tabData)
        //    {
        //        ViewModel.Tabs.Remove(tabData);
        //        tabView.TabItems.Remove(args.Tab);
        //    }
        //}
    }
}
