using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class BasicProperties : ObservableObject
    {
        public static class Key
        {
            // File System
            public static string DateAccessed => "System.DateAccessed";
            public static string DateCreated => "System.DateCreated";
            public static string DateModified => "System.DateModified";
            public static string Attributes => "System.FileAttributes";
            public static string Size => "System.Size";
            public static string SizeOnDisk => "System.FileAllocationSize";
            public static string ParsingPath => "System.ParsingPath";

            public static IEnumerable<string> Common()
            {
                yield return DateAccessed;
                yield return DateCreated;
                yield return DateModified;
                yield return Attributes;
                yield return Size;
                yield return SizeOnDisk;
            }

            // File

            // Folder
            public static string FileCount => "System.FileCount";
            public static string FolderKind => "System.FolderKind";
            public static IEnumerable<string> Folder()
            {
                yield return FileCount;
                yield return FolderKind;
            }

            // Drive
            public static string FileSystem => "System.Volume.FileSystem";
            public static string FreeSpace => "System.FreeSpace";
            public static string Capacity => "System.Capacity";
            public static IEnumerable<string> Drive()
            {
                yield return FileSystem;
                yield return FreeSpace;
                yield return Capacity;
            }
        }
    }
}
