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
        private readonly AppSettings appSettings;

        public PropertiesViewModel(IDialogService dialogService, AppSettings appSettings)
        {
            this.dialogService = dialogService;
            this.appSettings = appSettings;
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
