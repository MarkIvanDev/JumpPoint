using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Services
{
    public static partial class FolderTemplateService
    {
        static Task<ConcurrentDictionary<UserFolderTemplate, string>> PlatformGetUserFolderPaths()
            => throw new NotImplementedException();

        static ConcurrentDictionary<SystemFolderTemplate, string> PlatformGetSystemFolderPaths()
            => throw new NotImplementedException();
    }
}
