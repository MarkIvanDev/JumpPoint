using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.WslStorage;

namespace JumpPoint.Platform.Items.Templates
{
    public enum DriveTemplate
    {
        Unknown = 0,
        Local = 1,
        Optical = 2,
        Network = 3,
        Removable = 4,
        Cloud = 5,
        WSL = 6,

        // Local Kinds
        System = 11,
        HDD = 12,
        SSD = 13,
        
        // Optical Kinds
        CD = 31,
        CDR = 32,
        CDRW = 33,

        DVD = 34,
        DVDRAM = 35,
        DVDPLUSR = 36,
        DVDPLUSRDL = 37,
        DVDPLUSRW = 38,
        DVDPLUSRWDL = 39,
        DVDDASHR = 40,
        DVDDASHRDL = 41,
        DVDDASHRW = 42,
        DVDDASHRWDL = 43,
        HDDVD = 44,
        HDDVDRAM = 45,
        HDDVDR = 46,

        BD = 47,
        BDR = 48,
        BDRE = 49,

        // Removable Kinds
        USB = 61,
        SD = 62,
        MicroSD = 63,
        Phone = 64,
        Camera = 65,

        // Cloud Kinds
        OneDrive = 501,
        Storj = 502,
        OpenDrive = 503,

        // Wsl Kinds
        Ubuntu = 1001,
        Debian = 1002,
        Kali = 1003,
        OpenSuse = 1004,
        SLES = 1005,
        FedoraRemix = 1006,
        Pengwin = 1007,
        Oracle = 1008,
        Alma = 1009,
        Alpine = 1010,
    }

    public static class DriveTemplateExtensions
    {
        public static CloudStorageProvider ToCloudStorageProvider(this DriveTemplate driveTemplate)
        {
            switch (driveTemplate)
            {
                case DriveTemplate.OneDrive:
                    return CloudStorageProvider.OneDrive;

                case DriveTemplate.Storj:
                    return CloudStorageProvider.Storj;

                case DriveTemplate.OpenDrive:
                    return CloudStorageProvider.OpenDrive;

                case DriveTemplate.Cloud:
                default:
                    return CloudStorageProvider.Unknown;
            }
        }

        public static WslDistro ToWslDistro(this DriveTemplate driveTemplate)
        {
            switch (driveTemplate)
            {
                case DriveTemplate.Ubuntu:
                    return WslDistro.Ubuntu;

                case DriveTemplate.Debian:
                    return WslDistro.Debian;

                case DriveTemplate.Kali:
                    return WslDistro.Kali;

                case DriveTemplate.OpenSuse:
                    return WslDistro.OpenSuse;

                case DriveTemplate.SLES:
                    return WslDistro.SLES;

                case DriveTemplate.FedoraRemix:
                    return WslDistro.FedoraRemix;

                case DriveTemplate.Pengwin:
                    return WslDistro.Pengwin;

                case DriveTemplate.Oracle:
                    return WslDistro.Oracle;

                case DriveTemplate.Alma:
                    return WslDistro.Alma;

                case DriveTemplate.Alpine:
                    return WslDistro.Alpine;

                case DriveTemplate.WSL:
                default:
                    return WslDistro.Unknown;
            }
        }
    }
}
