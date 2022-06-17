using JumpPoint.Platform.Items;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Uwp.Helpers;
using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class RubberBandSelectionBehavior : Behavior<ListViewBase>
    {
        private ScrollViewer scrollViewer = null;
        private List<JumpPointItem> startingItems = new List<JumpPointItem>();
        private Point origin = default;
        private SelectionModifier selectionModifier = default;

        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed += AssociatedObject_PointerPressed;
                AssociatedObject.PointerReleased += AssociatedObject_PointerReleased;
                AssociatedObject.PointerCaptureLost += AssociatedObject_PointerCaptureLost;
                AssociatedObject.PointerCanceled += AssociatedObject_PointerCanceled;
                scrollViewer = XamlHelper.FindChildren<ScrollViewer>(AssociatedObject).FirstOrDefault();
                if (scrollViewer is null)
                {
                    AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
                }
            }
        }

        private void AssociatedObject_LayoutUpdated(object sender, object e)
        {
            if (scrollViewer is null)
            {
                scrollViewer = XamlHelper.FindChildren<ScrollViewer>(AssociatedObject).FirstOrDefault();
            }

            if (scrollViewer != null)
            {
                AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;
            }
        }

        private void AssociatedObject_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(AssociatedObject);
            if (!pointerPoint.Properties.IsLeftButtonPressed || e.Pointer.PointerDeviceType == PointerDeviceType.Touch) return;

            startingItems = AssociatedObject.SelectedItems.Cast<JumpPointItem>().ToList();

            origin = pointerPoint.Position;
            origin.Y += scrollViewer?.VerticalOffset ?? 0;

            if ((e.KeyModifiers & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
            {
                selectionModifier = SelectionModifier.Control;
            }
            else if ((e.KeyModifiers & VirtualKeyModifiers.Shift) == VirtualKeyModifiers.Shift)
            {
                selectionModifier = SelectionModifier.Shift;
            }
            else
            {
                selectionModifier = SelectionModifier.None;
                AssociatedObject.SelectedItems.Clear();
            }

            AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
            AssociatedObject.PointerMoved += AssociatedObject_PointerMoved;

            AssociatedObject.CapturePointer(e.Pointer);
        }

        private void AssociatedObject_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (RubberBand is null) return;
            if (scrollViewer is null) return;
            
            var currentPoint = e.GetCurrentPoint(AssociatedObject);
            if (!currentPoint.Properties.IsLeftButtonPressed) return;

            var verticalOffset = scrollViewer.VerticalOffset;
            var shiftedOrigin = new Point(origin.X, origin.Y - verticalOffset);
            DrawRubberBand(shiftedOrigin, currentPoint);

            var rect = new Drawing.Rectangle(
                x: (int)Canvas.GetLeft(RubberBand),
                y: (int)Math.Min(origin.Y, currentPoint.Position.Y + verticalOffset),
                width: (int)RubberBand.Width,
                height: (int)Math.Abs(origin.Y - (currentPoint.Position.Y + verticalOffset)));

            for (int i = 0; i < AssociatedObject.Items.Count; i++)
            {
                var container = AssociatedObject.ContainerFromIndex(i);
                if (container is SelectorItem item)
                {
                    var visual = item.TransformToVisual(AssociatedObject);
                    var point = visual.TransformPoint(new Point(0, verticalOffset));
                    var bounds = new Drawing.Rectangle((int)point.X, (int)point.Y, (int)item.ActualHeight, (int)item.ActualWidth);
                    if (rect.IntersectsWith(bounds))
                    {
                        UpdateSelection(item, true);
                    }
                    else
                    {
                        UpdateSelection(item, false);
                    }
                }
            }

            if (currentPoint.Position.Y > AssociatedObject.ActualHeight - 20)
            {
                // Scroll down the list if pointer is at the bottom
                var scrollIncrement = Math.Min(currentPoint.Position.Y - (AssociatedObject.ActualHeight - 20), 40);
                scrollViewer.ChangeView(null, verticalOffset + scrollIncrement, null, false);
            }
            else if (currentPoint.Position.Y < 20)
            {
                // Scroll up the list if pointer is at the top
                var scrollIncrement = Math.Min(20 - currentPoint.Position.Y, 40);
                scrollViewer.ChangeView(null, verticalOffset - scrollIncrement, null, false);
            }
        }

        private void UpdateSelection(SelectorItem item, bool isInside)
        {
            if (isInside)
            {
                switch (selectionModifier)
                {
                    case SelectionModifier.Control:
                        if (startingItems.Contains(item.Content))
                        {
                            item.IsSelected = false;
                        }
                        else
                        {
                            item.IsSelected = true;
                        }
                        break;

                    case SelectionModifier.Shift:
                        item.IsSelected = true;
                        break;

                    case SelectionModifier.None:
                    default:
                        item.IsSelected = true;
                        break;
                }
            }
            else
            {
                switch (selectionModifier)
                {
                    case SelectionModifier.Control:
                        if (startingItems.Contains(item.Content))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                        break;

                    case SelectionModifier.Shift:
                        if (!startingItems.Contains(item.Content))
                        {
                            item.IsSelected = false;
                        }
                        break;

                    case SelectionModifier.None:
                    default:
                        item.IsSelected = false;
                        break;
                }
            }
        }

        private void DrawRubberBand(Point origin, PointerPoint currentPoint)
        {
            if (RubberBand is null) return;

            if (currentPoint.Position.X >= origin.X)
            {
                double maxWidth = AssociatedObject.ActualSize.X - origin.X;
                if (currentPoint.Position.Y <= origin.Y)
                {
                    // Pointer was moved up and right
                    Canvas.SetLeft(RubberBand, Math.Max(0, origin.X));
                    Canvas.SetTop(RubberBand, Math.Max(0, currentPoint.Position.Y));
                    RubberBand.Width = Math.Max(0, Math.Min(currentPoint.Position.X - Math.Max(0, origin.X), maxWidth));
                    RubberBand.Height = Math.Max(0, origin.Y - Math.Max(0, currentPoint.Position.Y));
                }
                else
                {
                    // Pointer was moved down and right
                    Canvas.SetLeft(RubberBand, Math.Max(0, origin.X));
                    Canvas.SetTop(RubberBand, Math.Max(0, origin.Y));
                    RubberBand.Width = Math.Max(0, Math.Min(currentPoint.Position.X - Math.Max(0, origin.X), maxWidth));
                    RubberBand.Height = Math.Max(0, currentPoint.Position.Y - Math.Max(0, origin.Y));
                }
            }
            else
            {
                if (currentPoint.Position.Y <= origin.Y)
                {
                    // Pointer was moved up and left
                    Canvas.SetLeft(RubberBand, Math.Max(0, currentPoint.Position.X));
                    Canvas.SetTop(RubberBand, Math.Max(0, currentPoint.Position.Y));
                    RubberBand.Width = Math.Max(0, origin.X - Math.Max(0, currentPoint.Position.X));
                    RubberBand.Height = Math.Max(0, origin.Y - Math.Max(0, currentPoint.Position.Y));
                }
                else
                {
                    // Pointer was moved down and left
                    Canvas.SetLeft(RubberBand, Math.Max(0, currentPoint.Position.X));
                    Canvas.SetTop(RubberBand, Math.Max(0, origin.Y));
                    RubberBand.Width = Math.Max(0, origin.X - Math.Max(0, currentPoint.Position.X));
                    RubberBand.Height = Math.Max(0, currentPoint.Position.Y - Math.Max(0, origin.Y));
                }
            }
        }

        private void AssociatedObject_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Reset(e);
        }

        private void AssociatedObject_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Reset(e);
        }

        private void AssociatedObject_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Reset(e);
        }

        private void Reset(PointerRoutedEventArgs e)
        {
            if (RubberBand != null)
            {
                Canvas.SetTop(RubberBand, 0);
                Canvas.SetLeft(RubberBand, 0);
                RubberBand.Width = 0;
                RubberBand.Height = 0; 
            }

            origin = default;
            selectionModifier = default;

            AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
            AssociatedObject.ReleasePointerCapture(e.Pointer);
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed -= AssociatedObject_PointerPressed;
                AssociatedObject.PointerReleased -= AssociatedObject_PointerReleased;
                AssociatedObject.PointerCaptureLost -= AssociatedObject_PointerCaptureLost;
                AssociatedObject.PointerCanceled -= AssociatedObject_PointerCanceled;
                scrollViewer = null;
            }
        }


        public Rectangle RubberBand
        {
            get { return (Rectangle)GetValue(RubberBandProperty); }
            set { SetValue(RubberBandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RubberBand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RubberBandProperty =
            DependencyProperty.Register("RubberBand", typeof(Rectangle), typeof(RubberBandSelectionBehavior), new PropertyMetadata(null));


    }

    public enum SelectionModifier
    {
        None = 0,
        Control = 1,
        Shift = 2
    }
}
