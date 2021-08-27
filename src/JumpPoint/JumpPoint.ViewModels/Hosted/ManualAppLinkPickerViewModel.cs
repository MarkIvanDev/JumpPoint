using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using NittyGritty.Collections;
using NittyGritty.Commands;
using NittyGritty.Extensions;
using NittyGritty.Platform.Launcher;
using NittyGritty.Platform.Payloads;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels.Hosted
{
    public class ManualAppLinkPickerViewModel : ViewModelBase
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IProtocolForResultsPayload protocolForResultsPayload = null;

        public ManualAppLinkPickerViewModel()
        {

        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                Set(ref _name, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                Set(ref _link, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private Collection<IAppInfo> _appHandlers;

        public Collection<IAppInfo> AppHandlers
        {
            get { return _appHandlers; }
            set
            {
                Set(ref _appHandlers, value);
                AppHandler = null;
            }
        }

        private IAppInfo _appHandler;

        public IAppInfo AppHandler
        {
            get { return _appHandler; }
            set
            {
                Set(ref _appHandler, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private AppLinkLaunchTypes _launchTypes;

        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set
            {
                Set(ref _launchTypes, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        private TrackableCollection<ValueInfo> _queryKeys;

        public TrackableCollection<ValueInfo> QueryKeys
        {
            get { return _queryKeys ?? (_queryKeys = new TrackableCollection<ValueInfo>(Enumerable.Empty<ValueInfo>(), true)); }
            set { Set(ref _queryKeys, value); }
        }

        private TrackableCollection<ValueInfo> _inputKeys;

        public TrackableCollection<ValueInfo> InputKeys
        {
            get { return _inputKeys ?? (_inputKeys = new TrackableCollection<ValueInfo>(Enumerable.Empty<ValueInfo>(), true)); }
            set { Set(ref _inputKeys, value); }
        }

        private AsyncRelayCommand _findAppsCommand;
        public AsyncRelayCommand FindAppsCommand => _findAppsCommand ?? (_findAppsCommand = new AsyncRelayCommand(
            async () =>
            {
                var appHandlers = await AppLinkService.FindAppHandlers(Link);
                AppHandlers = new Collection<IAppInfo>(appHandlers);
            }));

        private RelayCommand _AddQueryKey;
        public RelayCommand AddQueryKeyCommand => _AddQueryKey ?? (_AddQueryKey = new RelayCommand(
            () =>
            {
                QueryKeys.Add(new ValueInfo());
            }));

        private RelayCommand<ValueInfo> _RemoveQueryKey;
        public RelayCommand<ValueInfo> RemoveQueryKeyCommand => _RemoveQueryKey ?? (_RemoveQueryKey = new RelayCommand<ValueInfo>(
            key =>
            {
                QueryKeys.Remove(key);
            }));

        private RelayCommand _addInputKeyCommand;
        public RelayCommand AddInputKeyCommand => _addInputKeyCommand ?? (_addInputKeyCommand = new RelayCommand(
            () =>
            {
                InputKeys.Add(new ValueInfo());
            }));

        private RelayCommand<ValueInfo> _removeInputKeyCommand;
        public RelayCommand<ValueInfo> RemoveInputKeyCommand => _removeInputKeyCommand ?? (_removeInputKeyCommand = new RelayCommand<ValueInfo>(
            (key) =>
            {
                InputKeys.Remove(key);
            }));

        private RelayCommand _Create;
        public RelayCommand CreateCommand => _Create ?? (_Create = new RelayCommand(
            () =>
            {
                protocolForResultsPayload.ReportResults(new Dictionary<string, object>
                {
                    { nameof(AppLinkPayload.Link), Link },
                    { nameof(AppLinkPayload.Name), Name },
                    { nameof(AppLinkPayload.Description), Description },
                    { nameof(AppLinkPayload.AppName), AppHandler.DisplayName },
                    { nameof(AppLinkPayload.AppId), AppHandler.PackageFamilyName },
                    { nameof(AppLinkPayload.Logo), AppHandler.Logo.ToByteArray() },
                    { nameof(AppLinkPayload.Background), Color.Transparent.ToHex() },
                    { nameof(AppLinkPayload.LaunchTypes), (int)LaunchTypes },
                    { nameof(AppLinkPayload.QueryKeys), QueryKeys.Select(q => q.Key).ToArray() },
                    { nameof(AppLinkPayload.InputKeys), InputKeys.Select(i => i.Key).ToArray() }
                });
            },
            () =>
            {
                return !string.IsNullOrWhiteSpace(Name) &&
                       Uri.TryCreate(Link, UriKind.Absolute, out _) &&
                       AppHandler != null &&
                       LaunchTypes != AppLinkLaunchTypes.None &&
                       !QueryKeys.Any(i => string.IsNullOrWhiteSpace(i.Key)) &&
                       !InputKeys.Any(i => string.IsNullOrWhiteSpace(i.Key));
            }));

        private RelayCommand _Cancel;
        public RelayCommand CancelCommand => _Cancel ?? (_Cancel = new RelayCommand(
            () =>
            {
                protocolForResultsPayload.ReportResults(null);
            }));

        public override void LoadState(object parameter, Dictionary<string, object> state)
        {
            protocolForResultsPayload = parameter as IProtocolForResultsPayload;
            QueryKeys.CollectionChanged += QueryKeys_CollectionChanged;
            QueryKeys.ItemPropertyChanged += QueryKeys_ItemPropertyChanged;
            InputKeys.CollectionChanged += InputKeys_CollectionChanged;
            InputKeys.ItemPropertyChanged += InputKeys_ItemPropertyChanged;
            this.PropertyChanged += NewAppLinkViewModel_PropertyChanged;
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            QueryKeys.Clear();
            QueryKeys.CollectionChanged -= QueryKeys_CollectionChanged;
            QueryKeys.ItemPropertyChanged -= QueryKeys_ItemPropertyChanged;
            InputKeys.Clear();
            InputKeys.CollectionChanged -= InputKeys_CollectionChanged;
            InputKeys.ItemPropertyChanged -= InputKeys_ItemPropertyChanged;
            this.PropertyChanged -= NewAppLinkViewModel_PropertyChanged;
        }


        private void QueryKeys_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private void QueryKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private void InputKeys_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private void InputKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private async void NewAppLinkViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppHandler))
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (Uri.TryCreate(Link, UriKind.Absolute, out var uri))
                    {
                        LaunchTypes = await AppLinkService.GetLaunchTypes(uri, AppHandler?.PackageFamilyName);
                    }

                }
                catch { }
                finally
                {
                    semaphore.Release();
                }
            }
        }

    }
}
