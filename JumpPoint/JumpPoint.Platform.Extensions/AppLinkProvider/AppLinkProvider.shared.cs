using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;

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

    public static class AppLinkPayloadExtensions
    {
        public static AppLinkInfo ToAppLinkInfo(this AppLinkPayload payload)
        {
            return string.IsNullOrWhiteSpace(payload.Link) ? null :
                new AppLinkInfo
                {
                    Link = payload.Link,
                    Name = payload.Name,
                    Description = payload.Description,
                    AppName = payload.AppName,
                    AppId = payload.AppId,
                    Logo = payload.Logo,
                    Background = payload.Background,
                    QueryKeys = payload.QueryKeys,
                    InputKeys = payload.InputKeys.ToInputKeys(),
                    LaunchTypes = (AppLinkLaunchTypes)payload.LaunchTypes,
                };
        }
    }
}
