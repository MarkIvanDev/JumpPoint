using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using Microsoft.Graph;

namespace JumpPoint.Platform.Items.OneDrive
{
    public class OneDriveDrive : CloudDrive
    {
        public OneDriveDrive(OneDriveAccount account, Drive drive) :
            base(CloudStorageProvider.OneDrive, $"{account.Name} ({account.Email})", $@"cloud:\OneDrive\{account.Name}",
                null, drive?.CreatedDateTime, drive?.LastModifiedDateTime, FileAttributes.Directory, null)
        {
            Account = account;
            GraphItem = drive;
            FileSystem = CloudStorageProvider.OneDrive.Humanize();
            FreeSpace = (ulong?)drive?.Quota?.Remaining;
            Capacity = (ulong?)drive?.Quota?.Total;
            UsedSpace = (ulong?)drive?.Quota?.Used;
        }

        public OneDriveAccount Account { get; }

        public Drive GraphItem { get; }

    }
}
