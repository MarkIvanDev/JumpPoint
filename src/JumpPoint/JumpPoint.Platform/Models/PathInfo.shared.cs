using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Utilities;
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
            private set { Set(ref _breadcrumbs, value); }
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

        public void Place(PathType type, string path, object parameter)
        {
            Type = type;
            Path = path;
            Parameter = parameter;
            Breadcrumbs = GenerateBreadcrumbs(path);
            DisplayName = Breadcrumbs.LastOrDefault()?.DisplayName ?? string.Empty;
        }

        public Collection<Breadcrumb> GenerateBreadcrumbs(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null, empty or whitespace", nameof(path));
            }

            var breadcrumbs = new Collection<Breadcrumb>();

            switch (Type)
            {
                case PathType.Folder:
                    {
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = PathType.Drives,
                            Path = nameof(PathType.Drives),
                            DisplayName = nameof(PathType.Drives)
                        });

                        breadcrumbs.AddRange(PathUtilities.GetCrumbs(path));

                        //var dirInfo = new DirectoryInfo(path);
                        //while (dirInfo != null)
                        //{
                        //    var type = dirInfo.Parent == null ? PathType.Drive : PathType.Folder;
                        //    breadcrumbs.Insert(1, new Breadcrumb()
                        //    {
                        //        PathType = type,
                        //        Path = dirInfo.FullName,
                        //        DisplayName = dirInfo.Name
                        //    });
                        //    dirInfo = dirInfo.Parent;
                        //}
                        return breadcrumbs;
                    }
                case PathType.Drive:
                    {
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = PathType.Drives,
                            Path = nameof(PathType.Drives),
                            DisplayName = nameof(PathType.Drives)
                        });

                        breadcrumbs.AddRange(PathUtilities.GetCrumbs(path));
                        //breadcrumbs.Add(new Breadcrumb()
                        //{
                        //    PathType = Type,
                        //    Path = Path,
                        //    DisplayName = Path
                        //});
                        return breadcrumbs;
                    }
                case PathType.Workspace:
                    {
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = PathType.Workspaces,
                            Path = nameof(PathType.Workspaces),
                            DisplayName = nameof(PathType.Workspaces)
                        });
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = Type,
                            Path = Path,
                            DisplayName = Path
                        });
                        return breadcrumbs;
                    }
                case PathType.Library:
                    {
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = PathType.Libraries,
                            Path = nameof(PathType.Libraries),
                            DisplayName = nameof(PathType.Libraries)
                        });
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = Type,
                            Path = Path,
                            DisplayName = Path
                        });
                        return breadcrumbs;
                    }
                case PathType.Dashboard:
                case PathType.Workspaces:
                case PathType.Favorites:
                case PathType.Drives:
                case PathType.Libraries:
                case PathType.AppLinks:
                case PathType.SettingLinks:
                case PathType.Settings:
                    {
                        breadcrumbs.Add(new Breadcrumb()
                        {
                            PathType = Type,
                            Path = Type.Humanize(),
                            DisplayName = Type.Humanize()
                        });
                        return breadcrumbs;
                    }
                case PathType.Unknown:
                case PathType.File:
                default:
                    return breadcrumbs;
            }
        }
    
    }

    public class Breadcrumb : ObservableObject
    {

        private PathType _pathType;

        public PathType PathType
        {
            get { return _pathType; }
            set { Set(ref _pathType, value); }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { Set(ref _displayName, value); }
        }

    }
}
