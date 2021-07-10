using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Models.Favorite;
using NittyGritty.Extensions;
using SQLite;

namespace JumpPoint.Platform.Services
{
    public static partial class DashboardService
    {
        private const string FAVORITE_DATAFILE = "favorites.db";
        private static readonly SQLiteAsyncConnection connection;
        private static readonly Lazy<ConcurrentDictionary<UserFolderTemplate, bool>> userFolders;
        private static readonly Lazy<ConcurrentDictionary<SystemFolderTemplate, bool>> systemFolders;

        static DashboardService()
        {
            connection = new SQLiteAsyncConnection(Path.Combine(JumpPointService.DataFolder, FAVORITE_DATAFILE));
            userFolders = new Lazy<ConcurrentDictionary<UserFolderTemplate, bool>>(PlatformGetUserFolderDefaults);
            systemFolders = new Lazy<ConcurrentDictionary<SystemFolderTemplate, bool>>(PlatformGetSystemFolderDefaults);
        }

        public static async Task Initialize()
        {
            await connection.RunInTransactionAsync(db =>
            {
                db.CreateTable<FavoriteWorkspace>();
                var workspaces = db.Table<FavoriteWorkspace>().ToList().Where(w => w.Path.GetPathKind() != PathKind.Workspace);
                foreach (var item in workspaces)
                {
                    item.Path = PathExtensions.GetWorkspacePath(item.Path);
                    _ = db.FindWithQuery<FavoriteWorkspace>($"SELECT * FROM {nameof(FavoriteWorkspace)} WHERE {nameof(FavoriteWorkspace.Path)} = ?", item.Path) != null ?
                        db.Delete(item) : db.Update(item);
                }

                db.CreateTable<FavoriteDrive>();
                db.CreateTable<FavoriteFolder>();
                db.CreateTable<FavoriteFile>();

                db.CreateTable<FavoriteAppLink>();
                var applinks = db.Table<FavoriteAppLink>().ToList().Where(a => a.Path.GetPathKind() != PathKind.AppLink);
                db.Execute("ATTACH DATABASE ? AS ?", AppLinkService.DataFilePath, nameof(AppLink));
                foreach (var item in applinks)
                {
                    var ali = db.FindWithQuery<AppLinkInfo>($"SELECT * FROM {nameof(AppLink)}.{nameof(AppLinkInfo)} " +
                        $"WHERE {nameof(AppLinkInfo)}.{nameof(AppLinkInfo.Link)} = ?", item.Path);
                    if (ali != null)
                    {
                        item.Path = PathExtensions.GetAppLinkPath(ali.Name);
                        _ = db.FindWithQuery<FavoriteAppLink>($"SELECT * FROM {nameof(FavoriteAppLink)} WHERE {nameof(FavoriteAppLink.Path)} = ?", item.Path) != null ?
                            db.Delete(item) : db.Update(item);
                    }
                    else
                    {
                        db.Delete(item);
                    }
                }
                db.Execute("DETACH DATABASE ?", nameof(AppLink));

                db.CreateTable<FavoriteSettingLink>();
            });
            await PlatformInitialize();
        }

        #region Favorites

        public static async Task<IList<JumpPointItem>> GetFavorites()
        {
            var items = new List<JumpPointItem>();
            items.AddRange(await GetFavorites(JumpPointItemType.Workspace));
            items.AddRange(await GetFavorites(JumpPointItemType.Drive));
            items.AddRange(await GetFavorites(JumpPointItemType.Folder));
            items.AddRange(await GetFavorites(JumpPointItemType.File));
            items.AddRange(await GetFavorites(JumpPointItemType.AppLink));
            items.AddRange(await GetFavorites(JumpPointItemType.SettingLink));
            return items;
        }

        public static async Task<IList<JumpPointItem>> GetFavorites(JumpPointItemType type)
        {
            var items = new List<JumpPointItem>();

            switch (type)
            {
                case JumpPointItemType.File:
                    var faveFiles = await connection.Table<FavoriteFile>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveFiles)
                    {
                        var file = await StorageService.GetFile(item.Path);
                        if (file != null)
                        {
                            file.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(file);
                        }
                    }
                    break;

                case JumpPointItemType.Folder:
                    var faveFolders = await connection.Table<FavoriteFolder>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveFolders)
                    {
                        var folder = await StorageService.GetFolder(item.Path);
                        if (folder != null)
                        {
                            folder.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(folder);
                        }
                    }
                    break;

                case JumpPointItemType.Drive:
                    var faveDrives = await connection.Table<FavoriteDrive>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveDrives)
                    {
                        var drive = await StorageService.GetDrive(item.Path);
                        if (drive != null)
                        {
                            drive.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(drive);
                        }
                    }
                    break;

                case JumpPointItemType.Workspace:
                    var faveWorkspaces = await connection.Table<FavoriteWorkspace>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveWorkspaces)
                    {
                        var workspace = await WorkspaceService.GetWorkspace(item.Path);
                        if (workspace != null)
                        {
                            workspace.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(workspace);
                        }
                    }
                    break;

                case JumpPointItemType.SettingLink:
                    var faveSettingLinks = await connection.Table<FavoriteSettingLink>().OrderBy(f => f.Template).ToListAsync();
                    foreach (var item in faveSettingLinks)
                    {
                        var settingLink = SettingLinkService.GetSettingLink(item.Template);
                        if (settingLink != null)
                        {
                            settingLink.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(settingLink);
                        }
                    }
                    break;

                case JumpPointItemType.AppLink:
                    var faveAppLinks = await connection.Table<FavoriteAppLink>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveAppLinks)
                    {
                        var appLink = await AppLinkService.GetAppLink(item.Path);
                        if (appLink != null)
                        {
                            appLink.DashboardGroup = DashboardGroup.Favorites;
                            items.Add(appLink);
                        }
                    }
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }

            return items;
        }

        public static async Task<bool> GetStatus(JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                    return await connection.FindWithQueryAsync<FavoriteFile>(
                        $"SELECT * FROM {nameof(FavoriteFile)} WHERE {nameof(FavoriteFile.Path)} = ?",
                        item.Path) != null;

                case JumpPointItemType.Folder:
                    return await connection.FindWithQueryAsync<FavoriteFolder>(
                        $"SELECT * FROM {nameof(FavoriteFolder)} WHERE {nameof(FavoriteFolder.Path)} = ?",
                        item.Path.NormalizeDirectory()) != null;

                case JumpPointItemType.Drive:
                    return await connection.FindWithQueryAsync<FavoriteDrive>(
                        $"SELECT * FROM {nameof(FavoriteDrive)} WHERE {nameof(FavoriteDrive.Path)} = ?",
                        item.Path.NormalizeDirectory()) != null;

                case JumpPointItemType.Workspace:
                    return await connection.FindWithQueryAsync<FavoriteWorkspace>(
                        $"SELECT * FROM {nameof(FavoriteWorkspace)} WHERE {nameof(FavoriteWorkspace.Path)} = ?",
                        item.Name) != null;

                case JumpPointItemType.SettingLink when item is SettingLink settingLink:
                    return await connection.FindWithQueryAsync<FavoriteSettingLink>(
                        $"SELECT * FROM {nameof(FavoriteSettingLink)} WHERE {nameof(FavoriteSettingLink.Template)} = ?",
                        settingLink.Template) != null;

                case JumpPointItemType.AppLink:
                    return await connection.FindWithQueryAsync<FavoriteAppLink>(
                        $"SELECT * FROM {nameof(FavoriteAppLink)} WHERE {nameof(FavoriteAppLink.Path)} = ?",
                        item.Path) != null;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return false;
            }
        }

        public static async Task SetStatus(JumpPointItem item, bool status)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteFile() { Path = item.Path }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteFile)} WHERE {nameof(FavoriteFile.Path)} = ?",
                            item.Path);
                    break;

                case JumpPointItemType.Folder:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteFolder() { Path = item.Path.NormalizeDirectory() }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteFolder)} WHERE {nameof(FavoriteFolder.Path)} = ?",
                            item.Path.NormalizeDirectory());
                    break;

                case JumpPointItemType.Drive:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteDrive() { Path = item.Path.NormalizeDirectory() }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteDrive)} WHERE {nameof(FavoriteDrive.Path)} = ?",
                            item.Path.NormalizeDirectory());
                    break;

                case JumpPointItemType.Workspace:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteWorkspace() { Path = item.Name }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteWorkspace)} WHERE {nameof(FavoriteWorkspace.Path)} = ?",
                            item.Name);
                    break;

                case JumpPointItemType.SettingLink when item is SettingLink settingLink:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteSettingLink() { Template = settingLink.Template }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteSettingLink)} WHERE {nameof(FavoriteSettingLink.Template)} = ?",
                            settingLink.Template);
                    break;

                case JumpPointItemType.AppLink:
                    _ = status ?
                        await connection.InsertAsync(new FavoriteAppLink() { Path = item.Path }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteAppLink)} WHERE {nameof(FavoriteAppLink.Path)} = ?",
                            item.Path);
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

        #endregion

        #region User Folders

        public static async Task<IList<FolderBase>> GetUserFolders(bool includeAll)
            => await PlatformGetUserFolders(includeAll);

        public static bool GetStatus(UserFolderTemplate userFolder)
            => PlatformGetStatus(userFolder);

        public static void SetStatus(UserFolderTemplate userFolder, bool status)
            => PlatformSetStatus(userFolder, status);

        public static async Task<IList<UserFolderSetting>> GetUserFolderSettings()
            => await PlatformGetUserFolderSettings();

        #endregion

        #region System Folders

        public static async Task<IList<FolderBase>> GetSystemFolders(bool includeAll)
            => await PlatformGetSystemFolders(includeAll);

        public static bool GetStatus(SystemFolderTemplate systemFolder)
            => PlatformGetStatus(systemFolder);

        public static void SetStatus(SystemFolderTemplate systemFolder, bool status)
            => PlatformSetStatus(systemFolder, status);

        public static async Task<IList<SystemFolderSetting>> GetSystemFolderSettings()
            => await PlatformGetSystemFolderSettings();

        #endregion
    }
}
