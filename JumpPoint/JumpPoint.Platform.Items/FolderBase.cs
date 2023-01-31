using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items
{
    public abstract class FolderBase : DirectoryBase
    {
        public FolderBase(StorageType storageType,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(JumpPointItemType.Folder, storageType, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            DisplayName = Name;
            FolderType = FolderType.Regular;
            FolderTemplate = FolderTemplate.General;
        }

        private FolderType _folderType;

        public FolderType FolderType
        {
            get { return _folderType; }
            set { Set(ref _folderType, value); }
        }

        private FolderTemplate _folderTemplate;

        public FolderTemplate FolderTemplate
        {
            get { return _folderTemplate; }
            set
            {
                Set(ref _folderTemplate, value);
                DisplayType = $"{_folderTemplate.Humanize()} Folder";
            }
        }

        private ulong _fileCount;

        public ulong FileCount
        {
            get { return _fileCount; }
            set { Set(ref _fileCount, value); }
        }

        private ulong _folderCount;

        public ulong FolderCount
        {
            get { return _folderCount; }
            set { Set(ref _folderCount, value); }
        }

    }
}
