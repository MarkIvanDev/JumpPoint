using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items.WslStorage
{
    public enum WslDistro
    {
        Unknown = 0,
        Ubuntu = 1,
        Debian = 2,
        Kali = 3,
        OpenSuse = 4,
        SLES = 5,
        FedoraRemix = 6,
        Pengwin = 7,
        Oracle = 8,
        Alma = 9,
        Alpine = 10,
    }

    public static class WslDistroExtensions
    {
        public static DriveTemplate ToDriveTemplate(this WslDistro distro)
        {
            switch (distro)
            {
                case WslDistro.Ubuntu:
                    return DriveTemplate.Ubuntu;

                case WslDistro.Debian:
                    return DriveTemplate.Debian;

                case WslDistro.Kali:
                    return DriveTemplate.Kali;

                case WslDistro.OpenSuse:
                    return DriveTemplate.OpenSuse;

                case WslDistro.SLES:
                    return DriveTemplate.SLES;

                case WslDistro.FedoraRemix:
                    return DriveTemplate.FedoraRemix;

                case WslDistro.Pengwin:
                    return DriveTemplate.Pengwin;

                case WslDistro.Oracle:
                    return DriveTemplate.Oracle;

                case WslDistro.Alma:
                    return DriveTemplate.Alma;

                case WslDistro.Alpine:
                    return DriveTemplate.Alpine;

                case WslDistro.Unknown:
                default:
                    return DriveTemplate.WSL;
            }
        }
    }
}
