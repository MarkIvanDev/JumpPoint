using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.ViewModels.Commands
{
    public static class NavigationBar
    {
        private static object _lock = new object();

        public static bool IsUpEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null) return false;
                switch (context.PathInfo.Type)
                {
                    case AppPath.Unknown:
                    case AppPath.Dashboard:
                        return false;
                    default:
                        return true;
                } 
            }
        }

        public static bool IsOpenInFileExplorerEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item is DirectoryBase && !context.Item.Path.StartsWith(@"\\?\"); 
            }
        }

        public static bool IsOpenInCommandPromptEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item is DirectoryBase && !context.Item.Path.StartsWith(@"\\?\"); 
            }
        }

        public static bool IsOpenInPowershellEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item is DirectoryBase && !context.Item.Path.StartsWith(@"\\?\"); 
            }
        }

        public static bool IsAddToWorkspaceEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                switch (context.Item.Type)
                {
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Drive:
                    case JumpPointItemType.Workspace:
                        return true;

                    case JumpPointItemType.Unknown:
                    case JumpPointItemType.File:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.AppLink:
                    default:
                        return false;
                } 
            }
        }

        public static bool IsAddToFavoritesEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return !context.Item.IsFavorite; 
            }
        }

        public static bool IsRemoveFromFavoritesEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item.IsFavorite; 
            }
        }

        public static bool IsPropertiesEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null) return false;
                switch (context.PathInfo.Type)
                {
                    case AppPath.Folder:
                    case AppPath.Drive:
                    case AppPath.Workspace:
                        return true;
                    case AppPath.AppLinks:
                    case AppPath.Properties:
                    case AppPath.Favorites:
                    case AppPath.Drives:
                    case AppPath.Workspaces:
                    case AppPath.Dashboard:
                    case AppPath.Settings:
                    case AppPath.Unknown:
                    default:
                        return false;
                } 
            }
        }

        public static bool IsSetWorkspaceTemplateEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item.Type == JumpPointItemType.Workspace; 
            }
        }

        public static bool IsSetFolderTemplateEnabled(ShellContextViewModelBase context)
        {
            lock (_lock)
            {
                if (context is null || context.Item is null) return false;
                return context.Item is FolderBase folder && folder.FolderType == FolderType.Regular; 
            }
        }

        public static bool IsNewFileEnabled(ShellContextViewModelBase context)
        {
            return context != null &&
                    (context.PathInfo.Type == AppPath.Drive || context.PathInfo.Type == AppPath.Folder);
        }

        public static bool IsNewFolderEnabled(ShellContextViewModelBase context)
        {
            return context != null &&
                    (context.PathInfo.Type == AppPath.Drive || context.PathInfo.Type == AppPath.Folder);
        }

        public static bool IsMoreNewItemsEnabled(ShellContextViewModelBase context)
        {
            return context != null &&
                    (context.PathInfo.Type == AppPath.Drive || context.PathInfo.Type == AppPath.Folder);
        }
    }
}
