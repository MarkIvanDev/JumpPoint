using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Items.OneDrive
{
    public class OneDriveAccount : ObservableObject
    {

        private string _identifier;

        [Unique, NotNull, Indexed]
        public string Identifier
        {
            get { return _identifier ?? (_identifier = string.Empty); }
            set { Set(ref _identifier, value); }
        }

        private string _name;

        [Unique, NotNull, Indexed, Collation("NOCASE")]
        public string Name
        {
            get { return _name ?? (_name = string.Empty); }
            set { Set(ref _name, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

    }
}
