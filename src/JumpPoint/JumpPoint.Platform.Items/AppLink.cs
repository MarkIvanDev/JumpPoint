using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NittyGritty;
using SQLite;

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

    public class ValueInfo : ObservableObject
    {

        private string _key;

        public string Key
        {
            get { return _key; }
            set { Set(ref _key, value); }
        }

        private DataType _dataType;

        public DataType DataType
        {
            get { return _dataType; }
            set { Set(ref _dataType, value); }
        }

        private object _value;

        [JsonIgnore]
        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataType
    {
        String = 0,
        Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        UInt8 = 4,
        UInt16 = 5,
        UInt32 = 6,
        UInt64 = 7,
        Single = 8,
        Double = 9,
        Boolean = 10,
        Char = 11,
        DateTime = 12,
        TimeSpan = 13,
        Guid = 14,
        ByteArray = 15,
        Point = 16,
        Size = 17,
        Rect = 18,
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
