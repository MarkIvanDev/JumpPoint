using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Extensions
{
    public class AppLinkProvider : ExtensionBase
    {
        public AppLinkProvider() : base(nameof(AppLinkProvider))
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
