using System;
using JumpPoint.Platform.Items.CloudStorage;
using SQLite;

namespace JumpPoint.Platform.Items.OpenDrive
{
    public class OpenDriveAccount : CloudAccount
    {
        public OpenDriveAccount() : base(CloudStorageProvider.OpenDrive)
        {
        }

        private string _password;

        [NotNull]
        public string Password
        {
            get { return _password ?? (_password = string.Empty); }
            set { Set(ref _password, value); }
        }

        private string _sessionId;

        [NotNull]
        public string SessionId
        {
            get { return _sessionId ?? (_sessionId = string.Empty); }
            set { Set(ref _sessionId, value); }
        }

    }
}
