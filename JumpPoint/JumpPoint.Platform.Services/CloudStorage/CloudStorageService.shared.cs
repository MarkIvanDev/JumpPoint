using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.OpenDrive;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storj;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services.OneDrive;
using JumpPoint.Platform.Services.OpenDrive;
using JumpPoint.Platform.Services.Storj;
using NittyGritty.Utilities;

namespace JumpPoint.Platform.Services
{
    public static partial class CloudStorageService
    {

        public static async Task Initialize()
        {
            await OneDriveService.Initialize();
            await StorjService.Initialize();
            await OpenDriveService.Initialize();
        }

        public static async Task<IList<CloudAccount>> GetAccounts()
        {
            var accounts = new List<CloudAccount>();
            accounts.AddRange(await GetAccounts(CloudStorageProvider.OneDrive));
            accounts.AddRange(await GetAccounts(CloudStorageProvider.Storj));
            accounts.AddRange(await GetAccounts(CloudStorageProvider.OpenDrive));
            return accounts;
        }

        public static async Task<IList<CloudAccount>> GetAccounts(CloudStorageProvider provider)
        {
            var accounts = new List<CloudAccount>();
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    accounts.AddRange(await OneDriveService.GetAccounts());
                    break;

                case CloudStorageProvider.Storj:
                    accounts.AddRange(await StorjService.GetAccounts());
                    break;

                case CloudStorageProvider.OpenDrive:
                    accounts.AddRange(await OpenDriveService.GetAccounts());
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }
            return accounts;
        }

        public static bool TryGetAccountProperties(CloudStorageProvider provider, out IList<CloudAccountProperty> properties)
        {
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    properties = null;
                    return false;

                case CloudStorageProvider.Storj:
                    properties = new List<CloudAccountProperty>
                    {
                        new CloudAccountProperty("Access Grant", true, true)
                    };
                    return true;

                case CloudStorageProvider.OpenDrive:
                    properties = new List<CloudAccountProperty>
                    {
                        new CloudAccountProperty("Password", true, true)
                    };
                    return true;

                case CloudStorageProvider.Unknown:
                default:
                    properties = null;
                    return false;
            }
        }

        public static async Task<CloudAccount> AddAccount(CloudStorageProvider provider, IDictionary<string, string> data = null)
        {
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.AddAccount();

                case CloudStorageProvider.Storj when data != null && data.ContainsKey("Name") && data.ContainsKey("Email") && data.ContainsKey("Access Grant"):
                    return await StorjService.AddAccount(data["Name"], data["Email"], data["Access Grant"]);

                case CloudStorageProvider.OpenDrive when data != null && data.ContainsKey("Name") && data.ContainsKey("Email") && data.ContainsKey("Password"):
                    return await OpenDriveService.AddAccount(data["Name"], data["Email"], data["Password"]);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<string> RenameAccount(CloudAccount account, string newName)
        {
            switch (account.Provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.RenameAccount((OneDriveAccount)account, newName);

                case CloudStorageProvider.Storj:
                    return await StorjService.RenameAccount((StorjAccount)account, newName);

                case CloudStorageProvider.OpenDrive:
                    return await OpenDriveService.RenameAccount((OpenDriveAccount)account, newName);

                case CloudStorageProvider.Unknown:
                default:
                    return account.Name;
            }
        }

        public static async Task RemoveAccount(CloudAccount account)
        {
            switch (account.Provider)
            {
                case CloudStorageProvider.OneDrive:
                    await OneDriveService.RemoveAccount((OneDriveAccount)account);
                    break;

                case CloudStorageProvider.Storj:
                    await StorjService.RemoveAccount((StorjAccount)account);
                    break;

                case CloudStorageProvider.OpenDrive:
                    await OpenDriveService.RemoveAccount((OpenDriveAccount)account);
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }
        }

        public static CloudStorageProvider GetProvider(string path)
        {
            if (path.GetPathKind() == PathKind.Cloud)
            {
                var crumbs = path.GetBreadcrumbs();
                var providerCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                if (providerCrumb != null)
                {
                    var provider = CodeHelper.InvokeOrDefault(() => providerCrumb.DisplayName.DehumanizeTo<CloudStorageProvider>(), CloudStorageProvider.Unknown);
                    return provider;
                }
            }
            
            return CloudStorageProvider.Unknown;
        }

        public static async Task<IList<CloudDrive>> GetDrives()
        {
            var drives = new List<CloudDrive>();
            drives.AddRange(await GetDrives(CloudStorageProvider.OneDrive));
            drives.AddRange(await GetDrives(CloudStorageProvider.Storj));
            drives.AddRange(await GetDrives(CloudStorageProvider.OpenDrive));
            return drives;
        }

        public static async Task<IList<CloudDrive>> GetDrives(CloudStorageProvider provider)
        {
            var drives = new List<CloudDrive>();
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    drives.AddRange(await OneDriveService.GetDrives());
                    break;

                case CloudStorageProvider.Storj:
                    drives.AddRange(await StorjService.GetDrives());
                    break;

                case CloudStorageProvider.OpenDrive:
                    drives.AddRange(await OpenDriveService.GetDrives());
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }

            return drives;
        }

        public static async Task<IList<StorageItemBase>> GetItems(ICloudDirectory directory)
        {
            var items = new List<StorageItemBase>();
            switch (directory.Provider)
            {
                case CloudStorageProvider.OneDrive when directory is OneDriveDrive odd:
                    items.AddRange(await OneDriveService.GetItems(odd));
                    break;
                case CloudStorageProvider.OneDrive when directory is OneDriveFolder odf:
                    items.AddRange(await OneDriveService.GetItems(odf));
                    break;

                case CloudStorageProvider.Storj when directory is StorjDrive sjd:
                    items.AddRange(await StorjService.GetItems(sjd));
                    break;
                case CloudStorageProvider.Storj when directory is StorjFolder sjf:
                    items.AddRange(await StorjService.GetItems(sjf));
                    break;

                case CloudStorageProvider.OpenDrive when directory is OpenDriveDrive opd:
                    items.AddRange(await OpenDriveService.GetItems(opd));
                    break;
                case CloudStorageProvider.OpenDrive when directory is OpenDriveFolder opf:
                    items.AddRange(await OpenDriveService.GetItems(opf));
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }

            return items;
        }

        public static async Task<CloudDrive> GetDrive(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetDrive(path);

                case CloudStorageProvider.Storj:
                    return await StorjService.GetDrive(path);

                case CloudStorageProvider.OpenDrive:
                    return await OpenDriveService.GetDrive(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<CloudFolder> GetFolder(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetFolder(path);

                case CloudStorageProvider.Storj:
                    return await StorjService.GetFolder(path);

                case CloudStorageProvider.OpenDrive:
                    return await OpenDriveService.GetFolder(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<CloudFile> GetFile(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetFile(path);

                case CloudStorageProvider.Storj:
                    return await StorjService.GetFile(path);

                case CloudStorageProvider.OpenDrive:
                    return await OpenDriveService.GetFile(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task OpenFile(FileBase file)
        {
            try
            {
                switch (file)
                {
                    case OneDriveFile odFile:
                        await Xamarin.Essentials.Browser.OpenAsync(odFile.GraphItem.WebUrl);
                        break;

                    case StorjFile sjFile:
                        await Xamarin.Essentials.Browser.OpenAsync(StorjService.GetUrl(sjFile));
                        break;

                    case OpenDriveFile opFile:
                        await Xamarin.Essentials.Browser.OpenAsync(OpenDriveService.GetUrl(opFile));
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        public static async Task<FileBase> DownloadFile(FileBase file)
        {
            switch (file)
            {
                case OneDriveFile odFile:
                    var odContent = await OneDriveService.GetContent(odFile);
                    if (odContent != null)
                    {
                        return await StorageService.DownloadFile(file.Name, odContent);
                    }
                    break;

                case StorjFile sjFile:
                    var sjContent = await StorjService.GetContent(sjFile);
                    if (sjContent != null)
                    {
                        return await StorageService.DownloadFile(file.Name, sjContent);
                    }
                    break;

                case OpenDriveFile opFile:
                    var opContent = await OpenDriveService.GetContent(opFile);
                    if (opContent != null)
                    {
                        return await StorageService.DownloadFile(file.Name, opContent);
                    }
                    break;

                default:
                    break;
            }
            return null;
        }

        public static async Task<CloudFolder> CreateFolder(DirectoryBase directory, string name, CreateOption option)
        {
            switch (directory)
            {
                case OneDriveDrive odDrive:
                    return await OneDriveService.CreateFolder(odDrive, name, option);

                case OneDriveFolder odFolder:
                    return await OneDriveService.CreateFolder(odFolder, name, option);

                case StorjDrive sjDrive:
                    return null;
                case StorjFolder sjFolder:
                    return await StorjService.CreateFolder(sjFolder, name, option);

                case OpenDriveDrive opDrive:
                    return await OpenDriveService.CreateFolder(opDrive, name, option);
                case OpenDriveFolder opFolder:
                    return await OpenDriveService.CreateFolder(opFolder, name, option);

                default:
                    return null;
            }
        }

        public static async Task<CloudFile> CreateFile(DirectoryBase directory, string name, CreateOption option, byte[] content)
        {
            switch (directory)
            {
                case OneDriveDrive odDrive:
                    return await OneDriveService.CreateFile(odDrive, name, option, content);

                case OneDriveFolder odFolder:
                    return await OneDriveService.CreateFile(odFolder, name, option, content);

                case StorjDrive sjDrive:
                    return null;
                case StorjFolder sjFolder:
                    return await StorjService.CreateFile(sjFolder, name, option, content);

                case OpenDriveDrive opDrive:
                    return await OpenDriveService.CreateFile(opDrive, name, option, content);
                case OpenDriveFolder opFolder:
                    return await OpenDriveService.CreateFile(opFolder, name, option, content);

                default:
                    return null;
            }
        }

        public static async Task<string> Rename(StorageItemBase item, string name, RenameOption option)
        {
            switch (item)
            {
                case OneDriveFolder odFolder:
                    return await OneDriveService.Rename(odFolder, name, option);

                case OneDriveFile odFile:
                    return await OneDriveService.Rename(odFile, name, option);

                case StorjFolder sjFolder:
                    return await StorjService.Rename(sjFolder, name, option);

                case StorjFile sjFile:
                    return await StorjService.Rename(sjFile, name, option);

                case OpenDriveFolder opFolder:
                    return await OpenDriveService.Rename(opFolder, name, option);

                case OpenDriveFile opFile:
                    return await OpenDriveService.Rename(opFile, name, option);

                default:
                    return string.Empty;
            }
        }

        public static async Task Delete(IList<StorageItemBase> items)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case OneDriveFolder odFolder:
                        await OneDriveService.Delete(odFolder);
                        break;

                    case OneDriveFile odFile:
                        await OneDriveService.Delete(odFile);
                        break;

                    case StorjFolder sjFolder:
                        await StorjService.Delete(sjFolder);
                        break;

                    case StorjFile sjFile:
                        await StorjService.Delete(sjFile);
                        break;

                    case OpenDriveFolder opFolder:
                        await OpenDriveService.Delete(opFolder);
                        break;

                    case OpenDriveFile opFile:
                        await OpenDriveService.Delete(opFile);
                        break;

                    default:
                        break;
                }
            }
        }

        #region Commands
        public static bool CanCreateFolder(DirectoryBase directory)
        {
            return !(directory is StorjDrive);
        }

        public static bool CanCreateFile(DirectoryBase directory)
        {
            return !(directory is StorjDrive);
        }

        public static bool CanCreateItem(DirectoryBase directory)
        {
            return !(directory is StorjDrive);
        }
        #endregion
    }
}
