using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Extensions;
using NittyGritty.Commands;
using NittyGritty.Platform.Payloads;
using NittyGritty.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JumpPoint.ViewModels.Hosted
{
    public class AppLinkProviderViewModel : ViewModelBase
    {
        private IProtocolForResultsPayload protocolForResultsPayload = null;

        public AppLinkProviderViewModel()
        {

        }

        private string _provider;

        public string Provider
        {
            get { return _provider; }
            set { Set(ref _provider, value); }
        }

        private string _service;

        public string Service
        {
            get { return _service; }
            set { Set(ref _service, value); }
        }

        private string _packageId;

        public string PackageId
        {
            get { return _packageId; }
            set { Set(ref _packageId, value); }
        }

        private Collection<AppLinkPayload> _appLinks;

        public Collection<AppLinkPayload> AppLinks
        {
            get { return _appLinks; }
            set { Set(ref _appLinks, value); }
        }

        private AppLinkPayload _appLink;

        public AppLinkPayload AppLink
        {
            get { return _appLink; }
            set
            {
                Set(ref _appLink, value);
                PickCommand.RaiseCanExecuteChanged();
            }
        }

        private AsyncRelayCommand _Refresh;
        public AsyncRelayCommand RefreshCommand => _Refresh ?? (_Refresh = new AsyncRelayCommand(
            async () =>
            {
                var appLinks = await AppLinkProviderManager.GetPayloads(Service, PackageId);
                AppLinks = new Collection<AppLinkPayload>(appLinks);
            }));

        private AsyncRelayCommand _Pick;
        public AsyncRelayCommand PickCommand => _Pick ?? (_Pick = new AsyncRelayCommand(
            async () =>
            {
                if (AppLink != null)
                {
                    if (AppLink.LogoUri != null)
                    {
                        AppLink.Logo = await AppLinkProviderHelper.GetLogo(AppLink.LogoUri);
                    }

                    protocolForResultsPayload?.ReportResults(new Dictionary<string, object>
                    {
                        { nameof(AppLinkPayload.Link), AppLink.Link },
                        { nameof(AppLinkPayload.Name), AppLink.Name },
                        { nameof(AppLinkPayload.Description), AppLink.Description },
                        { nameof(AppLinkPayload.AppName), AppLink.AppName },
                        { nameof(AppLinkPayload.AppId), AppLink.AppId },
                        { nameof(AppLinkPayload.Logo), AppLink.Logo },
                        { nameof(AppLinkPayload.Background), AppLink.Background },
                        { nameof(AppLinkPayload.LaunchTypes), AppLink.LaunchTypes },
                        { nameof(AppLinkPayload.QueryKeys), AppLink.QueryKeys },
                        { nameof(AppLinkPayload.InputKeys), AppLink.InputKeys }
                    });
                }
            },
            () => AppLink != null));

        private RelayCommand _Cancel;
        public RelayCommand CancelCommand => _Cancel ?? (_Cancel = new RelayCommand(
            () =>
            {
                protocolForResultsPayload?.ReportResults(null);
            }));

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            IsLoading = true;
            if (parameter is IProtocolForResultsPayload payload)
            {
                protocolForResultsPayload = payload;
                Provider = payload.Data.TryGetValue(nameof(AppLinkProvider.Name), out var name) ?
                    name.ToString() : null;
                Service = payload.Data.TryGetValue(nameof(AppLinkProvider.Service), out var service) ?
                    service.ToString() : null;
                PackageId = payload.Data.TryGetValue(nameof(AppLinkProvider.PackageId), out var packageId) ?
                    packageId.ToString() : null;
            }
            await RefreshCommand.TryExecute();
            IsLoading = false;
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            
        }
    }
}
