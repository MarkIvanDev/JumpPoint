using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Controls
{
    public class PageBase : Page
    {
        private SystemNavigationManager currentView;
        private IStateManager PageViewModel => base.DataContext as IStateManager;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PageViewModel?.LoadState(e.Parameter, null);
            currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (base.Frame.CanGoBack)
            {
                base.Frame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var dictionary = new Dictionary<string, object>();
            PageViewModel?.SaveState(dictionary);
            currentView.BackRequested -= OnBackRequested;
            currentView = null;
        }
    }
}
