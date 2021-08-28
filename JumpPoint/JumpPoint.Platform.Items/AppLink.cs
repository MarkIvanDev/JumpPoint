using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Humanizer;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Items
{
    public class AppLink : JumpPointItem
    {
        public AppLink(string name, string description, string link, string appName, string appId, Stream logo, Color background, IList<ValueInfo> query, IList<ValueInfo> inputKeys, AppLinkLaunchTypes launchTypes) :
            base(JumpPointItemType.AppLink)
        {
            Name = name;
            Description = description;
            Link = link;
            Path = $@"applink:\{name}";
            DisplayType = $"{appName} Link";
            AppName = appName;
            AppId = appId;
            Logo = logo;
            Background = background;
            Query = new Collection<ValueInfo>(query);
            InputData = new Collection<ValueInfo>(inputKeys);
            LaunchTypes = launchTypes;
        }

        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                DisplayName = base.Name;
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private string _link;

        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private string _appName;

        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _appId;

        public string AppId
        {
            get { return _appId; }
            set { Set(ref _appId, value); }
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

        private Collection<ValueInfo> _query;

        public Collection<ValueInfo> Query
        {
            get { return _query ?? (_query = new Collection<ValueInfo>()); }
            set { Set(ref _query, value); }
        }

        private Collection<ValueInfo> _inputData;

        public Collection<ValueInfo> InputData
        {
            get { return _inputData ?? (_inputData = new Collection<ValueInfo>()); }
            set { Set(ref _inputData, value); }
        }

        private AppLinkLaunchTypes _launchTypes;

        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set { Set(ref _launchTypes, value); }
        }
    }

    [DataContract]
    public class ValueInfo : ObservableObject
    {

        private string _key;

        [DataMember]
        public string Key
        {
            get { return _key; }
            set { Set(ref _key, value); }
        }

        private DataType _dataType;

        [DataMember]
        public DataType DataType
        {
            get { return _dataType; }
            set { Set(ref _dataType, value); }
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
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
