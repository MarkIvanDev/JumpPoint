using JumpPoint.Platform;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : NGPage, INotifyPropertyChanged
    {
        public DashboardPage()
        {
            this.InitializeComponent();
        }

        public DashboardViewModel ViewModel => DataContext as DashboardViewModel;

        #region Animation
        private void OnItemLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
            {
                InitializeAnimation(element);
            }
        }

        private void InitializeAnimation(UIElement element)
        {
            var elementVisual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = elementVisual.Compositor;

            // Create animation to scale up the rectangle
            var pointerEnteredAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerEnteredAnimation.InsertKeyFrame(1.0f, new Vector3(1.2f));

            // Create animation to scale the rectangle back down
            var pointerExitedAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerExitedAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f));

            var root = VisualTreeHelper.GetParent(element) as UIElement;
            root.PointerEntered += (sender, args) =>
            {
                elementVisual.CenterPoint = new Vector3(elementVisual.Size / 2, 0);
                elementVisual.StartAnimation("Scale", pointerEnteredAnimation);
            };

            root.PointerExited += (sender, args) =>
            {
                elementVisual.StartAnimation("Scale", pointerExitedAnimation);
            };
        }
        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TabParameter parameter)
            {
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Dashboard, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }


    }
}
