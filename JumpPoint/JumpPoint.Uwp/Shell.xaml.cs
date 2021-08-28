using System;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using NittyGritty.Uwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : NGPage
    {
        public Shell()
        {
            this.InitializeComponent();
            //ServiceLocator.NavigationService.Context = mainFrame;

            // Setup the TitleBar
            UpdateTitleBarLayout();
        }

        public ShellViewModel ViewModel => DataContext as ShellViewModel;

        public Uri SupportIconUri { get; } = new Uri($@"ms-appx://JumpPoint.Uwp/Assets/Icons/Support.png");

        public Uri SettingsIconUri { get; } = new Uri($@"ms-appx://JumpPoint.Uwp/Assets/Icons/Path/Settings.png");


        #region Title Bar
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreAppTitleBar = null)
        {
            //var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.InactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = coreAppTitleBar ?? CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);

            // Update title bar control size as needed to account for system size changes.
            rowTitleBar.Height = new GridLength(coreTitleBar.Height);
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

        #region Notifications

        public ObservableCollection<string> Problems { get; } = new ObservableCollection<string>();

        private void ExceptionManagement(NotificationMessage<Exception> message)
        {
            if(message.Content is UnauthorizedAccessException)
            {
                notifyGrantAccess.IsOpen = true;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage<Exception>>(this, MessengerTokens.ExceptionManagement, ExceptionManagement);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage<Exception>>(this, MessengerTokens.ExceptionManagement, ExceptionManagement);
            base.OnNavigatedFrom(e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.Content is Page page)
            {
                ViewModel.Context = page.DataContext as ShellContextViewModelBase;
            }
        }

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
            if(ribbonShadowVisual != null && sender is FrameworkElement element)
            {
                ribbonShadowVisual.Size = new Vector2((float)element.ActualWidth, (float)element.ActualHeight);
            }
        }

    }
}
