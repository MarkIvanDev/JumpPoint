using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;

namespace JumpPoint.Platform.Items
{
    public abstract class FileBase : StorageItemBase
    {
        public FileBase(StorageType storageType,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(JumpPointItemType.File, storageType, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
        }

        public override string Path
        {
            get { return base.Path; }
            set
            {
                base.Path = value;
                var displayName = System.IO.Path.GetFileNameWithoutExtension(value);
                DisplayName = string.IsNullOrEmpty(displayName) ? Name : displayName;
                FileType = System.IO.Path.GetExtension(value);
            }
        }

        private string _fileType;

        public string FileType
        {
            get { return _fileType; }
            set
            {
                Set(ref _fileType, value);
                DisplayType = $"{FileType.TrimStart('.').ToUpper()} File";
            }
        }

        private string _contentType;

        public string ContentType
        {
            get { return _contentType; }
            set { Set(ref _contentType, value); }
        }

        private Stream _thumbnail;

        public Stream Thumbnail
        {
            get { return _thumbnail; }
            set { Set(ref _thumbnail, value); }
        }

        #region Properties

        private CoreProperties _coreProperties;

        public CoreProperties CoreProperties
        {
            get { return _coreProperties; }
            set { Set(ref _coreProperties, value); }
        }

        private DocumentProperties _documentProperties;

        public DocumentProperties DocumentProperties
        {
            get { return _documentProperties; }
            set { Set(ref _documentProperties, value); }
        }

        private ImageProperties _imageProperties;

        public ImageProperties ImageProperties
        {
            get { return _imageProperties; }
            set { Set(ref _imageProperties, value); }
        }

        private PhotoProperties _photoProperties;

        public PhotoProperties PhotoProperties
        {
            get { return _photoProperties; }
            set { Set(ref _photoProperties, value); }
        }

        private MediaProperties _mediaProperties;

        public MediaProperties MediaProperties
        {
            get { return _mediaProperties; }
            set { Set(ref _mediaProperties, value); }
        }

        private AudioProperties _audioProperties;

        public AudioProperties AudioProperties
        {
            get { return _audioProperties; }
            set { Set(ref _audioProperties, value); }
        }

        private MusicProperties musicProperties;

        public MusicProperties MusicProperties
        {
            get { return musicProperties; }
            set { Set(ref musicProperties, value); }
        }

        private VideoProperties videoProperties;

        public VideoProperties VideoProperties
        {
            get { return videoProperties; }
            set { Set(ref videoProperties, value); }
        }

        private DrmProperties _drmProperties;

        public DrmProperties DrmProperties
        {
            get { return _drmProperties; }
            set { Set(ref _drmProperties, value); }
        }

        private GpsProperties _gpsProperties;

        public GpsProperties GpsProperties
        {
            get { return _gpsProperties; }
            set { Set(ref _gpsProperties, value); }
        }

        #endregion

    }
}
