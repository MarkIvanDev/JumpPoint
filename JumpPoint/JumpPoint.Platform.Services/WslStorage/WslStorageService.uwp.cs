using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.WslStorage;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class WslStorageService
    {
        static async Task<IList<WslDrive>> PlatformGetDrives()
        {
            try
            {
                var drives = new List<WslDrive>();
                var wsl = await StorageFolder.GetFolderFromPathAsync(Prefix.WSL);
                var distros = await wsl.GetFoldersAsync();
                foreach (var item in distros)
                {
                    var drive = await StorageService.GetDrive(StorageType.WSL, item);
                    if (drive is WslDrive wslDrive)
                    {
                        drives.Add(wslDrive);
                    }
                }
                return drives;
            }
            catch (Exception)
            {
                return new List<WslDrive>();
            }
        }

        static async Task<IList<StorageItemBase>> PlatformGetItems(IWslDirectory directory)
        {
            try
            {
                var items = new List<StorageItemBase>();

                if (directory.Context != null && directory.Context.Context is StorageFolder folder)
                {
                    var portableItems = await folder.GetItemsAsync();

                    foreach (var item in portableItems)
                    {
                        if (item.IsOfType(StorageItemTypes.File))
                        {
                            var i = await StorageService.GetFile(StorageType.WSL, item as StorageFile);
                            if (i is WslFile f)
                            {
                                items.Add(f);
                            }
                        }
                        else if (item.IsOfType(StorageItemTypes.Folder))
                        {
                            var i = await StorageService.GetFolder(StorageType.WSL, item as StorageFolder);
                            if (i is WslFolder f)
                            {
                                items.Add(f);
                            }
                        }
                    }
                }

                return items;
            }
            catch (Exception)
            {
                return new List<StorageItemBase>();
            }
        }

        static async Task<WslDrive> PlatformGetDrive(string path)
        {
            try
            {
                var sf = await StorageFolder.GetFolderFromPathAsync(path);
                var drive = await StorageService.GetDrive(StorageType.WSL, sf);
                return drive as WslDrive;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static async Task<WslFolder> PlatformGetFolder(string path)
        {
            try
            {
                var sf = await StorageFolder.GetFolderFromPathAsync(path);
                var drive = await StorageService.GetFolder(StorageType.WSL, sf);
                return drive as WslFolder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static async Task<WslFile> PlatformGetFile(string path)
        {
            try
            {
                var sf = await StorageFile.GetFileFromPathAsync(path);
                var drive = await StorageService.GetFile(StorageType.WSL, sf);
                return drive as WslFile;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
