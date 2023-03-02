using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Extensions
{
    public class NewItem : ExtensionBase
    {
        public NewItem() : base(nameof(NewItem))
        {
        }

        private string _link;

        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private string _service;

        public string Service
        {
            get { return _service; }
            set { Set(ref _service, value); }
        }

    }
}
