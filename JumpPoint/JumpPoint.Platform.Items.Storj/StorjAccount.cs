using System;
using JumpPoint.Platform.Items.CloudStorage;
using SQLite;

namespace JumpPoint.Platform.Items.Storj
{
    public class StorjAccount : CloudAccount
    {
        public StorjAccount() : base(CloudStorageProvider.Storj)
        {
        }

        private string _accessGrant;

        [NotNull]
        public string AccessGrant
        {
            get { return _accessGrant ?? (_accessGrant = string.Empty); }
            set { Set(ref _accessGrant, value); }
        }

    }
}
