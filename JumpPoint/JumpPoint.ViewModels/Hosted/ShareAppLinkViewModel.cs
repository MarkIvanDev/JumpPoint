using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using NittyGritty.Collections;
using NittyGritty.Commands;
using NittyGritty.Extensions;
using NittyGritty.Platform.Payloads;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels.Hosted
{
    public class ShareAppLinkViewModel : ViewModelBase
    {
        private IShareTargetPayload shareTargetPayload = null;


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

        private bool _exists;

        public bool Exists
        {
            get { return _exists; }
            set
            {
                Set(ref _exists, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private string _appName;

        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _appId;

        public string AppId
        {
            get { return _appId; }
            set { Set(ref _appId, value); }
        }

        private Stream _logo;

        public Stream Logo
        {
            get { return _logo; }
            set { Set(ref _logo, value); }
        }

        private Color _background;

        public Color Background
        {
            get { return _background; }
            set { Set(ref _background, value); }
        }

        private TrackableCollection<ValueInfo> _queryKeys;

        public TrackableCollection<ValueInfo> QueryKeys
        {
            get { return _queryKeys; }
            set
            {
                Set(ref _queryKeys, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private TrackableCollection<ValueInfo> _inputKeys;

        public TrackableCollection<ValueInfo> InputKeys
        {
            get { return _inputKeys; }
            set
            {
                Set(ref _inputKeys, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private AppLinkLaunchTypes _launchTypes;

        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set
            {
                Set(ref _launchTypes, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

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

        private AsyncRelayCommand _shareCommand;
        public AsyncRelayCommand ShareCommand => _shareCommand ?? (_shareCommand = new AsyncRelayCommand(
            async () =>
            {
                if (IsValid)
                {
                    var appLink = new AppLinkInfo()
                    {
                        Link = Link,
                        Name = Name,
                        Description = Description,
                        AppName = AppName,
                        AppId = AppId,
                        Logo = Logo.ToByteArray(),
                        Background = $"#{Background.A:X2}{Background.R:X2}{Background.G:X2}{Background.B:X2}",
                        QueryKeys = QueryKeys.Select(i => i.Key).ToArray(),
                        InputKeys = InputKeys,
                        LaunchTypes = LaunchTypes
                    };
                    await AppLinkService.Create(appLink);
                    shareTargetPayload?.Completed();
                }
            }));

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Link)) return false;
                if (Exists) return false;
                if (string.IsNullOrWhiteSpace(Name)) return false;
                if (LaunchTypes == AppLinkLaunchTypes.None) return false;
                if (QueryKeys.Any(i => string.IsNullOrWhiteSpace(i.Key))) return false;
                if (InputKeys.Any(i => string.IsNullOrWhiteSpace(i.Key))) return false;
                return true;
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            IsLoading = true;
            if (parameter is IShareTargetPayload payload)
            {
                shareTargetPayload = payload;
                var appLink = await AppLinkService.GetAppLink(payload);
                Link = appLink.Link;
                Exists = await AppLinkService.LinkExists(appLink.Link);
                Name = appLink.Name;
                AppName = appLink.AppName;
                AppId = appLink.AppId;
                Logo = appLink.Logo;
                Background = appLink.Background;
                QueryKeys = new TrackableCollection<ValueInfo>(appLink.Query, true);
                InputKeys = new TrackableCollection<ValueInfo>(appLink.InputData, true);
                LaunchTypes = appLink.LaunchTypes;
                QueryKeys.CollectionChanged += QueryKeys_CollectionChanged;
                QueryKeys.ItemPropertyChanged += QueryKeys_ItemPropertyChanged;
                InputKeys.CollectionChanged += InputKeys_CollectionChanged;
                InputKeys.ItemPropertyChanged += InputKeys_ItemPropertyChanged;
            }
            IsLoading = false;
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

        public override void SaveState(Dictionary<string, object> state)
        {
            QueryKeys.CollectionChanged -= QueryKeys_CollectionChanged;
            QueryKeys.ItemPropertyChanged -= QueryKeys_ItemPropertyChanged;
            InputKeys.CollectionChanged -= InputKeys_CollectionChanged;
            InputKeys.ItemPropertyChanged -= InputKeys_ItemPropertyChanged;

        }
    }
}
