using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;
using Windows.Storage;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services
{
    public static partial class DashboardService
    {
        private const string USER_FOLDERS = "UserFolders";
        private const string SYSTEM_FOLDERS = "SystemFolders";

        static async Task PlatformInitialize()
        {
            if (ApplicationData.Current.Version < 1)
            {
                await ApplicationData.Current.SetVersionAsync(1, (request) =>
                {
                    var deferral = request.GetDeferral();
                    foreach (var item in FolderTemplateService.GetUserFolderTemplates())
                    {
                        var previousKey = item.Humanize();
                        if (Preferences.ContainsKey(previousKey, USER_FOLDERS))
                        {
                            var value = Preferences.Get(previousKey, PlatformGetDefault(item), USER_FOLDERS);
                            Preferences.Set(item.ToString(), value, USER_FOLDERS);
                            Preferences.Remove(previousKey, USER_FOLDERS);
                        }
                    }

                    foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
                    {
                        var previousKey = item.Humanize();
                        if (Preferences.ContainsKey(previousKey, SYSTEM_FOLDERS))
                        {
                            var value = Preferences.Get(previousKey, PlatformGetDefault(item), SYSTEM_FOLDERS);
                            Preferences.Set(item.ToString(), value, SYSTEM_FOLDERS);
                            Preferences.Remove(previousKey, SYSTEM_FOLDERS);
                        }
                    }

                    if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("SettingItems"))
                    {
                        ApplicationData.Current.LocalSettings.DeleteContainer("SettingItems");
                    }

                    deferral.Complete();
                });
            }
            
        }

        #region User Folders

        static ConcurrentDictionary<UserFolderTemplate, bool> PlatformGetUserFolderDefaults()
        {
            var userFolders = new ConcurrentDictionary<UserFolderTemplate, bool>();
            userFolders.TryAdd(UserFolderTemplate.User, true);
            userFolders.TryAdd(UserFolderTemplate.Objects3D, true);
            userFolders.TryAdd(UserFolderTemplate.CameraRoll, true);
            userFolders.TryAdd(UserFolderTemplate.Desktop, true);
            userFolders.TryAdd(UserFolderTemplate.Documents, true);
            userFolders.TryAdd(UserFolderTemplate.Downloads, true);
            userFolders.TryAdd(UserFolderTemplate.Favorites, true);
            userFolders.TryAdd(UserFolderTemplate.Music, true);
            userFolders.TryAdd(UserFolderTemplate.OneDrive, true);
            userFolders.TryAdd(UserFolderTemplate.Pictures, true);
            userFolders.TryAdd(UserFolderTemplate.Playlists, true);
            userFolders.TryAdd(UserFolderTemplate.SavedPictures, true);
            userFolders.TryAdd(UserFolderTemplate.Screenshots, true);
            userFolders.TryAdd(UserFolderTemplate.Videos, true);

            userFolders.TryAdd(UserFolderTemplate.LocalAppData, false);
            userFolders.TryAdd(UserFolderTemplate.LocalAppDataLow, false);
            userFolders.TryAdd(UserFolderTemplate.RoamingAppData, false);

            userFolders.TryAdd(UserFolderTemplate.Cookies, false);
            userFolders.TryAdd(UserFolderTemplate.History, false);
            userFolders.TryAdd(UserFolderTemplate.InternetCache, false);
            userFolders.TryAdd(UserFolderTemplate.Recent, false);
            userFolders.TryAdd(UserFolderTemplate.Templates, false);

            userFolders.TryAdd(UserFolderTemplate.Contacts, false);
            userFolders.TryAdd(UserFolderTemplate.Links, false);
            userFolders.TryAdd(UserFolderTemplate.SavedGames, false);
            userFolders.TryAdd(UserFolderTemplate.Searches, false);

            return userFolders;
        }

        static async Task<IList<FolderBase>> PlatformGetUserFolders(bool includeAll)
        {
            var folders = new List<FolderBase>();
            foreach (var item in FolderTemplateService.GetUserFolderTemplates())
            {
                if (includeAll || PlatformGetStatus(item))
                {
                    var folder = await StorageService.GetUserFolder(item);
                    if (!(folder is null))
                    {
                        folder.DashboardGroup = DashboardGroup.UserFolders;
                        folders.Add(folder);
                    }
                }
            }
            return folders;
        }

        static bool PlatformGetDefault(UserFolderTemplate userFolder)
        {
            return userFolders.Value.TryGetValue(userFolder, out var setting) ? setting : false;
        }

        static bool PlatformGetStatus(UserFolderTemplate userFolder)
        {
            return Preferences.Get(userFolder.ToString(), PlatformGetDefault(userFolder), USER_FOLDERS);
        }

        static void PlatformSetStatus(UserFolderTemplate userFolder, bool status)
        {
            Preferences.Set(userFolder.ToString(), status, USER_FOLDERS);
        }

        static async Task<IList<UserFolderSetting>> PlatformGetUserFolderSettings()
        {
            return await Task.Run(() =>
            {
                var items = new List<UserFolderSetting>();
                foreach (var item in FolderTemplateService.GetUserFolderTemplates())
                {
                    items.Add(new UserFolderSetting(item));
                }
                return items;
            });
        }

        #endregion

        #region System Folders

        static ConcurrentDictionary<SystemFolderTemplate, bool> PlatformGetSystemFolderDefaults()
        {
            var systemFolders = new ConcurrentDictionary<SystemFolderTemplate, bool>();
            systemFolders.TryAdd(SystemFolderTemplate.Users, true);
            systemFolders.TryAdd(SystemFolderTemplate.Public, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDesktop, false);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDocuments, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDownloads, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicMusic, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicPictures, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicVideos, true);

            systemFolders.TryAdd(SystemFolderTemplate.Fonts, false);
            systemFolders.TryAdd(SystemFolderTemplate.ProgramData, false);

            var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (!string.IsNullOrEmpty(programFiles)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFiles, false);

            var programFilesX86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
            if (!string.IsNullOrEmpty(programFilesX86)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFilesX86, false);

            systemFolders.TryAdd(SystemFolderTemplate.System, false);
            systemFolders.TryAdd(SystemFolderTemplate.Windows, false);

            systemFolders.TryAdd(SystemFolderTemplate.Burn, false);
            return systemFolders;
        }

        static async Task<IList<FolderBase>> PlatformGetSystemFolders(bool includeAll)
        {
            var folders = new List<FolderBase>();
            foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
            {
                if (includeAll || PlatformGetStatus(item))
                {
                    var folder = await StorageService.GetSystemFolder(item);
                    if (!(folder is null))
                    {
                        folder.DashboardGroup = DashboardGroup.SystemFolders;
                        folders.Add(folder);
                    }
                }
            }
            return folders;
        }

        static bool PlatformGetDefault(SystemFolderTemplate systemFolder)
        {
            return systemFolders.Value.TryGetValue(systemFolder, out var setting) ? setting : false;
        }

        static bool PlatformGetStatus(SystemFolderTemplate systemFolder)
        {
            return Preferences.Get(systemFolder.ToString(), PlatformGetDefault(systemFolder), SYSTEM_FOLDERS);
        }

        static void PlatformSetStatus(SystemFolderTemplate systemFolder, bool status)
        {
            Preferences.Set(systemFolder.ToString(), status, SYSTEM_FOLDERS);
        }

        static async Task<IList<SystemFolderSetting>> PlatformGetSystemFolderSettings()
        {
            return await Task.Run(() =>
            {
                var items = new List<SystemFolderSetting>();
                foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
                {
                    items.Add(new SystemFolderSetting(item));
                }
                return items;
            });
        }

        #endregion

    }
}
