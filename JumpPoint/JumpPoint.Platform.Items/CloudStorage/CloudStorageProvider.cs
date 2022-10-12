using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public enum CloudStorageProvider
    {
        Unknown = 0,
        [Description("OneDrive")]
        OneDrive = 1,
        [Description("Storj")]
        Storj = 2,
        [Description("OpenDrive")]
        OpenDrive = 3,
    }

    public static class CloudStorageProviderExtensions
    {
        public static DriveTemplate ToDriveTemplate(this CloudStorageProvider provider)
        {
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return DriveTemplate.OneDrive;

                case CloudStorageProvider.Storj:
                    return DriveTemplate.Storj;

                case CloudStorageProvider.OpenDrive:
                    return DriveTemplate.OpenDrive;

                case CloudStorageProvider.Unknown:
                default:
                    return DriveTemplate.Cloud;
            }
        }
    }
}
