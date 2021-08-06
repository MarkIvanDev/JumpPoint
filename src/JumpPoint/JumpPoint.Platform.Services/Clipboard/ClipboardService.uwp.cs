using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class ClipboardService
    {
        public static async Task<IList<ClipboardItem>> GetClipboardItems(DataPackageView data)
        {
            var clipboardItems = new List<ClipboardItem>();

            if (data.Contains(StandardDataFormats.StorageItems))
            {
                var items = await data.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    var storageType = string.IsNullOrEmpty(item.Path) ?
                        StorageType.Portable :
                        await StorageService.GetPathStorageType(item.Path);
                    if (!storageType.HasValue) continue;
                    
                    if (item is StorageFile storageFile)
                    {
                        var file = await StorageService.GetFile(storageType.Value, storageFile);
                        if (file != null)
                        {
                            if (string.IsNullOrEmpty(file.Path))
                            {
                                file.Path = await StorageService.GetPath(storageFile);
                            }
                            await StorageService.Load(file);
                            clipboardItems.Add(new ClipboardFileItem
                            {
                                File = file
                            });
                        }
                    }
                    else if (item is StorageFolder storageFolder)
                    {
                        var parent = await storageFolder.GetParentAsync();
                        if (parent != null)
                        {
                            var folder = await StorageService.GetFolder(storageType.Value, storageFolder);
                            if (folder != null)
                            {
                                if (string.IsNullOrEmpty(folder.Path))
                                {
                                    folder.Path = await StorageService.GetPath(storageFolder);
                                }
                                folder.FolderTemplate = await FolderTemplateService.GetFolderTemplate(folder);
                                clipboardItems.Add(new ClipboardFolderItem
                                {
                                    Folder = folder
                                });
                            }
                        }
                        else
                        {
                            var drive = await StorageService.GetDrive(storageType.Value, storageFolder);
                            if (drive != null)
                            {
                                if (string.IsNullOrEmpty(drive.Path))
                                {
                                    drive.Path = await StorageService.GetPath(storageFolder);
                                }
                                drive.DriveTemplate = StorageService.GetDriveTemplate(drive.Path);
                                clipboardItems.Add(new ClipboardFolderItem
                                {
                                    Folder = drive
                                });
                            }
                        }
                    }
                }
            }

            if (data.Contains(StandardDataFormats.Text))
            {
                var text = await data.GetTextAsync();
                clipboardItems.Add(new ClipboardTextItem
                {
                    Text = text
                });
            }

            if (data.Contains(StandardDataFormats.Bitmap))
            {
                var bitmap = await data.GetBitmapAsync();
                var stream = await bitmap.OpenReadAsync();
                clipboardItems.Add(new ClipboardBitmapItem
                {
                    Bitmap = stream.AsStream()
                });
            }

            return clipboardItems;
        }
    
        public static async Task<IList<StorageItemBase>> GetStorageItems(DataPackageView data)
        {
            var storageItems = new List<StorageItemBase>();

            if (data.Contains(StandardDataFormats.StorageItems))
            {
                var items = await data.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    var storageType = string.IsNullOrEmpty(item.Path) ?
                        StorageType.Portable :
                        await StorageService.GetPathStorageType(item.Path);
                    if (!storageType.HasValue) continue;

                    if (item is StorageFile storageFile)
                    {
                        var file = await StorageService.GetFile(storageType.Value, storageFile);
                        if (file != null)
                        {
                            if (string.IsNullOrEmpty(file.Path))
                            {
                                file.Path = await StorageService.GetPath(storageFile);
                            }
                            storageItems.Add(file);
                        }
                    }
                    else if (item is StorageFolder storageFolder)
                    {
                        var parent = await storageFolder.GetParentAsync();
                        if (parent != null)
                        {
                            var folder = await StorageService.GetFolder(storageType.Value, storageFolder);
                            if (folder != null)
                            {
                                if (string.IsNullOrEmpty(folder.Path))
                                {
                                    folder.Path = await StorageService.GetPath(storageFolder);
                                }
                                storageItems.Add(folder);
                            }
                        }
                        else
                        {
                            var drive = await StorageService.GetDrive(storageType.Value, storageFolder);
                            if (drive != null)
                            {
                                if (string.IsNullOrEmpty(drive.Path))
                                {
                                    drive.Path = await StorageService.GetPath(storageFolder);
                                }
                                storageItems.Add(drive);
                            }
                        }
                    }
                }
            }

            return storageItems;
        }
    
    }
}
