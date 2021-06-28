using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Extensions.AppLinkProviders
{
    public class AppLinkPayload
    {
        public string Link { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AppName { get; set; }

        public string AppId { get; set; }

        public byte[] Logo { get; set; }

        public Uri LogoUri { get; set; }

        public string Background { get; set; }

        public int LaunchTypes { get; set; }

        public string[] QueryKeys { get; set; }

        public string[] InputKeys { get; set; }
    }
}
