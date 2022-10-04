using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.OpenDrive;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Extensions;
using OpenDriveSharp;

namespace JumpPoint.Platform.Services.OpenDrive
{
    public static class OpenDriveHelper
    {
        public static async Task<IOpenDriveDirectoryInfo> GetDirectoryInfo(OpenDriveClient client, string folderId)
        {
            var infoResult = await client.GetFolderInfo(folderId).ConfigureAwait(false);
            if (infoResult.IsSuccessful && infoResult is FolderInfoResult info)
            {
                if (folderId != "0")
                {
                    var pathResult = await client.GetFolderPath(folderId).ConfigureAwait(false);
                    if (pathResult.IsSuccessful && pathResult is FolderPathResult path)
                    {
                        return new OpenDriveFolderInfo
                        {
                            Id = info.FolderId,
                            Name = info.Name,
                            Path = path.Path,
                            Link = info.Link,
                            DateCreated = info.DateCreated == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateCreated),
                            DateModified = info.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateModified)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var userResult = await client.GetUserInfo().ConfigureAwait(false);
                    if (userResult.IsSuccessful && userResult is UserInfoResult user)
                    {
                        return new OpenDriveDriveInfo
                        {
                            Id = info.FolderId,
                            Name = info.Name,
                            Path = string.Empty,
                            Link = info.Link,
                            DateCreated = info.DateCreated == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateCreated),
                            DateModified = info.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateModified),
                            Capacity = (ulong?)(user.MaxStorage * 1024 * 1024),
                            UsedSpace = (ulong?)user.StorageUsed
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<OpenDriveFileInfo> GetFileInfo(OpenDriveClient client, string fileId)
        {
            var infoResult = await client.GetFileInfo(fileId).ConfigureAwait(false);
            if (infoResult.IsSuccessful && infoResult is FileInfoResult info)
            {
                var pathResult = await client.GetFilePath(fileId).ConfigureAwait(false);
                if (pathResult.IsSuccessful && pathResult is FilePathResult path)
                {
                    return new OpenDriveFileInfo
                    {
                        Id = info.FileId,
                        Name = info.Name,
                        Extension = info.Extension,
                        Path = path.Path,
                        Link = info.Link,
                        DownloadLink = info.DownloadLink,
                        Size = info.Size,
                        GroupId = info.GroupId,
                        DateCreated = info.DateUploaded == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateUploaded),
                        DateModified = info.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateModified),
                        DateAccessed = info.DateAccessed == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(info.DateAccessed)
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<IList<IOpenDriveItemInfo>> GetItems(OpenDriveClient client, IOpenDriveDirectoryInfo directory)
        {
            var contents = new List<IOpenDriveItemInfo>();
            var itemsResult = await client.ListFolderContents(directory.Id);
            if (itemsResult.IsSuccessful && itemsResult is ListFolderContentResult items)
            {
                foreach (var item in items.Folders ?? Array.Empty<ListFolderContentResult.Folder>())
                {
                    var pathResult = await client.GetFolderPath(item.FolderId).ConfigureAwait(false);
                    if (pathResult.IsSuccessful && pathResult is FolderPathResult path)
                    {
                        contents.Add(new OpenDriveFolderInfo
                        {
                            Id = item.FolderId,
                            Name = item.Name,
                            Path = path.Path,
                            Link = item.Link,
                            DateCreated = item.DateCreated == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(item.DateCreated),
                            DateModified = item.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(item.DateModified)
                        });
                    }
                }

                foreach (var item in items.Files ?? Array.Empty<ListFolderContentResult.File>())
                {
                    var pathResult = await client.GetFilePath(item.FileId).ConfigureAwait(false);
                    if (pathResult.IsSuccessful && pathResult is FilePathResult path)
                    {
                        contents.Add(new OpenDriveFileInfo
                        {
                            Id = item.FileId,
                            Name = item.Name,
                            Extension = item.Extension,
                            Path = directory.IsRoot && path.Path.StartsWith("Root/") ? path.Path.Substring(5) : path.Path,
                            Link = item.Link,
                            DownloadLink = item.DownloadLink,
                            Size = item.Size,
                            GroupId = item.GroupId,
                            DateModified = item.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(item.DateModified),
                        });
                    }
                }
            }
            return contents;
        }

        public static async Task<OpenDriveFolderInfo> CreateFolder(OpenDriveClient client, string name, IOpenDriveDirectoryInfo parentFolder, CreateOption option)
        {
            try
            {
                var newName = name.Trim();
                var itemExists = await FolderExists(client, parentFolder.Id, newName);
                switch (option)
                {
                    case CreateOption.DoNothing:
                        if (itemExists)
                        {
                            return null;
                        }
                        break;
                
                    case CreateOption.ReplaceExisting:
                    case CreateOption.OpenIfExists:
                        if (itemExists)
                        {
                            var itemByNameResult = await client.GetFolderItemByName(parentFolder.Id, newName);
                            if (itemByNameResult.IsSuccessful && itemByNameResult is FolderItemByNameResult itemByName)
                            {
                                var folder = itemByName.Folders?.FirstOrDefault();
                                if (folder != null)
                                {
                                    return await GetDirectoryInfo(client, folder.FolderId) as OpenDriveFolderInfo;
                                }
                            }
                        }
                        break;

                    case CreateOption.GenerateUniqueName:
                    default:
                        if (itemExists)
                        {
                            var number = 2;
                            var origName = newName;
                            newName = $"{origName} {number}";
                            while (await FolderExists(client, parentFolder.Id, newName))
                            {
                                number += 1;
                                newName = $"{origName} {number}";
                            }
                        }
                        break;
                }
                var result = await client.CreateFolder(newName, parentFolder.Id);
                if (result.IsSuccessful && result is CreateFolderResult r)
                {
                    var pathResult = await client.GetFolderPath(r.FolderId).ConfigureAwait(false);
                    if (pathResult.IsSuccessful && pathResult is FolderPathResult path)
                    {
                        return new OpenDriveFolderInfo
                        {
                            Id = r.FolderId,
                            Name = r.Name,
                            Path = path.Path,
                            Link = r.Link,
                            DateCreated = r.DateCreated == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(r.DateCreated),
                            DateModified = r.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(r.DateModified)
                        };
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OpenDriveFileInfo> CreateFile(OpenDriveClient client, string name, IOpenDriveDirectoryInfo parentFolder, CreateOption option, byte[] content)
        {
            try
            {
                var newName = name.Trim();
                var itemExists = await FileExists(client, parentFolder.Id, newName);
                switch (option)
                {
                    case CreateOption.DoNothing:
                        if (itemExists)
                        {
                            return null;
                        }
                        break;

                    case CreateOption.ReplaceExisting:
                    case CreateOption.OpenIfExists:
                        if (itemExists)
                        {
                            var itemByNameResult = await client.GetFolderItemByName(parentFolder.Id, newName);
                            if (itemByNameResult.IsSuccessful && itemByNameResult is FolderItemByNameResult itemByName)
                            {
                                var file = itemByName.Files?.FirstOrDefault();
                                if (file != null)
                                {
                                    return await GetFileInfo(client, file.FileId);
                                }
                            }
                        }
                        break;

                    case CreateOption.GenerateUniqueName:
                    default:
                        if (itemExists)
                        {
                            var number = 2;
                            var fileName = Path.GetFileNameWithoutExtension(newName);
                            var ext = Path.GetExtension(newName);
                            newName = $"{fileName} ({number}){ext}";
                            while (await FileExists(client, parentFolder.Id, newName))
                            {
                                number += 1;
                                newName = $"{fileName} ({number}){ext}";
                            }
                        }
                        break;
                }
                if (content is null)
                {
                    var createFileResult = await client.CreateFile(parentFolder.Id, parentFolder.Id, "txt");
                    if (createFileResult.IsSuccessful && createFileResult is CreateFileResult createFile)
                    {
                        var renameFileResult = await client.RenameFile(createFile.FileId, newName);
                        if (renameFileResult.IsSuccessful && renameFileResult is RenameFileResult renameFile)
                        {
                            return await GetFileInfo(client, renameFile.FileId);
                        }
                    }
                }
                else
                {
                    var stream = content.ToMemoryStream();
                    var fileHash = string.Empty;
                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(stream);
                        fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                    var result = await client.UploadFile(stream, parentFolder.Id, newName, content.Length, fileHash, open_if_exists: 1);
                    if (result.IsSuccessful && result is UploadCloseFileResult upload)
                    {
                        var pathResult = await client.GetFilePath(upload.FileId).ConfigureAwait(false);
                        if (pathResult.IsSuccessful && pathResult is FilePathResult path)
                        {
                            return new OpenDriveFileInfo
                            {
                                Id = upload.FileId,
                                Name = upload.Name,
                                Extension = upload.Extension,
                                Path = path.Path,
                                Link = upload.Link,
                                DownloadLink = upload.DownloadLink,
                                Size = upload.Size,
                                GroupId = upload.GroupId,
                                DateCreated = upload.DateUploaded == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(upload.DateUploaded),
                                DateModified = upload.DateModified == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(upload.DateModified),
                                DateAccessed = upload.DateAccessed == 0 ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(upload.DateAccessed)
                            };
                        }
                    }
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> RenameFolder(OpenDriveClient client, OpenDriveFolderInfo folder, string name, RenameOption option)
        {
            try
            {
                var newName = name.Trim();
                var segments = folder.Path.Split('/');
                segments[segments.Length - 1] = newName;
                var itemExists = await FolderExists(client, string.Join("/", segments));
                switch (option)
                {
                    case RenameOption.ReplaceExisting:
                    case RenameOption.DoNothing:
                        if (itemExists)
                        {
                            return string.Empty;
                        }
                        break;

                    case RenameOption.GenerateUniqueName:
                    default:
                        if (itemExists)
                        {
                            var number = 2;
                            var origName = newName;
                            newName = $"{origName} {number}";
                            segments[segments.Length - 1] = newName;
                            while (await FolderExists(client, string.Join("/", segments)))
                            {
                                number += 1;
                                newName = $"{origName} {number}";
                                segments[segments.Length - 1] = newName;
                            }
                        }
                        break;
                }

                var renameResult = await client.RenameFolder(folder.Id, newName);
                if (renameResult.IsSuccessful && renameResult is RenameFolderResult rename)
                {
                    folder.Name = rename.Name;
                    return rename.Name;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task<string> RenameFile(OpenDriveClient client, OpenDriveFileInfo file, string name, RenameOption option)
        {
            try
            {
                var newName = name.Trim();
                var segments = file.Path.Split('/');
                segments[segments.Length - 1] = newName;
                var itemExists = await FileExists(client, string.Join("/", segments));
                switch (option)
                {
                    case RenameOption.ReplaceExisting:
                    case RenameOption.DoNothing:
                        if (itemExists)
                        {
                            return string.Empty;
                        }
                        break;

                    case RenameOption.GenerateUniqueName:
                    default:
                        if (itemExists)
                        {
                            var number = 2;
                            var origName = newName;
                            newName = $"{origName} {number}";
                            segments[segments.Length - 1] = newName;
                            while (await FileExists(client, string.Join("/", segments)))
                            {
                                number += 1;
                                newName = $"{origName} {number}";
                                segments[segments.Length - 1] = newName;
                            }
                        }
                        break;
                }

                var renameResult = await client.RenameFile(file.Id, newName);
                if (renameResult.IsSuccessful && renameResult is RenameFileResult rename)
                {
                    file.Name = rename.Name;
                    return rename.Name;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task<bool> FolderExists(OpenDriveClient client, string parentFolderId, string name)
        {
            var itemByNameResult = await client.GetFolderItemByName(parentFolderId, name);
            if (itemByNameResult.IsSuccessful && itemByNameResult is FolderItemByNameResult itemByName)
            {
                return itemByName.Folders?.FirstOrDefault() != null;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> FolderExists(OpenDriveClient client, string path)
        {
            var idResult = await client.GetFolderIdByPath(path);
            return idResult.IsSuccessful;
        }

        public static async Task<bool> FileExists(OpenDriveClient client, string parentFolderId, string name)
        {
            var itemByNameResult = await client.GetFolderItemByName(parentFolderId, name);
            if (itemByNameResult.IsSuccessful && itemByNameResult is FolderItemByNameResult itemByName)
            {
                return itemByName.Files?.FirstOrDefault() != null;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> FileExists(OpenDriveClient client, string path)
        {
            var idResult = await client.GetFileIdByPath(path);
            return idResult.IsSuccessful;
        }

        public static StorageItemBase Convert(this IOpenDriveItemInfo info, OpenDriveAccount account)
        {
            if (info is OpenDriveDriveInfo drive)
            {
                return new OpenDriveDrive(account, drive);
            }
            else if (info is OpenDriveFolderInfo folder)
            {
                var path = $@"cloud:\OpenDrive\{account.Name}\{info.Path.Replace('/', '\\')}";
                return new OpenDriveFolder(account, folder, path);
            }
            else if (info is OpenDriveFileInfo file)
            {
                var path = $@"cloud:\OpenDrive\{account.Name}\{info.Path.Replace('/', '\\')}";
                return new OpenDriveFile(account, file, path);
            }
            else
            {
                return null;
            }
        }

    }


}
