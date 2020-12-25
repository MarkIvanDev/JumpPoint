using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Workspace;
using NittyGritty.Extensions;
using SQLite;

namespace JumpPoint.Platform.Services
{
    public static class WorkspaceService
    {
        private const string WORKSPACE_DATAFILE = "workspaces.db";
        private static readonly SQLiteAsyncConnection connection;

        static WorkspaceService()
        {
            connection = new SQLiteAsyncConnection(Path.Combine(JumpPointService.DataFolder, WORKSPACE_DATAFILE));
        }

        public static async Task Initialize()
        {
            await connection.RunInTransactionAsync(db =>
            {
                var oldTableExists = db.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type = ? AND name = ?", "table", "Workspace") > 0;
                if (oldTableExists)
                {
                    db.Execute("ALTER TABLE Workspace RENAME TO ?", nameof(WorkspaceInfo));
                }
            });
            await connection.CreateTableAsync<WorkspaceInfo>();
            await connection.CreateTableAsync<WorkspaceDriveItem>();
            await connection.CreateTableAsync<WorkspaceFolderItem>();
            await connection.CreateTableAsync<WorkspaceFileItem>();
            await connection.CreateTableAsync<WorkspaceSettingItem>();
            await connection.CreateTableAsync<WorkspaceAppLinkItem>();
        }

        public static ReadOnlyCollection<WorkspaceTemplate> GetTemplates()
        {
            var templates = (WorkspaceTemplate[])Enum.GetValues(typeof(WorkspaceTemplate));
            return new ReadOnlyCollection<WorkspaceTemplate>(templates);
        }
        
        public static async Task SetTemplate(int workspaceId, WorkspaceTemplate template)
        {
            await connection.RunInTransactionAsync((db) =>
            {
                var ws = db.Find<WorkspaceInfo>(workspaceId);
                if (ws != null)
                {
                    ws.Template = template;
                    db.Update(ws);
                }
            });
        }

        public static async Task<IList<Workspace>> GetWorkspaces()
        {
            var workspaces = new List<Workspace>();
            var workspaceInfos = await connection.Table<WorkspaceInfo>().ToListAsync();
            foreach (var item in workspaceInfos)
            {
                workspaces.Add(new Workspace(item.Id, item.Template, item.Name, item.DateCreated));
            }
            return workspaces;
        }

        public static async Task<Workspace> GetWorkspace(string name)
        {
            var ws = await connection.FindWithQueryAsync<WorkspaceInfo>(
                $"SELECT * FROM {nameof(WorkspaceInfo)} WHERE {nameof(WorkspaceInfo.Name)} = ?", name);
            return ws != null ? new Workspace(ws.Id, ws.Template, ws.Name, ws.DateCreated) : null;
        }

        public static Workspace GetWorkspace(WorkspaceInfo workspace)
        {
            return workspace is null ? null :
                new Workspace(
                    id: workspace.Id,
                    template: workspace.Template,
                    name: workspace.Name,
                    dateCreated: workspace.DateCreated);
        }

        public static async Task Load(Workspace workspace)
        {
            workspace.IsFavorite = await DashboardService.GetStatus(workspace);
            await connection.RunInTransactionAsync(db =>
            {
                workspace.AppLinkCount = (ulong)db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {nameof(WorkspaceAppLinkItem)} " +
                    $"WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ?", workspace.Id);
                workspace.DriveCount = (ulong)db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {nameof(WorkspaceDriveItem)} " +
                    $"WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ?", workspace.Id);
                workspace.FolderCount = (ulong)db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {nameof(WorkspaceFolderItem)} " +
                    $"WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ?", workspace.Id);
                workspace.FileCount = (ulong)db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {nameof(WorkspaceFileItem)} " +
                    $"WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ?", workspace.Id);
                workspace.SettingCount = (ulong)db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {nameof(WorkspaceSettingItem)} " +
                    $"WHERE {nameof(WorkspaceSettingItem.WorkspaceId)} = ?", workspace.Id);
            });
        }

        public static async Task<Workspace> Create(WorkspaceInfo workspace)
        {
            Workspace item = null;
            await connection.RunInTransactionAsync(db =>
            {
                var name = workspace.Name;
                var number = 2;
                while (db.ExecuteScalar<int>(
                    $"SELECT COUNT(*) FROM {nameof(WorkspaceInfo)} WHERE {nameof(WorkspaceInfo.Name)} = ?", name) > 0)
                {
                    name = $"{name} ({number})";
                    number += 1;
                }
                workspace.Name = name;
                db.Insert(workspace);
                item = GetWorkspace(workspace);
            });
            return item;
        }

        public static async Task Rename(Workspace workspace, string name, RenameCollisionOption option)
        {
            if (!(workspace is null))
            {
                await connection.RunInTransactionAsync(db =>
                {
                    var newName = name;
                    var number = 2;
                    while (db.ExecuteScalar<int>(
                        $"SELECT COUNT(*) FROM {nameof(WorkspaceInfo)} WHERE {nameof(WorkspaceInfo.Name)} = ?", newName) > 0)
                    {
                        newName = $"{name} ({number})";
                        number += 1;
                    }

                    var item = db.Find<WorkspaceInfo>(workspace.Id);
                    if (!(item is null))
                    {
                        item.Name = newName;
                        db.Update(item);
                        workspace.Name = newName;
                    }
                });
            }
        }

        public static async Task Delete(Workspace workspace, bool deletePermanently)
        {
            var storageItems = new List<string>();
            var appLinks = new List<string>();

            await connection.RunInTransactionAsync((db) =>
            {
                if (deletePermanently)
                {
                    storageItems.AddRange(db.Query<WorkspaceDriveItem>(
                        $"SELECT * FROM {nameof(WorkspaceDriveItem)} WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ?", workspace.Id)
                        .Select(i => i.Path));
                    storageItems.AddRange(db.Query<WorkspaceFileItem>(
                        $"SELECT * FROM {nameof(WorkspaceFileItem)} WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ?", workspace.Id)
                        .Select(i => i.Path));
                    storageItems.AddRange(db.Query<WorkspaceFolderItem>(
                        $"SELECT * FROM {nameof(WorkspaceFolderItem)} WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ?", workspace.Id)
                        .Select(i => i.Path));
                    appLinks.AddRange(db.Query<WorkspaceAppLinkItem>(
                        $"SELECT * FROM {nameof(WorkspaceAppLinkItem)} WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ?", workspace.Id)
                        .Select(i => i.Path));
                }

                db.Execute($"DELETE FROM {nameof(WorkspaceDriveItem)} WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ?", workspace.Id);
                db.Execute($"DELETE FROM {nameof(WorkspaceFolderItem)} WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ?", workspace.Id);
                db.Execute($"DELETE FROM {nameof(WorkspaceFileItem)} WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ?", workspace.Id);
                db.Execute($"DELETE FROM {nameof(WorkspaceSettingItem)} WHERE {nameof(WorkspaceSettingItem.WorkspaceId)} = ?", workspace.Id);
                db.Execute($"DELETE FROM {nameof(WorkspaceAppLinkItem)} WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ?", workspace.Id);
                db.Delete<WorkspaceInfo>(workspace.Id);
            });

            if (deletePermanently)
            {
                await StorageService.Delete(storageItems, false);
                await AppLinkService.Delete(appLinks);
            }
        }

        public static async Task Delete(IList<Workspace> workspaces, bool deletePermanently)
        {
            foreach (var item in workspaces)
            {
                await Delete(item, deletePermanently);
            }
        }

        public static async Task<IList<JumpPointItem>> GetItems(int id)
        {
            var items = new List<JumpPointItem>();
            items.AddRange(await GetItems(id, JumpPointItemType.Drive));
            items.AddRange(await GetItems(id, JumpPointItemType.Folder));
            items.AddRange(await GetItems(id, JumpPointItemType.File));
            items.AddRange(await GetItems(id, JumpPointItemType.AppLink));
            items.AddRange(await GetItems(id, JumpPointItemType.SettingLink));
            return items;
        }

        public static async Task<IList<JumpPointItem>> GetItems(int id, JumpPointItemType type)
        {
            var items = new List<JumpPointItem>();
            switch (type)
            {
                case JumpPointItemType.File:
                    var files = await connection.Table<WorkspaceFileItem>().Where(fi => fi.WorkspaceId == id).ToListAsync();
                    foreach (var file in files)
                    {
                        var item = await LocalStorageService.GetFile(file.Path);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    break;

                case JumpPointItemType.Folder:
                    var folders = await connection.Table<WorkspaceFolderItem>().Where(fi => fi.WorkspaceId == id).ToListAsync();
                    foreach (var folder in folders)
                    {
                        var item = await LocalStorageService.GetFolder(folder.Path);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    break;

                case JumpPointItemType.Drive:
                    var drives = await connection.Table<WorkspaceDriveItem>().Where(fi => fi.WorkspaceId == id).ToListAsync();
                    foreach (var drive in drives)
                    {
                        var item = await LocalStorageService.GetDrive(drive.Path);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    break;

                case JumpPointItemType.SettingLink:
                    var settingLinks = await connection.Table<WorkspaceSettingItem>().Where(fi => fi.WorkspaceId == id).ToListAsync();
                    foreach (var settingLink in settingLinks)
                    {
                        var item = SettingLinkService.GetSettingLink(settingLink.Setting);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    break;

                case JumpPointItemType.AppLink:
                    var appLinks = await connection.Table<WorkspaceAppLinkItem>().Where(fi => fi.WorkspaceId == id).ToListAsync();
                    foreach (var appLink in appLinks)
                    {
                        var item = await AppLinkService.GetAppLink(appLink.Path);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                    break;

                case JumpPointItemType.Workspace:
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
            return items;
        }
    
        public static async Task<bool> ItemExists(int id, JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                    return await connection.FindWithQueryAsync<WorkspaceFileItem>($"SELECT * FROM {nameof(WorkspaceFileItem)} " +
                        $"WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ? AND {nameof(WorkspaceFileItem.Path)} = ?",
                        id, item.Path) != null;

                case JumpPointItemType.Folder:
                    return await connection.FindWithQueryAsync<WorkspaceFolderItem>($"SELECT * FROM {nameof(WorkspaceFolderItem)} " +
                        $"WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ? AND {nameof(WorkspaceFolderItem.Path)} = ?",
                        id, item.Path.NormalizeDirectory()) != null;

                case JumpPointItemType.Drive:
                    return await connection.FindWithQueryAsync<WorkspaceDriveItem>($"SELECT * FROM {nameof(WorkspaceDriveItem)} " +
                        $"WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ? AND {nameof(WorkspaceDriveItem.Path)} = ?",
                        id, item.Path.NormalizeDirectory()) != null;

                case JumpPointItemType.SettingLink when item is SettingLink setting:
                    return await connection.FindWithQueryAsync<WorkspaceSettingItem>($"SELECT * FROM {nameof(WorkspaceSettingItem)} " +
                        $"WHERE {nameof(WorkspaceSettingItem.WorkspaceId)} = ? AND {nameof(WorkspaceSettingItem.Setting)} = ?",
                        id, setting.Template) != null;

                case JumpPointItemType.AppLink:
                    return await connection.FindWithQueryAsync<WorkspaceAppLinkItem>($"SELECT * FROM {nameof(WorkspaceAppLinkItem)} " +
                        $"WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ? AND {nameof(WorkspaceAppLinkItem.Path)} = ?",
                        id, item.Path) != null;

                case JumpPointItemType.Workspace:
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return false;
            }
        }
    
        public static async Task InsertItem(int id, JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                    await connection.RunInTransactionAsync(db =>
                    {
                        var exists = db.FindWithQuery<WorkspaceFileItem>($"SELECT * FROM {nameof(WorkspaceFileItem)} " +
                            $"WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ? AND {nameof(WorkspaceFileItem.Path)} = ?",
                            id, item.Path) != null;
                        if (!exists)
                        {
                            db.Insert(
                                new WorkspaceFileItem()
                                {
                                    WorkspaceId = id,
                                    Path = item.Path
                                });
                        }
                    });
                    break;

                case JumpPointItemType.Folder:
                    await connection.RunInTransactionAsync(db =>
                    {
                        var exists = db.FindWithQuery<WorkspaceFolderItem>($"SELECT * FROM {nameof(WorkspaceFolderItem)} " +
                            $"WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ? AND {nameof(WorkspaceFolderItem.Path)} = ?",
                            id, item.Path.NormalizeDirectory()) != null;
                        if (!exists)
                        {
                            db.Insert(
                                new WorkspaceFolderItem()
                                {
                                    WorkspaceId = id,
                                    Path = item.Path.NormalizeDirectory()
                                });
                        }
                    });
                    break;

                case JumpPointItemType.Drive:
                    await connection.RunInTransactionAsync(db =>
                    {
                        var exists = db.FindWithQuery<WorkspaceDriveItem>($"SELECT * FROM {nameof(WorkspaceDriveItem)} " +
                            $"WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ? AND {nameof(WorkspaceDriveItem.Path)} = ?",
                            id, item.Path.NormalizeDirectory()) != null;
                        if (!exists)
                        {
                            db.Insert(
                                new WorkspaceDriveItem()
                                {
                                    WorkspaceId = id,
                                    Path = item.Path.NormalizeDirectory()
                                });
                        }
                    });
                    break;

                case JumpPointItemType.SettingLink when item is SettingLink setting:
                    await connection.RunInTransactionAsync(db =>
                    {
                        var exists = db.FindWithQuery<WorkspaceSettingItem>($"SELECT * FROM {nameof(WorkspaceSettingItem)} " +
                            $"WHERE {nameof(WorkspaceSettingItem.WorkspaceId)} = ? AND {nameof(WorkspaceSettingItem.Setting)} = ?",
                            id, setting.Template) != null;
                        if (!exists)
                        {
                            db.Insert(
                                new WorkspaceSettingItem()
                                {
                                    WorkspaceId = id,
                                    Setting = setting.Template
                                });
                        }
                    });
                    break;

                case JumpPointItemType.AppLink:
                    await connection.RunInTransactionAsync(db =>
                    {
                        var exists = db.FindWithQuery<WorkspaceAppLinkItem>($"SELECT * FROM {nameof(WorkspaceAppLinkItem)} " +
                            $"WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ? AND {nameof(WorkspaceAppLinkItem.Path)} = ?",
                            id, item.Path) != null;
                        if (!exists)
                        {
                            db.Insert(
                                new WorkspaceAppLinkItem()
                                {
                                    WorkspaceId = id,
                                    Path = item.Path
                                });
                        }
                    });
                    break;

                case JumpPointItemType.Workspace:
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

        public static async Task RemoveItem(int id, JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                    await connection.ExecuteAsync($"DELETE FROM {nameof(WorkspaceFileItem)} " +
                        $"WHERE {nameof(WorkspaceFileItem.WorkspaceId)} = ? AND {nameof(WorkspaceFileItem.Path)} = ?",
                        id, item.Path);
                    break;

                case JumpPointItemType.Folder:
                    await connection.ExecuteAsync($"DELETE FROM {nameof(WorkspaceFolderItem)} " +
                        $"WHERE {nameof(WorkspaceFolderItem.WorkspaceId)} = ? AND {nameof(WorkspaceFolderItem.Path)} = ?",
                        id, item.Path.NormalizeDirectory());
                    break;

                case JumpPointItemType.Drive:
                    await connection.ExecuteAsync($"DELETE FROM {nameof(WorkspaceDriveItem)} " +
                        $"WHERE {nameof(WorkspaceDriveItem.WorkspaceId)} = ? AND {nameof(WorkspaceDriveItem.Path)} = ?",
                        id, item.Path.NormalizeDirectory());
                    break;

                case JumpPointItemType.SettingLink when item is SettingLink setting:
                    await connection.ExecuteAsync($"DELETE FROM {nameof(WorkspaceSettingItem)} " +
                        $"WHERE {nameof(WorkspaceSettingItem.WorkspaceId)} = ? AND {nameof(WorkspaceSettingItem.Setting)} = ?",
                        id, setting.Template);
                    break;

                case JumpPointItemType.AppLink:
                    await connection.ExecuteAsync($"DELETE FROM {nameof(WorkspaceAppLinkItem)} " +
                        $"WHERE {nameof(WorkspaceAppLinkItem.WorkspaceId)} = ? AND {nameof(WorkspaceAppLinkItem.Path)} = ?",
                        id, item.Path);
                    break;

                case JumpPointItemType.Workspace:
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

    }
}
