using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;

namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class ShareAppLinkPage : PageBase
    {
        public ShareAppLinkPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<ShareAppLinkViewModel>();
        }

        public ShareAppLinkViewModel ViewModel => DataContext as ShareAppLinkViewModel;
    }
}
