using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class FolderTemplateService
    {

        static ConcurrentDictionary<UserFolderTemplate, string> PlatformGetUserFolderPaths()
        {
            var paths = UserDataPaths.GetDefault();
            var userFolders = new ConcurrentDictionary<UserFolderTemplate, string>();
            try
            {
                userFolders.TryAdd(UserFolderTemplate.User, paths.Profile);
                // 3D Objects added at the last
                userFolders.TryAdd(UserFolderTemplate.CameraRoll, paths.CameraRoll);
                userFolders.TryAdd(UserFolderTemplate.Desktop, paths.Desktop);
                userFolders.TryAdd(UserFolderTemplate.Documents, paths.Documents);
                userFolders.TryAdd(UserFolderTemplate.Downloads, paths.Downloads);
                userFolders.TryAdd(UserFolderTemplate.Favorites, paths.Favorites);
                userFolders.TryAdd(UserFolderTemplate.Music, paths.Music);
                string oneDrive = Environment.GetEnvironmentVariable("OneDrive");
                if (!string.IsNullOrEmpty(oneDrive)) userFolders.TryAdd(UserFolderTemplate.OneDrive, oneDrive);
                userFolders.TryAdd(UserFolderTemplate.Pictures, paths.Pictures);
                // Playlists added at the last
                userFolders.TryAdd(UserFolderTemplate.SavedPictures, paths.SavedPictures);
                userFolders.TryAdd(UserFolderTemplate.Screenshots, paths.Screenshots);
                userFolders.TryAdd(UserFolderTemplate.Videos, paths.Videos);

                userFolders.TryAdd(UserFolderTemplate.LocalAppData, paths.LocalAppData);
                userFolders.TryAdd(UserFolderTemplate.LocalAppDataLow, paths.LocalAppDataLow);
                userFolders.TryAdd(UserFolderTemplate.RoamingAppData, paths.RoamingAppData);

                userFolders.TryAdd(UserFolderTemplate.Cookies, paths.Cookies);
                userFolders.TryAdd(UserFolderTemplate.History, paths.History);
                userFolders.TryAdd(UserFolderTemplate.InternetCache, paths.InternetCache);
                userFolders.TryAdd(UserFolderTemplate.Recent, paths.Recent);
                userFolders.TryAdd(UserFolderTemplate.Templates, paths.Templates);

                userFolders.TryAdd(UserFolderTemplate.Contacts, Path.Combine(paths.Profile, UserFolderTemplate.Contacts.Humanize()));
                userFolders.TryAdd(UserFolderTemplate.Links, Path.Combine(paths.Profile, UserFolderTemplate.Links.Humanize()));
                userFolders.TryAdd(UserFolderTemplate.SavedGames, Path.Combine(paths.Profile, UserFolderTemplate.SavedGames.Humanize()));
                userFolders.TryAdd(UserFolderTemplate.Searches, Path.Combine(paths.Profile, UserFolderTemplate.Searches.Humanize()));

                userFolders.TryAdd(UserFolderTemplate.Objects3D, KnownFolders.Objects3D.Path);
                userFolders.TryAdd(UserFolderTemplate.Playlists, KnownFolders.Playlists.Path);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
            return userFolders;
        }

        static ConcurrentDictionary<SystemFolderTemplate, string> PlatformGetSystemFolderPaths()
        {
            var paths = SystemDataPaths.GetDefault();
            var systemFolders = new ConcurrentDictionary<SystemFolderTemplate, string>();
            try
            {
                systemFolders.TryAdd(SystemFolderTemplate.Users, paths.UserProfiles);
                systemFolders.TryAdd(SystemFolderTemplate.Public, paths.Public);
                systemFolders.TryAdd(SystemFolderTemplate.PublicDesktop, paths.PublicDesktop);
                systemFolders.TryAdd(SystemFolderTemplate.PublicDocuments, paths.PublicDocuments);
                systemFolders.TryAdd(SystemFolderTemplate.PublicDownloads, paths.PublicDownloads);
                systemFolders.TryAdd(SystemFolderTemplate.PublicMusic, paths.PublicMusic);
                systemFolders.TryAdd(SystemFolderTemplate.PublicPictures, paths.PublicPictures);
                systemFolders.TryAdd(SystemFolderTemplate.PublicVideos, paths.PublicVideos);

                systemFolders.TryAdd(SystemFolderTemplate.Fonts, paths.Fonts);
                systemFolders.TryAdd(SystemFolderTemplate.ProgramData, paths.ProgramData);

                var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
                if (!string.IsNullOrEmpty(programFiles)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFiles, programFiles);

                var programFilesX86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
                if (!string.IsNullOrEmpty(programFilesX86)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFilesX86, programFilesX86);

                systemFolders.TryAdd(SystemFolderTemplate.System, paths.System);
                systemFolders.TryAdd(SystemFolderTemplate.Windows, paths.Windows);

                systemFolders.TryAdd(SystemFolderTemplate.Burn, Environment.GetFolderPath(Environment.SpecialFolder.CDBurning));
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
            return systemFolders;
        }

    }
}
