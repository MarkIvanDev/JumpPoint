using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using Microsoft.Graph;

namespace JumpPoint.Platform.Items.OneDrive
{
    public class OneDriveFile : CloudFile
    {
        public OneDriveFile(OneDriveAccount account, DriveItem driveItem, string path) :
            base(CloudStorageProvider.OneDrive, path, null, driveItem.CreatedDateTime, driveItem.LastModifiedDateTime, FileAttributes.Normal, (ulong?)driveItem.Size)
        {
            Account = account;
            GraphItem = driveItem;
        }

        public OneDriveAccount Account { get; }

        public DriveItem GraphItem { get; }

    }

}
