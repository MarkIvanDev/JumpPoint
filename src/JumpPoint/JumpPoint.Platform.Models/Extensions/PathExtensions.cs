using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty.Extensions;

namespace JumpPoint.Platform.Models.Extensions
{
    public static class PathExtensions
    {
        public static string NormalizePath(this string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim();
        }

        public static PathKind GetPathKind(this string path)
        {
            var workingPath = path.NormalizePath();
            if (workingPath.StartsWith(Prefix.UNMOUNTED))
            {
                return PathKind.Unmounted;
            }
            else if (workingPath.StartsWith(Prefix.NETWORK))
            {
                return PathKind.Network;
            }
            else if (workingPath.StartsWith(Prefix.CLOUD, StringComparison.OrdinalIgnoreCase))
            {
                return PathKind.Cloud;
            }
            else if (workingPath.StartsWith(Prefix.WORKSPACE, StringComparison.OrdinalIgnoreCase))
            {
                return PathKind.Workspace;
            }
            else if (workingPath.Length >= 2 && workingPath[1] == Path.VolumeSeparatorChar)
            {
                return PathKind.Mounted;
            }
            else if (Enum.TryParse<PathType>(workingPath, true, out var appPath) && appPath != PathType.Unknown)
            {
                return PathKind.Local;
            }
            return PathKind.Unknown;
        }

        public static IList<Breadcrumb> GetBreadcrumbs(this string path)
        {
            var crumbs = new List<Breadcrumb>();

            var workingPath = path.NormalizePath();
            var pathKind = workingPath.GetPathKind();
            var prefix = GetPrefix(pathKind);
            workingPath = workingPath.Remove(0, prefix.Length).WithEnding(@"\");

            var parent = Path.GetDirectoryName(workingPath);
            if (!string.IsNullOrEmpty(parent))
            {
                while (!string.IsNullOrEmpty(parent))
                {
                    var displayName = Path.GetFileName(parent);
                    displayName = string.IsNullOrEmpty(displayName) ? parent : displayName;
                    crumbs.Insert(0, new Breadcrumb
                    {
                        DisplayName = displayName,
                        Path = $"{prefix}{parent}"
                    });

                    parent = Path.GetDirectoryName(parent);
                }
            }
            else
            {
                crumbs.Insert(0, new Breadcrumb
                {
                    DisplayName = workingPath.TrimEnd('\\'),
                    Path = $"{prefix}{workingPath}"
                });
            }

            switch (pathKind)
            {
                case PathKind.Local when Enum.TryParse<PathType>(workingPath, true, out var appPath):
                    if (crumbs.Count > 1)
                    {
                        crumbs.RemoveRange(1, crumbs.Count - 1);
                    }
                    foreach (var item in crumbs)
                    {
                        item.PathType = appPath;
                        item.Path = appPath.Humanize();
                        item.DisplayName = appPath.Humanize();
                    }
                    return crumbs;

                case PathKind.Mounted:
                case PathKind.Unmounted:
                case PathKind.Network:
                    for (int i = 0; i < crumbs.Count; i++)
                    {
                        crumbs[i].PathType = i == 0 ?
                            PathType.Drive :
                            PathType.Folder;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        PathType = PathType.Drives,
                        Path = nameof(PathType.Drives),
                        DisplayName = nameof(PathType.Drives)
                    });
                    return crumbs;

                case PathKind.Cloud:
                    for (int i = 0; i < crumbs.Count; i++)
                    {
                        if (i == 0)
                        {
                            crumbs[i].PathType = PathType.CloudStorage;
                            crumbs[i].DisplayName = Enum.TryParse<CloudStorageProvider>(crumbs[i].DisplayName, out var provider) ?
                                provider.Humanize() : CloudStorageProvider.Unknown.Humanize();
                        }
                        else
                        {
                            crumbs[i].PathType = i == 1 ? PathType.Drive : PathType.Folder;
                        }
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        PathType = PathType.CloudStorages,
                        Path = PathType.CloudStorages.Humanize(),
                        DisplayName = PathType.CloudStorages.Humanize()
                    });
                    return crumbs;

                case PathKind.Workspace:
                    if (crumbs.Count > 1)
                    {
                        crumbs.RemoveRange(1, crumbs.Count - 1);
                    }
                    foreach (var item in crumbs)
                    {
                        item.PathType = PathType.Workspace;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        PathType = PathType.Workspaces,
                        Path = nameof(PathType.Workspaces),
                        DisplayName = nameof(PathType.Workspaces)
                    });
                    return crumbs;

                case PathKind.Unknown:
                default:
                    return crumbs;
            }

        }

        private static string GetPrefix(PathKind pathKind)
        {
            switch (pathKind)
            {
                case PathKind.Unmounted:
                    return Prefix.UNMOUNTED;

                case PathKind.Network:
                    return Prefix.NETWORK;

                case PathKind.Cloud:
                    return Prefix.CLOUD;

                case PathKind.Workspace:
                    return Prefix.WORKSPACE;

                case PathKind.Local:
                case PathKind.Mounted:
                case PathKind.Unknown:
                default:
                    return string.Empty;
            }
        }

        public static void Place(this PathInfo pathInfo, string path, object parameter)
        {
            pathInfo.Breadcrumbs = new Collection<Breadcrumb>(path.GetBreadcrumbs());
            pathInfo.Parameter = parameter;
            var lastCrumb = pathInfo.Breadcrumbs.LastOrDefault();
            if (lastCrumb != null)
            {
                pathInfo.Type = lastCrumb.PathType;
                pathInfo.Path = lastCrumb.Path;
                pathInfo.DisplayName = lastCrumb.DisplayName;
            }
            else
            {
                pathInfo.Type = PathType.Unknown;
                pathInfo.Path = path;
                pathInfo.DisplayName = string.Empty;
            }
        }

    }
}
