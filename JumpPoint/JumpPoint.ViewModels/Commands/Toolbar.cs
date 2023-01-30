using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Models;

namespace JumpPoint.ViewModels.Commands
{
    public static class Toolbar
    {
        public static bool IsCopyEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.Dashboard:
                    return context.SelectedItems.Count > 0 &&
                           context.SelectedItems.All(i =>
                           {
                               switch (i.Type)
                               {
                                   case JumpPointItemType.File when ((FileBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Folder when ((FolderBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Drive when ((DriveBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Workspace:
                                   case JumpPointItemType.Library:
                                       return true;
                                   case JumpPointItemType.AppLink:
                                   case JumpPointItemType.Unknown:
                                   default:
                                       return false;
                               }
                           });
                case AppPath.Properties:
                case AppPath.AppLinks:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }

        public static bool IsCutEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                    return context.SelectedItems.Count > 0 &&
                           context.SelectedItems.All(i =>
                           {
                               switch (i.Type)
                               {
                                   case JumpPointItemType.File when ((FileBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Folder when ((FolderBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Drive when ((DriveBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Workspace:
                                   case JumpPointItemType.Library:
                                       return true;
                                   case JumpPointItemType.AppLink:
                                   case JumpPointItemType.Unknown:
                                   default:
                                       return false;
                               }
                           });

                case AppPath.AppLinks:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }

        public static bool IsCopyItemsPathEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0;
        }

        public static bool IsPasteEnabled(ShellContextViewModelBase context, bool clipboardHasFiles)
        {
            if (context is null) return false;

            return clipboardHasFiles && (context.PathInfo.Type == AppPath.Drive || context.PathInfo.Type == AppPath.Folder) && context.Item is DirectoryBase dir && dir.StorageType != StorageType.Cloud;
        }

        public static bool IsRenameEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                    return context.SelectedItems.Count > 0 &&
                           context.SelectedItems.All(i =>
                           {
                               switch (i.Type)
                               {
                                   case JumpPointItemType.File:
                                   case JumpPointItemType.Folder:
                                   case JumpPointItemType.Workspace:
                                   case JumpPointItemType.AppLink:
                                       return true;
                                   case JumpPointItemType.Drive:
                                   case JumpPointItemType.Library:
                                   case JumpPointItemType.Unknown:
                                   default:
                                       return false;
                               }
                           });

                case AppPath.Drives:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }

        public static bool IsDeleteEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                    return context.SelectedItems.Count > 0 &&
                           context.SelectedItems.All(i =>
                           {
                               switch (i.Type)
                               {
                                   case JumpPointItemType.File:
                                   case JumpPointItemType.Folder:
                                   case JumpPointItemType.Workspace:
                                   case JumpPointItemType.AppLink:
                                       return true;

                                   case JumpPointItemType.Drive:
                                   case JumpPointItemType.Library:
                                   case JumpPointItemType.Unknown:
                                   default:
                                       return false;
                               }
                           });

                case AppPath.Drives:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }
    
        public static bool IsDeletePermanentlyEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                    return context.SelectedItems.Count > 0 &&
                           context.SelectedItems.All(i =>
                           {
                               switch (i.Type)
                               {
                                   case JumpPointItemType.File when ((FileBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Folder when ((FolderBase)i).StorageType != StorageType.Cloud:
                                   case JumpPointItemType.Workspace:
                                   case JumpPointItemType.AppLink:
                                       return true;

                                   case JumpPointItemType.Drive:
                                   case JumpPointItemType.Library:
                                   case JumpPointItemType.Unknown:
                                   default:
                                       return false;
                               }
                           });

                case AppPath.Drives:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }
    
        public static bool IsOpenEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0;
        }

        public static bool IsOpenWithEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count == 1 && context.SelectedItems.All(i => DetailsPane.IsOpenWithEnabled(i));
        }

        public static bool IsOpenInNewTabEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                       i.Type == JumpPointItemType.Folder ||
                       i.Type == JumpPointItemType.Drive ||
                       i.Type == JumpPointItemType.Workspace ||
                       i.Type == JumpPointItemType.Library);
        }

        public static bool IsOpenInNewWindowEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                       i.Type == JumpPointItemType.Folder ||
                       i.Type == JumpPointItemType.Drive ||
                       i.Type == JumpPointItemType.Workspace ||
                       i.Type == JumpPointItemType.Library);
        }

        public static bool IsOpenInFileExplorerEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                   {
                       return DetailsPane.IsOpenInFileExplorerEnabled(i);
                   });
        }

        public static bool IsOpenInCommandPromptEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                   {
                       return DetailsPane.IsOpenInCommandPromptEnabled(i);
                    });
        }

        public static bool IsOpenInPowershellEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                   {
                       return DetailsPane.IsOpenInPowershellEnabled(i);
                   });
        }

        public static bool IsOpenInWindowsTerminalEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i =>
                   {
                       return DetailsPane.IsOpenInWindowsTerminalEnabled(i);
                   });
        }

        public static bool IsShareEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 && context.SelectedItems.All(i => i is StorageItemBase item && item.StorageType != StorageType.Cloud);
        }

        public static bool IsPropertiesEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0;
        }

        public static bool IsToolsEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0;
        }

        public static bool IsAddToWorkspaceEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => DetailsPane.IsAddToWorkspaceEnabled(i));
        }

        public static bool IsAddToFavoritesEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => i.Type != JumpPointItemType.Unknown && i.Type != JumpPointItemType.Library) &&
                   context.SelectedItems.Any(i => DetailsPane.IsAddToFavoritesEnabled(i));
        }

        public static bool IsRemoveFromFavoritesEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => i.Type != JumpPointItemType.Unknown && i.Type != JumpPointItemType.Library) &&
                   context.SelectedItems.Any(i => DetailsPane.IsRemoveFromFavoritesEnabled(i));
        }

        public static bool IsMoreToolsEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0;
        }

        public static bool IsSetWorkspaceTemplateEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => DetailsPane.IsSetWorkspaceTemplateEnabled(i));
        }

        public static bool IsSetFolderTemplateEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => DetailsPane.IsSetFolderTemplateEnabled(i));
        }

        public static bool IsDownloadEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            return context.SelectedItems.Count > 0 &&
                   context.SelectedItems.All(i => i is FileBase file && file.StorageType == StorageType.Cloud);
        }

        public static bool IsSelectAllEnabled(ShellContextViewModelBase context)
        {
            if (context == null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Dashboard:
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Cloud:
                    return true;
                    
                case AppPath.Chat:
                case AppPath.Properties:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }

        public static bool IsInvertSelectionEnabled(ShellContextViewModelBase context)
        {
            if (context is null) return false;
            switch (context.PathInfo.Type)
            {
                case AppPath.Dashboard:
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Cloud:
                    return true;
                    
                case AppPath.Chat:
                case AppPath.Properties:
                case AppPath.Settings:
                case AppPath.Unknown:
                default:
                    return false;
            }
        }

    }
}
