using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

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

        private async void OnCopy(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext != null)
            {
                await Clipboard.SetTextAsync(button.DataContext.ToString());
            }
        }
    }
}
