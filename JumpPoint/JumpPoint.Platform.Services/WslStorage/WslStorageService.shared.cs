using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.WslStorage;
using JumpPoint.Platform.Models.Extensions;

namespace JumpPoint.Platform.Services
{
    public static partial class WslStorageService
    {
        public static WslDistro GetDistro(string path)
        {
            try
            {
                var crumbs = path.GetBreadcrumbs();
                var driveCrumb = crumbs.ElementAtOrDefault(1);
                switch (driveCrumb?.DisplayName)
                {
                    case string name when name.IndexOf("ubuntu", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Ubuntu;

                    case string name when name.IndexOf("debian", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Debian;

                    case string name when name.IndexOf("kali", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Kali;

                    case string name when name.IndexOf("opensuse", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.OpenSuse;

                    case string name when name.IndexOf("sles", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.SLES;

                    case string name when name.IndexOf("fedora remix", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.FedoraRemix;

                    case string name when name.IndexOf("pengwin", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Pengwin;

                    case string name when name.IndexOf("oracle", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Oracle;

                    case string name when name.IndexOf("alma", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Alma;

                    case string name when name.IndexOf("alpine", StringComparison.OrdinalIgnoreCase) >= 0:
                        return WslDistro.Alpine;

                    default:
                        return WslDistro.Unknown;
                }
            }
            catch (Exception)
            {
                return WslDistro.Unknown;
            }
        }

        public static Task<IList<WslDrive>> GetDrives()
            => PlatformGetDrives();

        public static Task<IList<StorageItemBase>> GetItems(IWslDirectory directory)
            => PlatformGetItems(directory);

        public static Task<WslDrive> GetDrive(string path)
            => PlatformGetDrive(path);

        public static Task<WslFolder> GetFolder(string path)
            => PlatformGetFolder(path);

        public static Task<WslFile> GetFile(string path)
            => PlatformGetFile(path);
    }
}
