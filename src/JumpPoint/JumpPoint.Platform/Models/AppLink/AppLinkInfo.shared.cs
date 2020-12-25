using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models
{
    public class AppLinkInfo : ObservableObject
    {

        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private string _path;

        [NotNull, Collation("NOCASE"), Unique]
        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private string _displayName;

        [NotNull, Collation("NOCASE"), Unique]
        public string DisplayName
        {
            get { return _displayName; }
            set { Set(ref _displayName, value); }
        }

        private string _appName;

        [Collation("NOCASE")]
        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _identifier;

        [Collation("NOCASE")]
        public string Identifier
        {
            get { return _identifier; }
            set { Set(ref _identifier, value); }
        }

        private byte[] _logo;

        public byte[] Logo
        {
            get { return _logo; }
            set { Set(ref _logo, value); }
        }

        private string _background;

        [NotNull]
        public string Background
        {
            get { return _background ?? (_background = "#00FFFFFF"); }
            set { Set(ref _background, value); }
        }

        private string _inputKeys;

        [NotNull]
        public string InputKeys
        {
            get { return _inputKeys ?? (_inputKeys = "[]"); }
            set { Set(ref _inputKeys, value); }
        }

        private AppLinkLaunchTypes _launchTypes;

        [NotNull]
        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set { Set(ref _launchTypes, value); }
        }

    }

    [Flags, StoreAsText]
    public enum AppLinkLaunchTypes
    {
        None = 0,
        Uri = 1,
        UriForResults = 2,
        All = Uri | UriForResults
    }
}
