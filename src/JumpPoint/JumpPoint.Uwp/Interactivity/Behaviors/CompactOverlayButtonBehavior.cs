using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class CompactOverlayButtonBehavior : Behavior<Button>
    {
        public ApplicationView ApplicationView => ApplicationView.GetForCurrentView();

        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                Window.Current.SizeChanged += Window_SizeChanged;
                AssociatedObject.Click += AssociatedObject_Click;
                UpdateVisibility();
            }
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            AssociatedObject.Visibility = ApplicationView.ViewMode == ApplicationViewMode.Default ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private async void AssociatedObject_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApplicationView.IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                await ApplicationView.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                Window.Current.SizeChanged -= Window_SizeChanged;
                AssociatedObject.Click -= AssociatedObject_Click;
            }
        }
    }
}
