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
        private static readonly Lazy<ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo>> settingLinks;

        static SettingLinkService()
        {
            settingLinks = new Lazy<ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo>>(PlatformGetSettingLinkInfos);
        }

        public static IEnumerable<SettingLinkTemplate> GetSettingLinkTemplates()
               => PlatformGetSettingLinkTemplates();

        public static async Task<ReadOnlyCollection<SettingLink>> GetSettingLinks()
            => await PlatformGetSettingLinks();

        public static SettingLink GetSettingLink(SettingLinkTemplate template)
            => PlatformGetSettingLink(template);

        public static async Task Open(SettingLinkTemplate template)
            => await PlatformOpen(template);

        public static async Task Load(SettingLink settingLink)
        {
            settingLink.IsFavorite = await DashboardService.GetStatus(settingLink);
        }

        private class SettingLinkInfo
        {
            public SettingLinkInfo(SettingLinkGroup group, string path)
            {
                Group = group;
                Path = path;
            }

            public SettingLinkGroup Group { get; }

            public string Path { get; }
        }

    }
}
