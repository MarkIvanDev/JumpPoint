using Microsoft.Xaml.Interactivity;
using NittyGritty.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class ScrollDecreaseButtonBehavior : Behavior<RepeatButton>
    {
        private ScrollViewer scrollViewer = null;
        private FrameworkElement scrollContent = null;

        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
                AssociatedObject.Click += AssociatedObject_Click;
                UpdateButton();
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButton();
        }

        private void Initialize()
        {
            if (scrollViewer is null)
            {
                scrollViewer = XamlHelper.FindParent<ScrollViewer>(AssociatedObject);
                if (scrollViewer != null)
                {
                    scrollViewer.Loaded += ScrollViewer_Loaded;
                    scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
                    var scrollContentPresenter = XamlHelper.FindChildren<ScrollContentPresenter>(scrollViewer).FirstOrDefault();
                    if (scrollContentPresenter.Content is FrameworkElement content)
                    {
                        scrollContent = content;
                        scrollContent.SizeChanged += ScrollContent_SizeChanged;
                    }
                }
            }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButton();
        }

        private void ScrollContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateButton();
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            Initialize();
            if (scrollViewer != null)
            {
                scrollViewer.ChangeView(Math.Max(0.0, scrollViewer.HorizontalOffset - 50), null, null);
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateButton();
        }

        private void UpdateButton()
        {
            Initialize();
            if (scrollViewer != null)
            {
                var horizontalOffset = scrollViewer.HorizontalOffset;
                var scrollableWidth = scrollViewer.ScrollableWidth;

                if (Math.Abs(horizontalOffset - scrollableWidth) < 0.1)
                {
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.IsEnabled = true;
                    }
                }
                else if (Math.Abs(horizontalOffset) < 0.1)
                {
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.IsEnabled = false;
                    }
                }
                else
                {
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.IsEnabled = true;
                    }
                }
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= AssociatedObject_Loaded;
                AssociatedObject.Click -= AssociatedObject_Click;
                if (scrollViewer != null)
                {
                    scrollViewer.Loaded -= ScrollViewer_Loaded;
                    scrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
                    scrollViewer = null;
                    if (scrollContent != null)
                    {
                        scrollContent.SizeChanged -= ScrollContent_SizeChanged;
                        scrollContent = null;
                    }
                }
            }
        }
    }
}
