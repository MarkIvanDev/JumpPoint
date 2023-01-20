using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;

namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class AppLinkProviderPage : PageBase
    {
        public AppLinkProviderPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<AppLinkProviderViewModel>();
        }

        public AppLinkProviderViewModel ViewModel => DataContext as AppLinkProviderViewModel;
    }
}
