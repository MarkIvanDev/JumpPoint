using JumpPoint.Platform.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class AppLinkDetails : UserControl
    {
        public AppLinkDetails()
        {
            this.InitializeComponent();
        }


        public AppLink AppLink
        {
            get { return (AppLink)GetValue(AppLinkProperty); }
            set { SetValue(AppLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppLinkProperty =
            DependencyProperty.Register("AppLink", typeof(AppLink), typeof(AppLinkDetails), new PropertyMetadata(null));



        public bool IsInDetailsPane
        {
            get { return (bool)GetValue(IsInDetailsPaneProperty); }
            set { SetValue(IsInDetailsPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInDetailsPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInDetailsPaneProperty =
            DependencyProperty.Register("IsInDetailsPane", typeof(bool), typeof(AppLinkDetails), new PropertyMetadata(false));


    }
}
