using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;
using NittyGritty.Collections;
using SQLite;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public abstract class CloudAccount : ObservableObject
    {

        public CloudAccount(CloudStorageProvider provider)
        {
            Provider = provider;
        }

        [Ignore]
        public CloudStorageProvider Provider { get; }

        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private string _name;

        [Unique, NotNull, Collation("NOCASE")]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

    }

    public class CloudAccountGroup : ObservableGroup<CloudStorageProvider, CloudAccount>
    {
        public CloudAccountGroup(CloudStorageProvider key) : base(key)
        {
        }

        public CloudAccountGroup(CloudStorageProvider key, IList<CloudAccount> items) : base(key, items)
        {
        }
    }

}
