using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class ChatbotPage : PageBase
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
