using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;

namespace JumpPoint.Uwp.Hosted
{
    public sealed partial class NewTextDocumentPage : PageBase
    {
        public NewTextDocumentPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<NewTextDocumentViewModel>();
        }

        public NewTextDocumentViewModel ViewModel => DataContext as NewTextDocumentViewModel;
    }
}
