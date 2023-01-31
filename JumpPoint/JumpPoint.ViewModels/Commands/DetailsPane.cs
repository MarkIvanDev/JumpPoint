using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;

namespace JumpPoint.ViewModels.Commands
{
    public static class DetailsPane
    {
        public static bool IsOpenWithEnabled(JumpPointItem item)
        {
            return item is FileBase file && file.StorageType != StorageType.Cloud;
        }

        public static bool IsOpenInFileExplorerEnabled(JumpPointItem item)
        {
            var pathKind = item.Path.GetPathKind();
            return item is StorageItemBase && pathKind != PathKind.Unmounted && pathKind != PathKind.Cloud;
        }

        public static bool IsOpenInCommandPromptEnabled(JumpPointItem item) 
        {
            var pathKind = item.Path.GetPathKind();
            return item is DirectoryBase && pathKind != PathKind.Unmounted && pathKind != PathKind.Cloud;
        }

        public static bool IsOpenInPowershellEnabled(JumpPointItem item)
        {
            var pathKind = item.Path.GetPathKind();
            return item is DirectoryBase && pathKind != PathKind.Unmounted && pathKind != PathKind.Cloud;
        }

        public static bool IsOpenInWindowsTerminalEnabled(JumpPointItem item)
        {
            var pathKind = item.Path.GetPathKind();
            return item is DirectoryBase && pathKind != PathKind.Unmounted && pathKind != PathKind.Cloud;
        }

        public static bool IsStorageSenseEnabled(JumpPointItem item)
        {
            var pathKind = item.Path.GetPathKind();
            return pathKind == PathKind.Mounted;
        }

        public static bool IsCleanManagerEnabled(JumpPointItem item)
        {
            var pathKind = item.Path.GetPathKind();
            return pathKind == PathKind.Mounted;
        }

        public static bool IsAddToWorkspaceEnabled(JumpPointItem item)
        {
            return item is StorageItemBase || item is AppLink;
        }

        public static bool IsAddToFavoritesEnabled(JumpPointItem item)
        {
            return !item.IsFavorite;
        }

        public static bool IsRemoveFromFavoritesEnabled(JumpPointItem item)
        {
            return item.IsFavorite;
        }

        public static bool IsSetFolderTemplateEnabled(JumpPointItem item)
        {
            return item is FolderBase folder && folder.FolderType == FolderType.Regular;
        }

        public static bool IsSetWorkspaceTemplateEnabled(JumpPointItem item)
        {
            return item.Type == JumpPointItemType.Workspace;
        }

    }
}
