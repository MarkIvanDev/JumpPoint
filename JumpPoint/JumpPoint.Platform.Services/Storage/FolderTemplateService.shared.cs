using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Extensions;
using SQLite;

namespace JumpPoint.Platform.Services
{
    public static partial class FolderTemplateService
    {
        private const string FOLDER_DATAFILE = "folders.db";
        private static readonly SQLiteAsyncConnection connection;
        private static ConcurrentDictionary<UserFolderTemplate, string> userFolders;
        private static ConcurrentDictionary<SystemFolderTemplate, string> systemFolders;

        static FolderTemplateService()
        {
            connection = new SQLiteAsyncConnection(new SQLiteConnectionString(
                Path.Combine(JumpPointService.DataFolder, FOLDER_DATAFILE),
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                true));
        }

        public static async Task Initialize()
        {
            await connection.CreateTableAsync<FolderInfo>();
            userFolders = await PlatformGetUserFolderPaths();
            systemFolders = PlatformGetSystemFolderPaths();
        }

        public static ReadOnlyCollection<FolderTemplate> GetFolderTemplates()
        {
            var templates = (FolderTemplate[])Enum.GetValues(typeof(FolderTemplate));
            return new ReadOnlyCollection<FolderTemplate>(templates);
        }

        public static async Task<FolderTemplate> GetFolderTemplate(FolderBase folder)
        {
            try
            {
                var uft = GetUserFolderTemplate(folder.Path);
                if (uft != UserFolderTemplate.None)
                {
                    folder.FolderType = FolderType.User;
                    return (FolderTemplate)uft;
                }
                else
                {
                    var sft = GetSystemFolderTemplate(folder.Path);
                    if (sft != SystemFolderTemplate.None)
                    {
                        folder.FolderType = FolderType.System;
                        return (FolderTemplate)sft;
                    }
                    else
                    {
                        folder.FolderType = FolderType.Regular;
                        var info = await connection.FindWithQueryAsync<FolderInfo>(
                            $"SELECT * FROM {nameof(FolderInfo)} WHERE {nameof(FolderInfo.Path)} = ?", folder.Path.NormalizeDirPath());
                        return info?.Template ?? FolderTemplate.General;
                    }
                }
            }
            catch (Exception)
            {
                return FolderTemplate.General;
            }
        }

        public static async Task SetFolderTemplate(FolderBase folder, FolderTemplate template)
        {
            await connection.RunInTransactionAsync((db) =>
            {
                var f = db.FindWithQuery<FolderInfo>(
                    $"SELECT * FROM {nameof(FolderInfo)} WHERE {nameof(FolderInfo.Path)} = ?", folder.Path.NormalizeDirPath());
                if (template == FolderTemplate.General)
                {
                    if (f != null)
                    {
                        db.Delete(f);
                    }
                }
                else
                {
                    if (f != null)
                    {
                        f.Template = template;
                        db.Update(f);
                    }
                    else
                    {
                        db.Insert(new FolderInfo() { Path = folder.Path.NormalizeDirPath(), Template = template });
                    }
                }
            });
        }

        public static IEnumerable<UserFolderTemplate> GetUserFolderTemplates()
        {
            yield return UserFolderTemplate.User;
            yield return UserFolderTemplate.Objects3D;
            yield return UserFolderTemplate.CameraRoll;
            yield return UserFolderTemplate.Desktop;
            yield return UserFolderTemplate.Documents;
            yield return UserFolderTemplate.Downloads;
            yield return UserFolderTemplate.Favorites;
            yield return UserFolderTemplate.Music;
            if (userFolders.ContainsKey(UserFolderTemplate.OneDrive)) yield return UserFolderTemplate.OneDrive;
            yield return UserFolderTemplate.Pictures;
            yield return UserFolderTemplate.Playlists;
            yield return UserFolderTemplate.SavedPictures;
            if (userFolders.ContainsKey(UserFolderTemplate.Screenshots))  yield return UserFolderTemplate.Screenshots;
            yield return UserFolderTemplate.Videos;

            yield return UserFolderTemplate.LocalAppData;
            yield return UserFolderTemplate.LocalAppDataLow;
            yield return UserFolderTemplate.RoamingAppData;

            //yield return UserFolderTemplate.Cookies;
            yield return UserFolderTemplate.History;
            //yield return UserFolderTemplate.InternetCache;
            yield return UserFolderTemplate.Recent;
            yield return UserFolderTemplate.Templates;

            yield return UserFolderTemplate.Contacts;
            yield return UserFolderTemplate.Links;
            yield return UserFolderTemplate.SavedGames;
            yield return UserFolderTemplate.Searches;
        }

        public static bool TryGetPath(UserFolderTemplate template, out string path)
        {
            return userFolders.TryGetValue(template, out path);
        }

        public static UserFolderTemplate GetUserFolderTemplate(string path)
        {
            foreach (var item in GetUserFolderTemplates())
            {
                if (userFolders.TryGetValue(item, out var userFolderPath) &&
                    path.NormalizeDirPath().Equals(userFolderPath.NormalizeDirPath(), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return UserFolderTemplate.None;
        }

        public static IEnumerable<SystemFolderTemplate> GetSystemFolderTemplates()
        {
            yield return SystemFolderTemplate.Users;
            yield return SystemFolderTemplate.Public;
            //yield return SystemFolderTemplate.PublicDesktop;
            yield return SystemFolderTemplate.PublicDocuments;
            yield return SystemFolderTemplate.PublicDownloads;
            yield return SystemFolderTemplate.PublicMusic;
            yield return SystemFolderTemplate.PublicPictures;
            yield return SystemFolderTemplate.PublicVideos;

            yield return SystemFolderTemplate.Fonts;
            //yield return SystemFolderTemplate.ProgramData;
            if (systemFolders.ContainsKey(SystemFolderTemplate.ProgramFiles)) yield return SystemFolderTemplate.ProgramFiles;
            if (systemFolders.ContainsKey(SystemFolderTemplate.ProgramFilesX86)) yield return SystemFolderTemplate.ProgramFilesX86;

            yield return SystemFolderTemplate.System;
            yield return SystemFolderTemplate.Windows;

            //yield return SystemFolderTemplate.Burn;
        }

        public static bool TryGetPath(SystemFolderTemplate template, out string path)
        {
            return systemFolders.TryGetValue(template, out path);
        }

        public static SystemFolderTemplate GetSystemFolderTemplate(string path)
        {
            foreach (var item in GetSystemFolderTemplates())
            {
                if (systemFolders.TryGetValue(item, out var systemFolderPath) &&
                    path.NormalizeDirPath().Equals(systemFolderPath.NormalizeDirPath(), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return SystemFolderTemplate.None;
        }

    }
}
