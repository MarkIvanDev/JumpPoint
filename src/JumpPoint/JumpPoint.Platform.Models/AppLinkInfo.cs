using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Items;
using Newtonsoft.Json;
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

        private string _link;

        [NotNull, Collation("NOCASE"), Unique]
        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private string _name;

        [NotNull, Collation("NOCASE"), Unique]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private string _appName;

        [Collation("NOCASE")]
        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _appId;

        [Collation("NOCASE")]
        public string AppId
        {
            get { return _appId; }
            set { Set(ref _appId, value); }
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

        private string[] _queryKeys;

        [Ignore]
        public string[] QueryKeys
        {
            get { return _queryKeys ?? (_queryKeys = Array.Empty<string>()); }
            set { Set(ref _queryKeys, value); }
        }

        public string QueryKeysJson
        {
            get { return JsonConvert.SerializeObject(QueryKeys); }
            set { QueryKeys = JsonConvert.DeserializeObject<string[]>(value ?? "[]"); }
        }

        private Collection<ValueInfo> _inputKeys;

        [Ignore]
        public Collection<ValueInfo> InputKeys
        {
            get { return _inputKeys ?? (_inputKeys = new Collection<ValueInfo>()); }
            set { Set(ref _inputKeys, value); }
        }

        [NotNull]
        public string InputKeysJson
        {
            get { return JsonConvert.SerializeObject(InputKeys); }
            set { InputKeys = JsonConvert.DeserializeObject<Collection<ValueInfo>>(value ?? "[]"); }
        }

        private AppLinkLaunchTypes _launchTypes;

        [NotNull]
        public AppLinkLaunchTypes LaunchTypes
        {
            get { return _launchTypes; }
            set { Set(ref _launchTypes, value); }
        }

    }

}
