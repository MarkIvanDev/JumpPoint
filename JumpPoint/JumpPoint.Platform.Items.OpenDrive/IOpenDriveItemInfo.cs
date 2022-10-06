using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.OpenDrive
{
    public interface IOpenDriveItemInfo
    {
        string Id { get; set; }
        string Name { get; set; }
        string Path { get; set; }
    }

    public interface IOpenDriveDirectoryInfo : IOpenDriveItemInfo
    {
        bool IsRoot { get; }
    }

    public class OpenDriveDriveInfo : IOpenDriveDirectoryInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? DateModified { get; set; }
        public string Link { get; set; }
        public ulong? Capacity { get; set; }
        public ulong? UsedSpace { get; set; }
        public bool IsRoot { get; } = true;
    }

    public class OpenDriveFolderInfo : IOpenDriveDirectoryInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? DateModified { get; set; }
        public string Link { get; set; }
        public bool IsRoot { get; } = false;
    }

    public class OpenDriveFileInfo : IOpenDriveItemInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int GroupId { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? DateModified { get; set; }
        public DateTimeOffset? DateAccessed { get; set; }
        public string Link { get; set; }
        public string DownloadLink { get; set; }
        public string ThumbLink { get; set; }
        public string WebLink => $"https://www.opendrive.com/file/{Id}";
    }
}
