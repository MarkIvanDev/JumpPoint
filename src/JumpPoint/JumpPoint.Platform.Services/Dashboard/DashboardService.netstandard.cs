using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Services
{
    public static partial class DashboardService
    {

        static Task PlatformInitialize()
            => throw new NotImplementedException();

        #region User Folders

        static ConcurrentDictionary<UserFolderTemplate, bool> PlatformGetUserFolderDefaults()
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<FolderBase>> PlatformGetUserFolders()
            => throw new NotImplementedException();

        static bool PlatformGetStatus(UserFolderTemplate userFolder)
            => throw new NotImplementedException();

        static void PlatformSetStatus(UserFolderTemplate userFolder, bool status)
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<UserFolderSetting>> PlatformGetUserFolderSettings()
            => throw new NotImplementedException();

        #endregion

        #region System Folders

        static ConcurrentDictionary<SystemFolderTemplate, bool> PlatformGetSystemFolderDefaults()
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<FolderBase>> PlatformGetSystemFolders()
            => throw new NotImplementedException();

        static bool PlatformGetStatus(SystemFolderTemplate systemFolder)
            => throw new NotImplementedException();

        static void PlatformSetStatus(SystemFolderTemplate systemFolder, bool status)
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<SystemFolderSetting>> PlatformGetSystemFolderSettings()
            => throw new NotImplementedException();

        #endregion

    }
}
