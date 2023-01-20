using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;
namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class HashToolPage : PageBase
    {
        public HashToolPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<HashToolViewModel>();
        }

        public HashToolViewModel ViewModel => DataContext as HashToolViewModel;
    }
}
