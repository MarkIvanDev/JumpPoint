using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Items.OneDrive
{
    public class OneDriveAccount : CloudAccount
    {
        public OneDriveAccount() : base(CloudStorageProvider.OneDrive)
        {
        }

        private string _identifier;

        [Unique, NotNull]
        public string Identifier
        {
            get { return _identifier ?? (_identifier = string.Empty); }
            set { Set(ref _identifier, value); }
        }

    }
}
