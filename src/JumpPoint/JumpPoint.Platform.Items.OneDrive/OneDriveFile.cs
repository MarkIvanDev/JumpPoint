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
        public OneDriveFile(
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(CloudStorageProvider.OneDrive, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            
        }

        private string _etag;

        public string ETag
        {
            get { return _etag; }
            set { Set(ref _etag, value); }
        }

        private string _webUrl;

        public string WebUrl
        {
            get { return _webUrl; }
            set { Set(ref _webUrl, value); }
        }

        private string _ctag;

        public string CTag
        {
            get { return _ctag; }
            set { Set(ref _ctag, value); }
        }

        private DriveItem _graphItem;

        public DriveItem GraphItem
        {
            get { return _graphItem; }
            set { Set(ref _graphItem, value); }
        }

    }

}
