using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items
{
    public abstract class DriveBase : DirectoryBase
    {
        protected DriveBase(StorageType storageType, string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(JumpPointItemType.Drive, storageType, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            DriveTemplate = DriveTemplate.Unknown;
            Name = name;
            DisplayName = name;
        }

        private DriveTemplate _driveTemplate;

        public DriveTemplate DriveTemplate
        {
            get { return _driveTemplate; }
            set
            {
                Set(ref _driveTemplate, value);
                DisplayType = $"{_driveTemplate.Humanize()} Drive";
            }
        }

        private string _fileSystem;

        public string FileSystem
        {
            get { return _fileSystem; }
            set { Set(ref _fileSystem, value); }
        }

        private ulong? _usedSpace;

        public ulong? UsedSpace
        {
            get { return _usedSpace; }
            set { Set(ref _usedSpace, value); }
        }

        private ulong? _freeSpace;

        public ulong? FreeSpace
        {
            get { return _freeSpace; }
            set { Set(ref _freeSpace, value); }
        }

        private ulong? _capacity;

        public ulong? Capacity
        {
            get { return _capacity; }
            set { Set(ref _capacity, value); }
        }

    }
}
