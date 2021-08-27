using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using NittyGritty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JumpPoint.Platform.Models
{
    public abstract class ClipboardItem : ObservableObject
    {
        public ClipboardItem(ClipboardItemType type)
        {
            Type = type;
        }

        public ClipboardItemType Type { get; }
    }

    public enum ClipboardItemType
    {
        Unknown = 0,
        Folder = 1,
        File = 2,
        Text = 3,
        Bitmap = 4,
    }

    public class ClipboardFolderItem : ClipboardItem
    {
        public ClipboardFolderItem() : base(ClipboardItemType.Folder)
        {
        }

        private DirectoryBase _folder;

        public DirectoryBase Folder
        {
            get { return _folder; }
            set { Set(ref _folder, value); }
        }

    }

    public class ClipboardFileItem : ClipboardItem
    {
        public ClipboardFileItem() : base(ClipboardItemType.File)
        {
        }

        private FileBase _file;

        public FileBase File
        {
            get { return _file; }
            set { Set(ref _file, value); }
        }

    }

    public class ClipboardTextItem : ClipboardItem
    {
        public ClipboardTextItem() : base(ClipboardItemType.Text)
        {
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

    }

    public class ClipboardBitmapItem : ClipboardItem
    {
        public ClipboardBitmapItem() : base(ClipboardItemType.Bitmap)
        {
        }

        private Stream _bitmap;

        public Stream Bitmap
        {
            get { return _bitmap; }
            set { Set(ref _bitmap, value); }
        }

    }

}
