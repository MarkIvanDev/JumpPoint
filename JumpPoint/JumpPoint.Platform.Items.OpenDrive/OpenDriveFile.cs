using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;

namespace JumpPoint.Platform.Items.OpenDrive
{
    public class OpenDriveFile : CloudFile
    {
        public OpenDriveFile(OpenDriveAccount account, OpenDriveFileInfo info, string path) :
            base(CloudStorageProvider.OpenDrive, path, null, null, info.DateModified, FileAttributes.Normal, (ulong?)info.Size)
        {
            Account = account;
            Info = info;
        }

        public OpenDriveAccount Account { get; }

        public OpenDriveFileInfo Info { get; }
    }
}
