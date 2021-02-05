using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JumpPoint.Platform.Items.CloudStorage.OneDrive
{
    public class OneDriveFile : CloudFile
    {
        public OneDriveFile(
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(CloudStorageService.OneDrive, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            
        }
    }

}
