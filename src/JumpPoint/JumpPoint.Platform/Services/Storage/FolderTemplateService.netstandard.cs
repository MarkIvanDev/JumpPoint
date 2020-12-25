using System;
using System.Collections.Concurrent;
using System.Text;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Services
{
    public static partial class FolderTemplateService
    {
        static ConcurrentDictionary<UserFolderTemplate, string> PlatformGetUserFolderPaths()
            => throw new NotImplementedException();

        static ConcurrentDictionary<SystemFolderTemplate, string> PlatformGetSystemFolderPaths()
            => throw new NotImplementedException();
    }
}
