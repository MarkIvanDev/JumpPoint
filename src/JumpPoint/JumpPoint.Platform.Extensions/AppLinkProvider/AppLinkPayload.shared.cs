using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;

namespace JumpPoint.Platform.Extensions
{
    public class AppLinkPayload
    {

        public string Link { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AppName { get; set; }

        public string AppId { get; set; }

        public byte[] Logo { get; set; }

        public string Background { get; set; }

        public int LaunchTypes { get; set; }

        public string[] QueryKeys { get; set; }

        public string[] InputKeys { get; set; }

        public AppLinkInfo ToAppLinkInfo()
        {
            return string.IsNullOrWhiteSpace(Link) ? null :
                new AppLinkInfo
                {
                    Link = Link,
                    Name = Name,
                    Description = Name,
                    AppName = AppName,
                    AppId = AppId,
                    Logo = Logo,
                    Background = Background,
                    QueryKeys = QueryKeys,
                    InputKeys = InputKeys.ToInputKeys(),
                    LaunchTypes = (AppLinkLaunchTypes)LaunchTypes,
                };
        }

    }
}
