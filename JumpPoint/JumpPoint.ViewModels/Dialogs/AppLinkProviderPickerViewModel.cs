using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Extensions;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class AppLinkProviderPickerViewModel : ObservableObject
    {

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private Collection<AppLinkProvider> _providers;

        public Collection<AppLinkProvider> Providers
        {
            get { return _providers; }
            set { Set(ref _providers, value); }
        }

        private AppLinkProvider _provider;

        public AppLinkProvider Provider
        {
            get { return _provider; }
            set
            {
                Set(ref _provider, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get { return Provider != null; }
        }

        public async Task Initialize()
        {
            IsLoading = true;
            var appLinkProviders = await AppLinkProviderManager.GetProviders();
            Providers = new Collection<AppLinkProvider>(appLinkProviders.Where(p => p.IsAvailable && p.IsEnabled).ToList());
            IsLoading = false;
        }

    }
}
