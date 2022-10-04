using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.OpenDrive
{
    public class OpenDriveFolder : CloudFolder
    {
        public OpenDriveFolder(OpenDriveAccount account, OpenDriveFolderInfo info, string path) :
            base(CloudStorageProvider.OpenDrive, path, null, info.DateCreated, info.DateModified, FileAttributes.Directory, null)
        {
            Account = account;
            Info = info;
            FolderType = FolderType.Regular;
        }

        public OpenDriveAccount Account { get; }

        public OpenDriveFolderInfo Info { get; }
    }
}
