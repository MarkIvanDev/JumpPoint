using JumpPoint.Platform;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Helpers
{
    public interface ITileIconHelper
    {
        Uri GetIconUri(AppPath appPath, TileSize tileSize = TileSize.Medium);

        Uri GetIconUri(FolderTemplate folderTemplate, TileSize tileSize = TileSize.Medium);

        Uri GetIconUri(DriveTemplate driveTemplate, TileSize tileSize = TileSize.Medium);

        Uri GetIconUri(WorkspaceTemplate workspaceTemplate, TileSize tileSize = TileSize.Medium);

        Uri GetIconUri(CloudStorageProvider cloudStorageProvider, TileSize tileSize = TileSize.Medium);

        Uri GetIconUri(AppPath pathType, object tag, TileSize tileSize = TileSize.Medium);
    }

    public enum TileSize
    {
        Medium = 0,
        Small = 1,
        Wide = 2,
        Large = 3
    }
}
