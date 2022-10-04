using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Interop.Enums;
using JumpPoint.Platform.Interop.Structs;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.LocalStorage;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.NetworkStorage;
using Windows.Storage;
using System.Threading.Tasks;
using FileAttributes = System.IO.FileAttributes;

namespace JumpPoint.Platform.Interop
{
    public static class FileInterop
    {

        [DllImport("api-ms-win-core-file-fromapp-l1-1-0.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindFirstFileExFromApp(
            string lpFileName,
            FINDEX_INFO_LEVELS fInfoLevelId,
            out WIN32_FIND_DATA lpFindFileData,
            FINDEX_SEARCH_OPS fSearchOp,
            IntPtr lpSearchFilter,
            FIND_FIRST_EX_ADDITIONAL_FLAGS dwAdditionalFlags);

        [DllImport("api-ms-win-core-file-l1-1-0.dll", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(
            IntPtr hFindFile,
            out WIN32_FIND_DATA lpFindFileData);

        [DllImport("api-ms-win-core-file-l1-1-0.dll")]
        public static extern bool FindClose(
            IntPtr hFindFile);

        [DllImport("api-ms-win-core-timezone-l1-1-0.dll", SetLastError = true)]
        public static extern bool FileTimeToSystemTime(
            ref FILETIME lpFileTime,
            out SYSTEMTIME lpSystemTime);

        public static IntPtr? GetFindHandle(string path, out WIN32_FIND_DATA findData)
        {
            var handle = FindFirstFileExFromApp(
                $@"\\?\{path}",
                FINDEX_INFO_LEVELS.FindExInfoBasic,
                out findData,
                FINDEX_SEARCH_OPS.FindExSearchNameMatch,
                IntPtr.Zero,
                FIND_FIRST_EX_ADDITIONAL_FLAGS.FIND_FIRST_EX_LARGE_FETCH);
            if (handle.ToInt64() == -1)
            {
                var errorCode = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(errorCode);
                FindClose(handle);
                switch (errorCode)
                {
                    case 2:
                    case 3:
                        //Messenger.Default.Send(
                        //    new NotificationMessage<Exception>(
                        //        new FileNotFoundException(win32Exception.Message, path),
                        //        $"File/Folder does not exist: {path}"),
                        //    MessengerTokens.ExceptionManagement);
                        break;
                    //throw new FileNotFoundException($"File/Folder does not exist: {path}", path, win32Exception);
                    case 5:
                        //Messenger.Default.Send(
                        //    new NotificationMessage<Exception>(
                        //        new UnauthorizedAccessException(win32Exception.Message),
                        //        win32Exception.Message),
                        //    MessengerTokens.ExceptionManagement);
                        break;
                    //throw new UnauthorizedAccessException(win32Exception.Message, win32Exception);
                    case 123:
                        Messenger.Default.Send(
                            new NotificationMessage<Exception>(
                                new ArgumentException(win32Exception.Message, nameof(path)),
                                $"Path is not valid: {path}"),
                            MessengerTokens.ExceptionManagement);
                        break;
                    //throw new ArgumentException($"Path is not valid: {path}", nameof(path), win32Exception);
                    default:
                        Messenger.Default.Send(
                            new NotificationMessage<Exception>(
                                win32Exception,
                                win32Exception.Message),
                            MessengerTokens.ExceptionManagement);
                        break;
                        //throw win32Exception;
                }
                return null;
            }
            return handle;
        }

        public static IEnumerable<StorageItemBase> GetItems(StorageType storageType, string directory)
        {
            var hFile = GetFindHandle($@"{directory.TrimEnd('\\')}\*.*", out var findData);
            if (hFile.HasValue && hFile.Value.ToInt64() != -1)
            {
                do
                {
                    // ((FileAttributes)findData.dwFileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                    if (((FileAttributes)findData.dwFileAttributes & FileAttributes.System) != FileAttributes.System)
                    {
                        var path = Path.Combine(directory, findData.cFileName);
                        if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                yield return GetFolder(storageType, path, findData);
                            }
                        }
                        else
                        {
                            if (!findData.cFileName.EndsWith(".lnk") && !findData.cFileName.EndsWith(".url"))
                            {
                                yield return GetFile(storageType, path, findData);
                            }
                        }
                    }
                } while (FindNextFile(hFile.Value, out findData));
                FindClose(hFile.Value);
            }
        }

        #region Folders

        public static IEnumerable<FolderBase> GetFolders(StorageType storageType, string directory)
        {
            var hFile = GetFindHandle($@"{directory.TrimEnd('\\')}\*.*", out var findData);
            if (hFile.HasValue && hFile.Value.ToInt64() != -1)
            {
                do
                {
                    // ((FileAttributes)findData.dwFileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                    if (((FileAttributes)findData.dwFileAttributes & FileAttributes.System) != FileAttributes.System)
                    {
                        if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                var path = Path.Combine(directory, findData.cFileName);
                                yield return GetFolder(storageType, path, findData);
                            }
                        }
                    }
                } while (FindNextFile(hFile.Value, out findData));
                FindClose(hFile.Value);
            }
        }

        public static FolderBase GetFolder(StorageType storageType, string path)
        {
            var folderInfo = GetFindInfo(path);
            return folderInfo.HasValue ? GetFolder(storageType, path, folderInfo.Value) : null;
        }

        public static FolderBase GetFolder(StorageType storageType, string path, WIN32_FIND_DATA data)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    return new LocalFolder(
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: null);

                case StorageType.Portable:
                    return new PortableFolder(
                        context: null,
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: null);

                case StorageType.Network:
                    return new NetworkFolder(
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: null);

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        #endregion

        #region Files

        public static IEnumerable<FileBase> GetFiles(StorageType storageType, string directory)
        {
            var hFile = GetFindHandle($@"{directory.TrimEnd('\\')}\*.*", out var findData);
            if (hFile.HasValue && hFile.Value.ToInt64() != -1)
            {
                do
                {
                    // ((FileAttributes)findData.dwFileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                    if (((FileAttributes)findData.dwFileAttributes & FileAttributes.System) != FileAttributes.System)
                    {
                        if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            if (!findData.cFileName.EndsWith(".lnk") && !findData.cFileName.EndsWith(".url"))
                            {
                                var path = Path.Combine(directory, findData.cFileName);
                                yield return GetFile(storageType, path, findData);
                            }
                        }
                    }
                } while (FindNextFile(hFile.Value, out findData));
                FindClose(hFile.Value);
            }
        }

        public static FileBase GetFile(StorageType storageType, string path)
        {
            var fileInfo = GetFindInfo(path);
            return fileInfo.HasValue ? GetFile(storageType, path, fileInfo.Value) : null;
        }

        public static FileBase GetFile(StorageType storageType, string path, WIN32_FIND_DATA data)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    return new LocalFile(
                        path: path,
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow));

                case StorageType.Portable:
                    return new PortableFile(
                        context: null,
                        path: path,
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow));

                case StorageType.Network:
                    return new NetworkFile(
                        path: path,
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow));

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        #endregion

        public static DriveBase GetDrive(StorageType storageType, string path, DriveTemplate template, string name)
        {
            var driveInfo = GetFindInfo(path);
            return driveInfo.HasValue ? GetDrive(storageType, path, template, name, driveInfo.Value) : null;
        }

        public static DriveBase GetDrive(StorageType storageType, string path, DriveTemplate template, string name, WIN32_FIND_DATA data)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    return new LocalDrive(
                        name: name,
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow))
                    {
                        DriveTemplate = template
                    };

                case StorageType.Portable:
                    return new PortableDrive(
                        name: name,
                        context: null,
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow));

                case StorageType.Network:
                    return new NetworkDrive(
                        name: name,
                        path: path.TrimEnd('\\'),
                        dateAccessed: ComputeDate(data.ftLastAccessTime),
                        dateCreated: ComputeDate(data.ftCreationTime),
                        dateModified: ComputeDate(data.ftLastWriteTime),
                        attributes: (FileAttributes?)data.dwFileAttributes,
                        size: ComputeFileSize(data.nFileSizeHigh, data.nFileSizeLow));

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        public static WIN32_FIND_DATA? GetFindInfo(string path)
        {
            var hFile = GetFindHandle($@"{path.TrimEnd('\\')}", out var findData);
            if (hFile.HasValue)
            {
                FindClose(hFile.Value);
                return findData;
            }
            return null;
        }

        public static ulong ComputeFileSize(uint fileSizeHigh, uint fileSizeLow)
        {
            ulong fDataFSize = fileSizeLow;

            if (fDataFSize < 0 && (long)fileSizeHigh > 0)
            {
                return fDataFSize + 4294967296 + (ulong)(fileSizeHigh * 4294967296);
            }
            else
            {
                if ((long)fileSizeHigh > 0)
                {
                    return fDataFSize + (ulong)(fileSizeHigh * 4294967296);
                }
                else if (fDataFSize < 0)
                {
                    return (fDataFSize + 4294967296);
                }
                else
                {
                    return fDataFSize;
                }
            }
        }

        public static DateTimeOffset? ComputeDate(FILETIME fileTime)
        {
            if (fileTime.Equals(default(FILETIME)))
            {
                return null;
            }

            if (FileTimeToSystemTime(ref fileTime, out var systemTime))
            {
                return new DateTimeOffset(
                    systemTime.Year, systemTime.Month, systemTime.Day, systemTime.Hour, systemTime.Minute, systemTime.Second, systemTime.Milliseconds, TimeSpan.Zero);
            }
            return null;
        }
    
        public static async Task<IStorageItem> GetStorageItem(StorageItemBase item)
        {
            switch (item)
            {
                case DirectoryBase directory:
                    return await GetStorageFolder(directory);

                case FileBase file:
                    return await GetStorageFile(file);

                default:
                    return null;
            }
        }

        public static async Task<StorageFolder> GetStorageFolder(DirectoryBase directory)
        {
            try
            {
                switch (directory.StorageType)
                {
                    case StorageType.Local:
                    case StorageType.Network:
                        return await StorageFolder.GetFolderFromPathAsync(directory.Path);

                    case StorageType.Portable when directory is IPortableDirectory portableDirectory:
                        return portableDirectory.Context != null ?
                            portableDirectory.Context.Context as StorageFolder :
                            await StorageFolder.GetFolderFromPathAsync(directory.Path);

                    case StorageType.Cloud:
                    default:
                        return null;
                }
            }
            catch
            {
                //Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        public static async Task<StorageFile> GetStorageFile(FileBase file)
        {
            try
            {
                switch (file.StorageType)
                {
                    case StorageType.Local:
                    case StorageType.Network:
                        return await StorageFile.GetFileFromPathAsync(file.Path);

                    case StorageType.Portable when file is PortableFile portableFile:
                        return portableFile.Context != null ?
                            portableFile.Context.Context as StorageFile :
                            await StorageFile.GetFileFromPathAsync(file.Path);

                    case StorageType.Cloud:
                    default:
                        return null;
                }
            }
            catch
            {
                //Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

    }
}
