using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JumpPoint.Platform.Items.CloudStorage.OneDrive
{
    public class OneDriveDrive : CloudDrive
    {
        public OneDriveDrive(string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(name, CloudStorageService.OneDrive, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
        }
    }
}
