using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.FullTrust.Core;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Dialogs;
using Newtonsoft.Json;
using NittyGritty.Collections;
using NittyGritty.Commands;
using NittyGritty.Services.Core;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels.Standalone
{
    public class PropertiesViewModel : ViewModelBase
    {
        private static readonly SemaphoreSlim refreshSemaphore = new SemaphoreSlim(1, 1);
        private readonly IDialogService dialogService;

        public PropertiesViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        #region Items

        private Collection<Seed> _seeds;

        public Collection<Seed> Seeds
        {
            get { return _seeds ?? (_seeds = new Collection<Seed>()); }
            set { Set(ref _seeds, value); }
        }

        private Dictionary<string, JPGroup> _items;

        public Dictionary<string, JPGroup> Items
        {
            get
            {
                return _items;
            }
            set
            {
                Set(ref _items, value);
            }
        }

        #endregion

        #region General

        private ulong _subtotalFreeSpaceDrives;

        public ulong SubtotalFreeSpaceDrives
        {
            get { return _subtotalFreeSpaceDrives; }
            set { Set(ref _subtotalFreeSpaceDrives, value); }
        }

        #region Size
        public ulong Total
        {
            get { return SubtotalSizeDrives + SubtotalSizeFolders + SubtotalSizeFiles + SubtotalSizeWorkspaces + SubtotalSizeSettings; }
        }

        private ulong _subtotalSizeDrives;

        public ulong SubtotalSizeDrives
        {
            get { return _subtotalSizeDrives; }
            set
            {
                Set(ref _subtotalSizeDrives, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ulong _subtotalSizeFolders;

        public ulong SubtotalSizeFolders
        {
            get { return _subtotalSizeFolders; }
            set
            {
                Set(ref _subtotalSizeFolders, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ulong _subtotalSizeFiles;

        public ulong SubtotalSizeFiles
        {
            get { return _subtotalSizeFiles; }
            set
            {
                Set(ref _subtotalSizeFiles, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ulong _subtotalSizeWorkspaces;

        public ulong SubtotalSizeWorkspaces
        {
            get { return _subtotalSizeWorkspaces; }
            set
            {
                Set(ref _subtotalSizeWorkspaces, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private ulong _subtotalSizeSettings;

        public ulong SubtotalSizeSettings
        {
            get { return _subtotalSizeSettings; }
            set
            {
                Set(ref _subtotalSizeSettings, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        #endregion

        #region File and Folder Count

        private ulong _subtotalFileCountFolders;

        public ulong SubtotalFileCountFolders
        {
            get { return _subtotalFileCountFolders; }
            set { Set(ref _subtotalFileCountFolders, value); }
        }

        private ulong _subtotalFolderCountFolders;

        public ulong SubtotalFolderCountFolders
        {
            get { return _subtotalFolderCountFolders; }
            set { Set(ref _subtotalFolderCountFolders, value); }
        }

        private ulong _subtotalFileCountWorkspaces;

        public ulong SubtotalFileCountWorkspaces
        {
            get { return _subtotalFileCountWorkspaces; }
            set { Set(ref _subtotalFileCountWorkspaces, value); }
        }

        private ulong _subtotalFolderCountWorkspaces;

        public ulong SubtotalFolderCountWorkspaces
        {
            get { return _subtotalFolderCountWorkspaces; }
            set { Set(ref _subtotalFolderCountWorkspaces, value); }
        }

        private ulong _subtotalDriveCountWorkspaces;

        public ulong SubtotalDriveCountWorkspaces
        {
            get { return _subtotalDriveCountWorkspaces; }
            set { Set(ref _subtotalDriveCountWorkspaces, value); }
        }

        private ulong _subtotalAppLinkCountWorkspaces;

        public ulong SubtotalAppLinkCountWorkspaces
        {
            get { return _subtotalAppLinkCountWorkspaces; }
            set { Set(ref _subtotalAppLinkCountWorkspaces, value); }
        }

        #endregion

        #region Attributes
        public Collection<FileAttributes> Attributes
        {
            get
            {
                return new Collection<FileAttributes>()
                {
                    FileAttributes.Normal,
                    FileAttributes.ReadOnly,
                    FileAttributes.Hidden,
                    FileAttributes.System,
                    FileAttributes.Directory,
                    FileAttributes.Archive,
                    FileAttributes.Temporary,
                    FileAttributes.Compressed,
                    FileAttributes.Encrypted,
                };
            }
        }

        private Collection<FileAttributes> _subtotalAttributesFolders;

        public Collection<FileAttributes> SubtotalAttributesFolders
        {
            get { return _subtotalAttributesFolders ?? (_subtotalAttributesFolders = new Collection<FileAttributes>()); }
            set { Set(ref _subtotalAttributesFolders, value); }
        }

        private Collection<FileAttributes> _subtotalAttributesFiles;

        public Collection<FileAttributes> SubtotalAttributesFiles
        {
            get { return _subtotalAttributesFiles ?? (_subtotalAttributesFiles = new Collection<FileAttributes>()); }
            set { Set(ref _subtotalAttributesFiles, value); }
        }

        #endregion

        #endregion

        #region Common Commands

        private AsyncRelayCommand<JumpPointItem> _openCommand;
        public AsyncRelayCommand<JumpPointItem> OpenCommand => _openCommand ?? (_openCommand = new AsyncRelayCommand<JumpPointItem>(
            async item =>
            {
                switch (item.Type)
                {
                    case JumpPointItemType.Folder:
                        await JumpPointService.OpenNewWindow(AppPath.Folder, item.Path);
                        break;
                    case JumpPointItemType.Drive:
                        await JumpPointService.OpenNewWindow(AppPath.Drive, item.Path);
                        break;
                    case JumpPointItemType.Workspace:
                        await JumpPointService.OpenNewWindow(AppPath.Workspace, item.Path);
                        break;
                    case JumpPointItemType.File when item is FileBase file:
                        var fileLaunchResult = await JumpPointService.OpenFile(file, true);
                        if (!fileLaunchResult)
                        {
                            var openInFE = await dialogService.ShowMessage("You could open the file from file explorer which we will open for you. Proceed?",
                                "Open in File Explorer?", "Open", "Cancel");
                            if (openInFE)
                            {
                                await JumpPointService.OpenInFileExplorer(Path.GetDirectoryName(item.Path), new List<StorageItemBase> { file });
                            }
                        }
                        break;
                    case JumpPointItemType.AppLink when item is AppLink appLink:
                        var launchType = appLink.LaunchTypes;
                        if (launchType == AppLinkLaunchTypes.All || appLink.Query.Count > 0 || appLink.InputData.Count > 0)
                        {
                            var appLinkLaunch = new AppLinkLaunchViewModel(appLink);
                            await dialogService.Show(DialogKeys.AppLinkLaunch, appLinkLaunch);
                            launchType = appLinkLaunch.LaunchType;
                        }

                        switch (launchType)
                        {
                            case AppLinkLaunchTypes.Uri:
                                await AppLinkService.OpenUri(appLink);
                                break;
                            case AppLinkLaunchTypes.UriForResults:
                                var results = await AppLinkService.OpenUriForResults(appLink);
                                var appLinkLaunchResults = new AppLinkLaunchResultsViewModel(appLink, results);
                                await dialogService.Show(DialogKeys.AppLinkLaunchResults, appLinkLaunchResults);
                                break;
                            case AppLinkLaunchTypes.All:
                            case AppLinkLaunchTypes.None:
                            default:
                                break;
                        }
                        break;

                    case JumpPointItemType.Library:
                        break;
                }
            }));

        private AsyncRelayCommand<JumpPointItem> _openFileExplorerCommand;
        public AsyncRelayCommand<JumpPointItem> OpenFileExplorerCommand => _openFileExplorerCommand ?? (_openFileExplorerCommand = new AsyncRelayCommand<JumpPointItem>(
            async item =>
            {
                switch (item.Type)
                {
                    case JumpPointItemType.File:
                        var parent = Directory.GetParent(item.Path).FullName;
                        await JumpPointService.OpenInFileExplorer(parent, Enumerable.Repeat(item as StorageItemBase, 1));
                        break;
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Drive:
                        await JumpPointService.OpenInFileExplorer(item.Path);
                        break;
                    case JumpPointItemType.Workspace:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.AppLink:
                    case JumpPointItemType.Unknown:
                    default:
                        break;
                }

            }));

        private AsyncRelayCommand<JumpPointItem> _openCommandPromptCommand;
        public AsyncRelayCommand<JumpPointItem> OpenCommandPromptCommand => _openCommandPromptCommand ?? (_openCommandPromptCommand = new AsyncRelayCommand<JumpPointItem>(
            async item =>
            {
                switch (item.Type)
                {
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Drive:
                        await DesktopService.OpenInCommandPrompt(new List<string> { item.Path });
                        break;
                    case JumpPointItemType.File:
                    case JumpPointItemType.Workspace:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.AppLink:
                    case JumpPointItemType.Unknown:
                    default:
                        break;
                }
            }));

        private AsyncRelayCommand<JumpPointItem> _openPowershellCommand;
        public AsyncRelayCommand<JumpPointItem> OpenPowershellCommand => _openPowershellCommand ?? (_openPowershellCommand = new AsyncRelayCommand<JumpPointItem>(
            async item =>
            {
                switch (item.Type)
                {
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Drive:
                        await DesktopService.OpenInPowershell(new List<string> { item.Path });
                        break;
                    case JumpPointItemType.File:
                    case JumpPointItemType.Workspace:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.AppLink:
                    case JumpPointItemType.Unknown:
                    default:
                        break;
                }
            }));
        #endregion

        #region Drives

        private AsyncRelayCommand _StorageSense;
        public AsyncRelayCommand StorageSenseCommand => _StorageSense ?? (_StorageSense = new AsyncRelayCommand(
            async () =>
            {
                await Xamarin.Essentials.Launcher.OpenAsync("ms-settings:storagepolicies");
            }));

        private AsyncRelayCommand<DriveBase> _CleanManager;
        public AsyncRelayCommand<DriveBase> CleanManagerCommand => _CleanManager ?? (_CleanManager = new AsyncRelayCommand<DriveBase>(
            async drive =>
            {
                await DesktopService.OpenCleanManager(drive.Path.FirstOrDefault());
            }));

        private AsyncRelayCommand _DiskDefragmenter;
        public AsyncRelayCommand DiskDefragmenterCommand => _DiskDefragmenter ?? (_DiskDefragmenter = new AsyncRelayCommand(
            async () =>
            {
                await DesktopService.OpenSystemApp(SystemApp.DiskDefragmenter);
            }));
        #endregion

        #region Files

        private FileBase _currentFile;

        public FileBase CurrentFile
        {
            get { return _currentFile; }
            set { Set(ref _currentFile, value); }
        }

        #endregion

        private AsyncRelayCommand _refreshCommand;
        public AsyncRelayCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new AsyncRelayCommand(
            async () =>
            {
                await Run(Refresh);
            }));

        private async Task Refresh(CancellationToken token)
        {
            await refreshSemaphore.WaitAsync();
            try
            {
                var drives = new JPGroup(nameof(JumpPointItemType.Drive));
                var folders = new JPGroup(nameof(JumpPointItemType.Folder));
                var files = new JPGroup(nameof(JumpPointItemType.File));
                var workspaces = new JPGroup(nameof(JumpPointItemType.Workspace));
                var apps = new JPGroup(nameof(JumpPointItemType.AppLink));
                
                var fileAttributes = new Collection<FileAttributes>();
                var folderAttributes = new Collection<FileAttributes>();

                foreach (var seed in Seeds)
                {
                    switch (seed.Type)
                    {
                        case JumpPointItemType.File:
                            var file = await StorageService.GetFile(seed.Path);
                            if (file == null) continue;
                            await JumpPointService.Load(file);
                            await StorageService.LoadProperties(file, FileBaseProperties.All);
                            files.Items.Add(file);
                            SubtotalSizeFiles += file.Size ?? 0;
                            fileAttributes.Add(file.Attributes ?? 0);
                            break;
                        case JumpPointItemType.Folder:
                            var folder = await StorageService.GetFolder(seed.Path);
                            if (folder == null) continue;
                            await JumpPointService.Load(folder);
                            folders.Items.Add(folder);
                            folderAttributes.Add(folder.Attributes ?? 0);
                            break;
                        case JumpPointItemType.Drive:
                            var drive = await StorageService.GetDrive(seed.Path);
                            if (drive == null) continue;
                            await JumpPointService.Load(drive);
                            drives.Items.Add(drive);
                            SubtotalFreeSpaceDrives += drive.FreeSpace ?? 0;
                            SubtotalSizeDrives += drive.Capacity ?? 0;
                            break;
                        case JumpPointItemType.Workspace:
                            var workspace = await WorkspaceService.GetWorkspace(seed.Path);
                            if (workspace == null) continue;
                            await JumpPointService.Load(workspace);
                            workspaces.Items.Add(workspace);
                            break;
                        case JumpPointItemType.AppLink:
                            var app = await AppLinkService.GetAppLink(seed.Path);
                            if (app is null) continue;
                            await JumpPointService.Load(app);
                            apps.Items.Add(app);
                            break;
                        case JumpPointItemType.Library:
                        case JumpPointItemType.Unknown:
                        default:
                            break;
                    }
                }
                Items = new Dictionary<string, JPGroup>()
                {
                    { drives.Key, drives },
                    { folders.Key, folders },
                    { files.Key, files },
                    { workspaces.Key, workspaces },
                    { apps.Key, apps }
                };
                CurrentFile = Items[nameof(JumpPointItemType.File)].Items.FirstOrDefault() as FileBase;
                SubtotalAttributesFiles = fileAttributes;
                SubtotalAttributesFolders = folderAttributes;

                foreach (FolderBase f in folders.Items)
                {
                    await ComputeFolderStats(f, f);
                }

                foreach (Workspace w in workspaces.Items)
                {
                    await ComputeWorkspaceStats(w);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
            finally
            {
                refreshSemaphore.Release();
            }
        }

        async Task ComputeFolderStats(FolderBase current, FolderBase parent)
        {
            var items = await StorageService.GetItems(current);
            foreach (var item in items)
            {
                if (item is FolderBase fo)
                {
                    parent.FolderCount += 1;
                    SubtotalFolderCountFolders += 1;
                    await ComputeFolderStats(fo, parent);
                }
                else if (item is FileBase fi)
                {
                    parent.FileCount += 1;
                    parent.Size += fi.Size ?? 0;
                    SubtotalFileCountFolders += 1;
                    SubtotalSizeFolders += fi.Size ?? 0;
                }
            }
        }

        async Task ComputeWorkspaceStats(Workspace workspace)
        {
            var fis = await WorkspaceService.GetItems(workspace.Id, JumpPointItemType.File);
            workspace.FileCount = (ulong)fis.Count;
            SubtotalFileCountWorkspaces += (ulong)fis.Count;
            foreach (var fi in fis.Cast<FileBase>())
            {
                SubtotalSizeWorkspaces += fi.Size ?? 0;
            }

            var fos = await WorkspaceService.GetItems(workspace.Id, JumpPointItemType.Folder);
            workspace.FolderCount = (ulong)fos.Count;
            SubtotalFolderCountWorkspaces += (ulong)fos.Count;
            foreach (var fo in fos.Cast<FolderBase>())
            {
                ulong? size = await StorageService.GetFolderSize(fo);
                SubtotalSizeWorkspaces += size ?? 0;
            }

            var drs = await WorkspaceService.GetItems(workspace.Id, JumpPointItemType.Drive);
            workspace.DriveCount = (ulong)drs.Count;
            SubtotalDriveCountWorkspaces += (ulong)drs.Count;

            var als = await WorkspaceService.GetItems(workspace.Id, JumpPointItemType.AppLink);
            workspace.AppLinkCount = (ulong)als.Count;
            SubtotalAppLinkCountWorkspaces += (ulong)als.Count;
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            if(parameter != null)
            {
                Seeds = JsonConvert.DeserializeObject<Collection<Seed>>(parameter.ToString());
            }
            else if(state != null)
            {
                Seeds = JsonConvert.DeserializeObject<Collection<Seed>>(RestoreStateItem<string>(state, nameof(Seeds)));
            }
            await RefreshCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            state[nameof(Seeds)] = JsonConvert.SerializeObject(Seeds);
            CancelAll();
        }

    }

    public class JPGroup : Group<string, JumpPointItem>
    {
        public JPGroup(string key) : base(key)
        {

        }

        public JPGroup(string key, IList<JumpPointItem> items) : base(key, items)
        {

        }
    }
}
