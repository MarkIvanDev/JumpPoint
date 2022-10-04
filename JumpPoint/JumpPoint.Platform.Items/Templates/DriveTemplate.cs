using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
