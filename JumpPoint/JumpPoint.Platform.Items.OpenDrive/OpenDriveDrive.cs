using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;

namespace JumpPoint.Platform.Items.OpenDrive
{
    public class OpenDriveDrive : CloudDrive
    {
        public OpenDriveDrive(OpenDriveAccount account, OpenDriveDriveInfo info) :
            base(CloudStorageProvider.OpenDrive, $"{account.Name} ({account.Email})", $@"cloud:\OpenDrive\{account.Name}",
                null, info?.DateCreated, info?.DateModified, FileAttributes.Directory, null)
        {
            Account = account;
            Info = info;
            FileSystem = CloudStorageProvider.OpenDrive.Humanize();
            FreeSpace = info?.Capacity - info?.UsedSpace;
            Capacity = info?.Capacity;
            UsedSpace = info?.UsedSpace;
        }

        public OpenDriveAccount Account { get; }

        public OpenDriveDriveInfo Info { get; }
    }
}
