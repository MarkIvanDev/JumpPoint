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

        private Collection<Breadcrumb> GenerateBreadcrumbs(string path)
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

                        breadcrumbs.AddRange(GetStorageCrumbs(path));
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

                        breadcrumbs.AddRange(GetStorageCrumbs(path));
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

        public static IList<Breadcrumb> GetStorageCrumbs(string path)
        {
            var workingPath = path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

            var prefix = string.Empty;
            if (workingPath.StartsWith(@"\\?\")) // Unmounted storage
            {
                prefix = @"\\?\";
                workingPath = workingPath.Remove(0, 4);
            }
            else if (workingPath.StartsWith(@"\\")) // Network path
            {
                prefix = @"\\";
                workingPath = workingPath.Remove(0, 2);
            }

            var crumbs = new List<Breadcrumb>();
            workingPath = workingPath.EndsWith("\\") ? workingPath : $@"{workingPath}\";
            var parent = System.IO.Path.GetDirectoryName(workingPath);
            if (!string.IsNullOrEmpty(parent))
            {
                while (!string.IsNullOrEmpty(parent))
                {
                    var displayName = System.IO.Path.GetFileName(parent);
                    displayName = string.IsNullOrEmpty(displayName) ? parent : displayName;

                    crumbs.Insert(0, new Breadcrumb()
                    {
                        PathType = string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(parent)) ? PathType.Drive : PathType.Folder,
                        Path = $"{prefix}{parent}",
                        DisplayName = displayName
                    });

                    parent = System.IO.Path.GetDirectoryName(parent);
                }
            }
            else
            {
                crumbs.Insert(0, new Breadcrumb()
                {
                    PathType = PathType.Drive,
                    Path = $"{prefix}{workingPath}",
                    DisplayName = workingPath
                });
            }
            return crumbs;
        }

    }
}
