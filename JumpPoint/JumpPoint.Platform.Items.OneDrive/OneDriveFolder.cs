using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;
using Microsoft.Graph;

namespace JumpPoint.Platform.Items.OneDrive
{
    public class OneDriveFolder : CloudFolder
    {
        public OneDriveFolder(OneDriveAccount account, DriveItem driveItem, string path) :
            base(CloudStorageProvider.OneDrive, path, null, driveItem.CreatedDateTime, driveItem.LastModifiedDateTime, FileAttributes.Directory, (ulong?)driveItem.Size)
        {
            Account = account;
            GraphItem = driveItem;
            FolderType = FolderType.Regular;
        }

        public OneDriveAccount Account { get; }

        public DriveItem GraphItem { get; }

    }
}
