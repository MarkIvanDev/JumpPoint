using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;

namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class ManualAppLinkPickerPage : PageBase
    {
        public ManualAppLinkPickerPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<ManualAppLinkPickerViewModel>();
        }

        public ManualAppLinkPickerViewModel ViewModel => DataContext as ManualAppLinkPickerViewModel;
    }
}
