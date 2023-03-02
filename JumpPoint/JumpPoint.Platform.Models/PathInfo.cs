using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Humanizer;
using Newtonsoft.Json;
using NittyGritty;
using NittyGritty.Extensions;

namespace JumpPoint.Platform.Models
{
    public class PathInfo : ObservableObject
    {
        public PathInfo()
        {
            Type = AppPath.Unknown;
            Path = string.Empty;
        }

        private AppPath _type;

        public AppPath Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private Collection<Breadcrumb> _breadcrumbs;

        public Collection<Breadcrumb> Breadcrumbs
        {
            get { return _breadcrumbs ?? (_breadcrumbs = new Collection<Breadcrumb>()); }
            set { Set(ref _breadcrumbs, value); }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName ?? (_displayName = string.Empty); }
            set { Set(ref _displayName, value); }
        }

        private TabParameter _parameter;

        public TabParameter Parameter
        {
            get { return _parameter; }
            set { Set(ref _parameter, value); }
        }

        private object _tag;

        public object Tag
        {
            get { return _tag; }
            set { Set(ref _tag, value); }
        }

    }

    public class TabParameter
    {
        public string TabKey { get; set; }

        public string Parameter { get; set; }

        public static TabParameter FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TabParameter>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
