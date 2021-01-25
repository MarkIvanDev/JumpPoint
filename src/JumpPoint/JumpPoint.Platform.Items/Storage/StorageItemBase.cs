using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JumpPoint.Platform.Items.Storage
{
    public abstract class StorageItemBase : JumpPointItem
    {
        public StorageItemBase(JumpPointItemType type, StorageType storageType,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size)
            : base(type)
        {
            StorageType = storageType;
            Path = path;
            DateAccessed = dateAccessed;
            DateCreated = dateCreated;
            DateModified = dateModified;
            Attributes = attributes;
            Size = size;
        }

        public StorageType StorageType { get; }

        public override string Path
        {
            get { return base.Path; }
            set
            {
                base.Path = value;
                Name = System.IO.Path.GetFileName(value);
            }
        }

        private DateTimeOffset? _dateAccessed;

        public DateTimeOffset? DateAccessed
        {
            get { return _dateAccessed; }
            set { Set(ref _dateAccessed, value); }
        }


        private DateTimeOffset? _dateCreated;

        public DateTimeOffset? DateCreated
        {
            get { return _dateCreated; }
            set { Set(ref _dateCreated, value); }
        }

        private DateTimeOffset? _dateModified;

        public DateTimeOffset? DateModified
        {
            get { return _dateModified; }
            set { Set(ref _dateModified, value); }
        }

        private FileAttributes? _attributes;

        public FileAttributes? Attributes
        {
            get { return _attributes; }
            set { Set(ref _attributes, value); }

        }

        private ulong? _size;

        public ulong? Size
        {
            get { return _size; }
            set { Set(ref _size, value); }
        }

        private ulong? _sizeOnDisk;

        public ulong? SizeOnDisk
        {
            get { return _sizeOnDisk; }
            set { Set(ref _sizeOnDisk, value); }
        }

    }
}
