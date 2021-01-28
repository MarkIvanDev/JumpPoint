using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;
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
            await connection.CreateTableAsync<FavoriteWorkspace>();
            await connection.CreateTableAsync<FavoriteDrive>();
            await connection.CreateTableAsync<FavoriteFolder>();
            await connection.CreateTableAsync<FavoriteFile>();
            await connection.CreateTableAsync<FavoriteAppLink>();
            await connection.CreateTableAsync<FavoriteSettingLink>();
            await PlatformInitialize();
        }

        #region Favorites

        public static async Task<ReadOnlyCollection<JumpPointItem>> GetFavorites()
        {
            var items = new List<JumpPointItem>();
            items.AddRange(await GetFavorites(JumpPointItemType.Workspace));
            items.AddRange(await GetFavorites(JumpPointItemType.Drive));
            items.AddRange(await GetFavorites(JumpPointItemType.Folder));
            items.AddRange(await GetFavorites(JumpPointItemType.File));
            items.AddRange(await GetFavorites(JumpPointItemType.AppLink));
            items.AddRange(await GetFavorites(JumpPointItemType.SettingLink));
            return new ReadOnlyCollection<JumpPointItem>(items);
        }

        public static async Task<ReadOnlyCollection<JumpPointItem>> GetFavorites(JumpPointItemType type)
        {
            var items = new List<JumpPointItem>();

            switch (type)
            {
                case JumpPointItemType.File:
                    var faveFiles = await connection.Table<FavoriteFile>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveFiles)
                    {
                        var file = await LocalStorageService.GetFile(item.Path);
                        if (file != null)
                        {
                            items.Add(file);
                        }
                    }
                    break;

                case JumpPointItemType.Folder:
                    var faveFolders = await connection.Table<FavoriteFolder>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveFolders)
                    {
                        var folder = await LocalStorageService.GetFolder(item.Path);
                        if (folder != null)
                        {
                            items.Add(folder);
                        }
                    }
                    break;

                case JumpPointItemType.Drive:
                    var faveDrives = await connection.Table<FavoriteDrive>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveDrives)
                    {
                        var drive = await LocalStorageService.GetDrive(item.Path);
                        if (drive != null)
                        {
                            items.Add(drive);
                        }
                    }
                    break;

                case JumpPointItemType.Workspace:
                    var faveWorkspaces = await connection.Table<FavoriteWorkspace>().OrderBy(f => f.Path).ToListAsync();
                    foreach (var item in faveWorkspaces)
                    {
                        var workspace = await LocalStorageService.GetFile(item.Path);
                        if (workspace != null)
                        {
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
                            items.Add(appLink);
                        }
                    }
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }

            return new ReadOnlyCollection<JumpPointItem>(items);
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
                        item.Path) != null;

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
                        await connection.InsertAsync(new FavoriteWorkspace() { Path = item.Path }, "OR IGNORE") :
                        await connection.ExecuteAsync($"DELETE FROM {nameof(FavoriteWorkspace)} WHERE {nameof(FavoriteWorkspace.Path)} = ?",
                            item.Path);
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

        public static async Task<ReadOnlyCollection<FolderBase>> GetUserFolders()
            => await PlatformGetUserFolders();

        public static bool GetStatus(UserFolderTemplate userFolder)
            => PlatformGetStatus(userFolder);

        public static void SetStatus(UserFolderTemplate userFolder, bool status)
            => PlatformSetStatus(userFolder, status);

        public static async Task<ReadOnlyCollection<UserFolderSetting>> GetUserFolderSettings()
            => await PlatformGetUserFolderSettings();

        #endregion

        #region System Folders

        public static async Task<ReadOnlyCollection<FolderBase>> GetSystemFolders()
            => await PlatformGetSystemFolders();

        public static bool GetStatus(SystemFolderTemplate systemFolder)
            => PlatformGetStatus(systemFolder);

        public static void SetStatus(SystemFolderTemplate systemFolder, bool status)
            => PlatformSetStatus(systemFolder, status);

        public static async Task<ReadOnlyCollection<SystemFolderSetting>> GetSystemFolderSettings()
            => await PlatformGetSystemFolderSettings();

        #endregion
    }
}
