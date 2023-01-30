using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using Nito.AsyncEx;
using NittyGritty.Models;
using MainThread = Xamarin.Essentials.MainThread;

namespace JumpPoint.ViewModels.Helpers
{
    public class ShellItems
    {
        private readonly AsyncLock mutex;
        private readonly AsyncLock mutexFavorites;
        private readonly AsyncLock mutexWorkspaces;
        private readonly AsyncLock mutexDrives;
        private readonly AsyncLock mutexCloudDrives;
        private readonly AsyncLock mutexWsl;

        public ShellItems()
        {
            mutex = new AsyncLock();
            mutexFavorites = new AsyncLock();
            mutexWorkspaces = new AsyncLock();
            mutexDrives = new AsyncLock();
            mutexCloudDrives = new AsyncLock();
            mutexWsl = new AsyncLock();

            Favorites = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = AppPath.Favorites,
                Key = ViewModelKeys.Favorites,
                Parameter = null,
                Tag = AppPath.Favorites,
                Children = new ObservableCollection<ShellItem>()
            };
            Workspaces = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = AppPath.Workspaces,
                Key = ViewModelKeys.Workspaces,
                Parameter = null,
                Tag = AppPath.Workspaces,
                Children = new ObservableCollection<ShellItem>()
            };
            OneDrive = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = null,
                Key = ViewModelKeys.Folder,
                Parameter = null,
                Tag = FolderTemplate.OneDrive
            };
            Drives = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = AppPath.Drives,
                Key = ViewModelKeys.Drives,
                Parameter = null,
                Tag = AppPath.Drives,
                Children = new ObservableCollection<ShellItem>()
            };
            CloudDrives = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = AppPath.CloudDrives,
                Key = ViewModelKeys.CloudDrives,
                Parameter = null,
                Tag = AppPath.CloudDrives,
                Children = new ObservableCollection<ShellItem>()
            };
            WSL = new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = AppPath.WSL,
                Key = ViewModelKeys.WSL,
                Parameter = null,
                Tag = AppPath.WSL,
                Children = new ObservableCollection<ShellItem>()
            };

            Sidebar = new ObservableCollection<ShellItem>
            {
                new ShellItem()
                {
                    Type = ShellItemType.Item,
                    Content = AppPath.Dashboard,
                    Key = ViewModelKeys.Dashboard,
                    Parameter = null,
                    Tag = AppPath.Dashboard
                },
                Favorites,
                Workspaces,
                ShellItem.Separator,
                OneDrive,
                Drives,
                CloudDrives,
                WSL,
                ShellItem.Separator,
                new ShellItem()
                {
                    Type = ShellItemType.Item,
                    Content = AppPath.AppLinks,
                    Key = ViewModelKeys.AppLinks,
                    Parameter = null,
                    Tag = AppPath.AppLinks
                }
            };
            Footer = new ObservableCollection<ShellItem>
            {
                new ShellItem()
                {
                    Type = ShellItemType.Item,
                    Content = AppPath.ClipboardManager,
                    Key = ViewModelKeys.ClipboardManager,
                    Parameter = null,
                    Tag = AppPath.ClipboardManager
                },
                new ShellItem()
                {
                    Type = ShellItemType.Item,
                    Content = AppPath.Chat,
                    Key = ViewModelKeys.Chatbot,
                    Parameter = null,
                    Tag = AppPath.Chat
                }
            };
        }

        public ObservableCollection<ShellItem> Sidebar { get; }

        public ObservableCollection<ShellItem> Footer { get; }

        public ShellItem Favorites { get; }

        public ShellItem Workspaces { get; }

        public ShellItem OneDrive { get; }

        public ShellItem Drives { get; }

        public ShellItem CloudDrives { get; }

        public ShellItem WSL { get; }

        public void Start()
        {
            Messenger.Default.Register<NotificationMessage<SidebarMessage>>(this, MessengerTokens.SidebarManagement, UpdateSidebar);
            PortableStorageService.PortableDriveCollectionChanged += PortableStorageService_PortableDriveCollectionChanged;
        }

        private async void PortableStorageService_PortableDriveCollectionChanged(object sender, PortableDriveCollectionChangedEventArgs e)
        {
            using (await mutexDrives.LockAsync())
            {
                var existingItem = Drives.Children.FirstOrDefault(i => i.Content is PortableDrive pd && pd.DeviceId == e.DeviceId);
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                        var drive = await PortableStorageService.GetDriveFromId(e.DeviceId);
                        if (drive != null)
                        {
                            await JumpPointService.Load(drive);
                            var item = GetShellItem(drive);
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                if (existingItem != null)
                                {
                                    var index = Drives.Children.IndexOf(existingItem);
                                    Drives.Children[index] = item;
                                }
                                else
                                {
                                    Drives.Children.Add(item);
                                }
                            });
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            if (existingItem != null)
                            {
                                Drives.Children.Remove(existingItem);
                            }
                        });
                        break;

                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Reset:
                    default:
                        break;
                }
            }
        }

        private async void UpdateSidebar(NotificationMessage<SidebarMessage> message)
        {
            try
            {
                switch (message.Notification)
                {
                    case nameof(AppPath.Favorites):
                        await UpdateFavoritesSidebar(message.Content);
                        break;

                    case nameof(AppPath.Workspaces):
                        await UpdateWorkspacesSidebar(message.Content);
                        break;

                    case nameof(AppPath.Folder):
                        await UpdateFavoritesSidebar(message.Content);
                        break;

                    case nameof(AppPath.Workspace):
                        await UpdateFavoritesSidebar(message.Content);
                        await UpdateWorkspacesSidebar(message.Content);
                        break;

                    case nameof(AppPath.CloudDrives):
                        await RefreshCloudDrives();
                        break;

                    case nameof(AppPath.WSL):
                        await RefreshWSL();
                        break;

                    default:
                        break;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        private async Task UpdateFavoritesSidebar(SidebarMessage message)
        {
            switch (message.Action)
            {
                case CollectionChangedAction.Add:
                    using (await mutexFavorites.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            if ((jpi.Type == JumpPointItemType.Workspace || jpi.Type == JumpPointItemType.Drive || jpi.Type == JumpPointItemType.Folder) &&
                                Favorites.Children.FirstOrDefault(i =>
                                    i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                    item.Path == jpi.Path) is null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => Favorites.Children.Add(GetShellItem(jpi)));
                            }
                        }
                    }
                    break;

                case CollectionChangedAction.Remove:
                    using (await mutexFavorites.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            var toRemove = Favorites.Children.FirstOrDefault(i =>
                                i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                item.Path == jpi.Path);
                            if (toRemove != null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => Favorites.Children.Remove(toRemove));
                            }
                        }
                    }
                    break;

                case CollectionChangedAction.Reset:
                    await RefreshFavorites();
                    break;

                case CollectionChangedAction.Update:
                    using (await mutexFavorites.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            var toUpdate = Favorites.Children.FirstOrDefault(i =>
                                i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                item.Path == jpi.Path);
                            if (toUpdate != null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => UpdateShellItem(toUpdate, jpi));
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private async Task UpdateWorkspacesSidebar(SidebarMessage message)
        {
            switch (message.Action)
            {
                case CollectionChangedAction.Add:
                    using (await mutexWorkspaces.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            if (Workspaces.Children.FirstOrDefault(i =>
                                i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                item.Path == jpi.Path) is null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => Workspaces.Children.Add(GetShellItem(jpi)));
                            }
                        }
                    }
                    break;

                case CollectionChangedAction.Remove:
                    using (await mutexWorkspaces.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            var toRemove = Workspaces.Children.FirstOrDefault(i =>
                                i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                item.Path == jpi.Path);
                            if (toRemove != null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => Workspaces.Children.Remove(toRemove));
                            }
                        }
                    }
                    break;

                case CollectionChangedAction.Reset:
                    await RefreshWorkspaces();
                    break;

                case CollectionChangedAction.Update:
                    using (await mutexWorkspaces.LockAsync())
                    {
                        foreach (var jpi in message.Items)
                        {
                            var toUpdate = Workspaces.Children.FirstOrDefault(i =>
                                i.Content is JumpPointItem item && item.Type == jpi.Type &&
                                item.Path == jpi.Path);
                            if (toUpdate != null)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() => UpdateShellItem(toUpdate, jpi));
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        public void Stop()
        {
            Messenger.Default.Unregister<NotificationMessage<SidebarMessage>>(this, MessengerTokens.SidebarManagement, UpdateSidebar);
            PortableStorageService.PortableDriveCollectionChanged -= PortableStorageService_PortableDriveCollectionChanged;
        }

        public async Task Refresh()
        {
            using (await mutex.LockAsync())
            {
                await RefreshFavorites();
                await RefreshWorkspaces();
                await RefreshDrives();
                await RefreshCloudDrives();
                await RefreshWSL();
            }
        }

        public async Task RefreshFavorites()
        {
            using (await mutexFavorites.LockAsync())
            {
                Favorites.Children.Clear();

                var favoriteWorkspaces = await DashboardService.GetFavorites(JumpPointItemType.Workspace);
                foreach (var item in favoriteWorkspaces.Cast<Workspace>())
                {
                    Favorites.Children.Add(GetShellItem(item));
                }

                var favoriteDrives = await DashboardService.GetFavorites(JumpPointItemType.Drive);
                foreach (var item in favoriteDrives.Cast<DriveBase>())
                {
                    Favorites.Children.Add(GetShellItem(item));
                }

                var favoritesFolders = await DashboardService.GetFavorites(JumpPointItemType.Folder);
                foreach (var item in favoritesFolders.Cast<FolderBase>())
                {
                    item.FolderTemplate = await FolderTemplateService.GetFolderTemplate(item);
                    Favorites.Children.Add(GetShellItem(item));
                }
            }
        }
        
        public async Task RefreshWorkspaces()
        {
            using (await mutexWorkspaces.LockAsync())
            {
                Workspaces.Children.Clear();

                var ws = await WorkspaceService.GetWorkspaces();
                foreach (var item in ws)
                {
                    Workspaces.Children.Add(GetShellItem(item));
                }
            }
        }
        
        public async Task RefreshDrives()
        {
            using (await mutexDrives.LockAsync())
            {
                var oneDrive = await StorageService.GetUserFolder(UserFolderTemplate.OneDrive);
                if (oneDrive is null)
                {
                    Sidebar.Remove(OneDrive);
                }
                else
                {
                    OneDrive.Content = oneDrive;
                    OneDrive.Parameter = TabbedNavigationHelper.GetParameter(oneDrive);
                    if (!Sidebar.Contains(OneDrive))
                    {
                        Sidebar.Add(OneDrive);
                    }
                }

                Drives.Children.Clear();

                var ds = await StorageService.GetDrives();
                foreach (var drive in ds)
                {
                    Drives.Children.Add(GetShellItem(drive));
                }
            }
        } 
        
        public async Task RefreshCloudDrives()
        {
            using (await mutexCloudDrives.LockAsync())
            {
                CloudDrives.Children.Clear();

                var drives = await CloudStorageService.GetDrives();
                foreach (var item in drives)
                {
                    CloudDrives.Children.Add(GetShellItem(item));
                }
            }
        }

        public async Task RefreshWSL()
        {
            using (await mutexWsl.LockAsync())
            {
                WSL.Children.Clear();

                var wsl = await WslStorageService.GetDrives();
                foreach (var item in wsl)
                {
                    WSL.Children.Add(GetShellItem(item));
                }
            }
        }
        
        private Task RefreshLibraries()
        {
            return Task.CompletedTask;
            //var librariesShellItem = new ShellItem()
            //{
            //    Type = ShellItemType.Header,
            //    Content = nameof(AppPath.Libraries),
            //    Key = ViewModelKeys.Libraries,
            //    Parameter = null,
            //    Tag = nameof(JumpPointItemType.Library),
            //    Children = new ObservableCollection<ShellItem>()
            //};
            

            //var libs = await LibraryService.GetLibraries();
            //foreach (var lib in libs)
            //{
            //    librariesShellItem.Children.Add(new ShellItem()
            //    {
            //        Type = ShellItemType.Item,
            //        Content = lib.DisplayName,
            //        Key = ViewModelKeys.Library,
            //        Parameter = new QueryString()
            //        {
            //            { nameof(PathInfo.Type), AppPath.Libraries.ToString() },
            //            { nameof(PathInfo.Path), lib.Path }
            //        }.ToString(),
            //        Tag = lib.LibraryTemplate
            //    });
            //}
        }

        #region Get ShellItem
        public static ShellItem GetShellItem(JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.Drive when item is DriveBase drive:
                    return GetShellItem(drive);

                case JumpPointItemType.Folder when item is FolderBase folder:
                    return GetShellItem(folder);

                case JumpPointItemType.File when item is FileBase file:
                    return GetShellItem(file);

                case JumpPointItemType.Workspace when item is Workspace workspace:
                    return GetShellItem(workspace);

                case JumpPointItemType.AppLink when item is AppLink appLink:
                    return GetShellItem(appLink);

                case JumpPointItemType.Library when item is Library library:
                    return GetShellItem(library);

                case JumpPointItemType.Unknown:
                default:
                    return null;
            }
        }

        private static ShellItem GetShellItem(DriveBase drive)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = drive,
                Key = ViewModelKeys.Drive,
                Parameter = TabbedNavigationHelper.GetParameter(drive),
                Tag = drive.DriveTemplate
            };
        }

        private static ShellItem GetShellItem(FolderBase folder)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = folder,
                Key = ViewModelKeys.Folder,
                Parameter = TabbedNavigationHelper.GetParameter(folder),
                Tag = folder.FolderTemplate
            };
        }

        private static ShellItem GetShellItem(FileBase file)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = file,
                Key = null,
                Parameter = TabbedNavigationHelper.GetParameter(file),
                Tag = null
            };
        }

        private static ShellItem GetShellItem(Workspace workspace)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = workspace,
                Key = ViewModelKeys.Workspace,
                Parameter = TabbedNavigationHelper.GetParameter(workspace),
                Tag = workspace.Template
            };
        }

        private static ShellItem GetShellItem(Library library)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = library,
                Key = ViewModelKeys.Library,
                Parameter = TabbedNavigationHelper.GetParameter(library),
                Tag = library.LibraryTemplate
            };
        }

        private static ShellItem GetShellItem(AppLink appLink)
        {
            return new ShellItem()
            {
                Type = ShellItemType.Item,
                Content = appLink,
                Key = null,
                Parameter = TabbedNavigationHelper.GetParameter(appLink),
                Tag = null
            };
        }

        #endregion

        #region Update ShellItem
        public static void UpdateShellItem(ShellItem shellItem, JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.Drive when item is DriveBase drive:
                    UpdateShellItem(shellItem, drive);
                    break;

                case JumpPointItemType.Folder when item is FolderBase folder:
                    UpdateShellItem(shellItem, folder);
                    break;

                case JumpPointItemType.File when item is FileBase file:
                    UpdateShellItem(shellItem, file);
                    break;

                case JumpPointItemType.Workspace when item is Workspace workspace:
                    UpdateShellItem(shellItem, workspace);
                    break;

                case JumpPointItemType.AppLink when item is AppLink appLink:
                    UpdateShellItem(shellItem, appLink);
                    break;

                case JumpPointItemType.Library when item is Library library:
                    UpdateShellItem(shellItem, library);
                    break;

                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

        private static void UpdateShellItem(ShellItem shellItem, DriveBase drive)
        {
            shellItem.Content = drive;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(drive);
            shellItem.Tag = drive.DriveTemplate;
        }

        private static void UpdateShellItem(ShellItem shellItem, FolderBase folder)
        {
            shellItem.Content = folder;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(folder);
            shellItem.Tag = folder.FolderTemplate;
        }

        private static void UpdateShellItem(ShellItem shellItem, FileBase file)
        {
            shellItem.Content = file;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(file);
            shellItem.Tag = null;
        }

        private static void UpdateShellItem(ShellItem shellItem, Workspace workspace)
        {
            shellItem.Content = workspace;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(workspace);
            shellItem.Tag = workspace.Template;
        }

        private static void UpdateShellItem(ShellItem shellItem, Library library)
        {
            shellItem.Content = library;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(library);
            shellItem.Tag = library.LibraryTemplate;
        }

        private static void UpdateShellItem(ShellItem shellItem, AppLink appLink)
        {
            shellItem.Content = appLink;
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(appLink);
            shellItem.Tag = null;
        }

        public static void UpdateShellItem(ShellItem shellItem, CloudAccount account)
        {
            shellItem.Content = $"{account.Name} ({account.Email})";
            shellItem.Parameter = TabbedNavigationHelper.GetParameter(account);
        }

        #endregion

    }

}
