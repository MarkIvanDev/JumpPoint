using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Models;

namespace JumpPoint.Platform.Items
{
    public class AppLink : JumpPointItem
    {
        public AppLink(string name, string path, string appName, string identifier, Stream logo, Color background, IList<ValueInfo> inputKeys, AppLinkLaunchTypes launchTypes) :
            base(JumpPointItemType.AppLink)
        {
            Name = name;
            DisplayName = name;
            Path = path;
            DisplayType = JumpPointItemType.AppLink.Humanize();
            AppName = appName;
            Identifier = identifier;
            Logo = logo;
            Background = background;
            InputKeys = new Collection<ValueInfo>(inputKeys);
            LaunchTypes = launchTypes;
        }

        private string _appName;

        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _identifier;

        public string Identifier
        {
            get { return _identifier; }
            set { Set(ref _identifier, value); }
        }

        private Stream _logo;

        public Stream Logo
        {
            get { return _logo; }
            set { Set(ref _logo, value); }
        }

        private Color _background;

        public Color Background
        {
            get { return _background; }
            set { Set(ref _background, value); }
        }

        private Collection<ValueInfo> _inputKeys;

        public Collection<ValueInfo> InputKeys
        {
            get { return _inputKeys ?? (_inputKeys = new Collection<ValueInfo>()); }
            set { Set(ref _inputKeys, value); }
        }

        private AppLinkLaunchTypes _launchTypes;

        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set { Set(ref _launchTypes, value); }
        }
    }
}
