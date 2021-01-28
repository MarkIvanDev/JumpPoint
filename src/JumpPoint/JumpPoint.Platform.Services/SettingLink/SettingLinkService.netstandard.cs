using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Services
{
    public static partial class SettingLinkService
    {

        static ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo> PlatformGetSettingLinkInfos()
            => throw new NotImplementedException();

        static IEnumerable<SettingLinkTemplate> PlatformGetSettingLinkTemplates()
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<SettingLink>> PlatformGetSettingLinks()
            => throw new NotImplementedException();

        static SettingLink PlatformGetSettingLink(SettingLinkTemplate template)
            => throw new NotImplementedException();

        static Task PlatformOpen(SettingLinkTemplate template)
            => throw new NotImplementedException();

    }
}
