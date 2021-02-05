using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public class CloudAccount : ObservableObject
    {

        private CloudStorageService _service;

        public CloudStorageService Service
        {
            get { return _service; }
            set { Set(ref _service, value); }
        }

        private string _name;

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
}
