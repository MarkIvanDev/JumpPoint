using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Humanizer;
using NittyGritty;
using NittyGritty.Extensions;

namespace JumpPoint.Platform.Models
{
    public class PathInfo : ObservableObject
    {
        public PathInfo()
        {
            Type = PathType.Unknown;
            Path = string.Empty;
        }

        private PathType _type;

        public PathType Type
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

        private object _parameter;

        public object Parameter
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
}
