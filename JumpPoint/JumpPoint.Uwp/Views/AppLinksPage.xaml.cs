using System.ComponentModel;
using System.Numerics;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Views
{
    public sealed partial class AppLinksPage : PageBase, INotifyPropertyChanged
    {
        public AppLinksPage()
        {
            this.InitializeComponent();
        }

        public AppLinksViewModel ViewModel => DataContext as AppLinksViewModel;

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
            var parameter = TabParameter.FromJson(e.Parameter?.ToString());
            if (parameter != null)
            {
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.AppLinks, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
