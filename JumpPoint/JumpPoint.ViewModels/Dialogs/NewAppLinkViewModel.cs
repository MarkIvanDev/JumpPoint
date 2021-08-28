using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using NittyGritty;
using NittyGritty.Collections;
using NittyGritty.Commands;
using NittyGritty.Platform.Launcher;

namespace JumpPoint.ViewModels.Dialogs
{
    public class NewAppLinkViewModel : ObservableObject
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public NewAppLinkViewModel()
        {
            QueryKeys = new TrackableCollection<ValueInfo>(Enumerable.Empty<ValueInfo>(), true);
            QueryKeys.CollectionChanged += QueryKeys_CollectionChanged;
            QueryKeys.ItemPropertyChanged += QueryKeys_ItemPropertyChanged;
            InputKeys = new TrackableCollection<ValueInfo>(Enumerable.Empty<ValueInfo>(), true);
            InputKeys.CollectionChanged += InputKeys_CollectionChanged;
            InputKeys.ItemPropertyChanged += InputKeys_ItemPropertyChanged;
            this.PropertyChanged += NewAppLinkViewModel_PropertyChanged;
        }

        private void QueryKeys_ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsValid));
        }

        private void QueryKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsValid));
        }

        private void InputKeys_ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsValid));
        }

        private void InputKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsValid));
        }

        private async void NewAppLinkViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                Set(ref _name, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                Set(ref _link, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private ReadOnlyCollection<IAppInfo> _appHandlers;

        public ReadOnlyCollection<IAppInfo> AppHandlers
        {
            get { return _appHandlers; }
            set
            {
                Set(ref _appHandlers, value);
                AppHandler = AppHandlers?.FirstOrDefault();
            }
        }

        private IAppInfo _appHandler;

        public IAppInfo AppHandler
        {
            get { return _appHandler; }
            set
            {
                Set(ref _appHandler, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private AppLinkLaunchTypes _launchTypes;

        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set { Set(ref _launchTypes, value); }
        }

        private TrackableCollection<ValueInfo> _queryKeys;

        public TrackableCollection<ValueInfo> QueryKeys
        {
            get { return _queryKeys; }
            set { Set(ref _queryKeys, value); }
        }

        private TrackableCollection<ValueInfo> _inputKeys;

        public TrackableCollection<ValueInfo> InputKeys
        {
            get { return _inputKeys; }
            set { Set(ref _inputKeys, value); }
        }

        private AsyncRelayCommand _findAppsCommand;
        public AsyncRelayCommand FindAppsCommand => _findAppsCommand ?? (_findAppsCommand = new AsyncRelayCommand(
            async () =>
            {
                var appHandlers = await AppLinkService.FindAppHandlers(Link);
                AppHandlers = new ReadOnlyCollection<IAppInfo>(appHandlers);
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

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Name)
                       && Uri.TryCreate(Link, UriKind.Absolute, out _)
                       && AppHandler != null
                       && LaunchTypes != AppLinkLaunchTypes.None
                       && !QueryKeys.Any(i => string.IsNullOrWhiteSpace(i.Key))
                       && !InputKeys.Any(i => string.IsNullOrWhiteSpace(i.Key));
            }
        }
    }
}
