using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty.Extensions;
using NittyGritty.Utilities;

namespace JumpPoint.Platform.Models.Extensions
{
    public static class PathExtensions
    {
        public static string NormalizePath(this string path)
        {
            return string.IsNullOrEmpty(path) ?
                path :
                path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim();
        }

        public static PathKind GetPathKind(this string path)
        {
            if (string.IsNullOrEmpty(path)) return PathKind.Unknown;

            var workingPath = path.NormalizePath();
            if (workingPath.StartsWith(Prefix.UNMOUNTED, StringComparison.OrdinalIgnoreCase))
            {
                return PathKind.Unmounted;
            }
            else if (workingPath.StartsWith(Prefix.WSL, StringComparison.OrdinalIgnoreCase))
            {
                return PathKind.WSL;
            }
            else if (workingPath.StartsWith(Prefix.NETWORK, StringComparison.OrdinalIgnoreCase))
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
            else if (workingPath.StartsWith(Prefix.APPLINK, StringComparison.OrdinalIgnoreCase))
            {
                return PathKind.AppLink;
            }
            else if (workingPath.Length >= 2 && workingPath[1] == Path.VolumeSeparatorChar)
            {
                return PathKind.Mounted;
            }
            else if (CodeHelper.InvokeOrDefault(() => workingPath.DehumanizeTo<AppPath>(), AppPath.Unknown) != AppPath.Unknown)
            {
                return PathKind.Local;
            }
            return PathKind.Unknown;
        }

        public static IList<Breadcrumb> GetBreadcrumbs(this string path)
        {
            var crumbs = new List<Breadcrumb>();

            if (string.IsNullOrEmpty(path)) return crumbs;

            var workingPath = path.NormalizePath();
            var pathKind = workingPath.GetPathKind();
            if (pathKind == PathKind.Unknown) return crumbs;

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
                case PathKind.Local when CodeHelper.InvokeOrDefault(() => workingPath.TrimEnd('\\').DehumanizeTo<AppPath>(), AppPath.Unknown) is AppPath appPath:
                    if (crumbs.Count > 1)
                    {
                        crumbs.RemoveRange(1, crumbs.Count - 1);
                    }
                    foreach (var item in crumbs)
                    {
                        item.AppPath = appPath;
                        item.Path = appPath.Humanize();
                        item.DisplayName = appPath.Humanize();
                    }
                    return crumbs;

                case PathKind.Mounted:
                case PathKind.Unmounted:
                case PathKind.Network:
                    for (int i = 0; i < crumbs.Count; i++)
                    {
                        crumbs[i].AppPath = i == 0 ?
                            AppPath.Drive :
                            AppPath.Folder;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        AppPath = AppPath.Drives,
                        Path = nameof(AppPath.Drives),
                        DisplayName = nameof(AppPath.Drives)
                    });
                    return crumbs;

                case PathKind.Cloud:
                    for (int i = 0; i < crumbs.Count; i++)
                    {
                        if (i == 0)
                        {
                            crumbs[i].AppPath = AppPath.Cloud;
                            var provider = CodeHelper.InvokeOrDefault(() => crumbs[i].DisplayName.DehumanizeTo<CloudStorageProvider>(), CloudStorageProvider.Unknown);
                            crumbs[i].DisplayName = provider.Humanize();
                        }
                        else
                        {
                            crumbs[i].AppPath = i == 1 ? AppPath.Drive : AppPath.Folder;
                        }
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        AppPath = AppPath.CloudDrives,
                        Path = AppPath.CloudDrives.Humanize(),
                        DisplayName = AppPath.CloudDrives.Humanize()
                    });
                    return crumbs;

                case PathKind.Workspace:
                    if (crumbs.Count > 1)
                    {
                        crumbs.RemoveRange(1, crumbs.Count - 1);
                    }
                    foreach (var item in crumbs)
                    {
                        item.AppPath = AppPath.Workspace;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        AppPath = AppPath.Workspaces,
                        Path = nameof(AppPath.Workspaces),
                        DisplayName = nameof(AppPath.Workspaces)
                    });
                    return crumbs;

                case PathKind.AppLink:
                    if (crumbs.Count > 1)
                    {
                        crumbs.RemoveRange(1, crumbs.Count - 1);
                    }
                    foreach (var item in crumbs)
                    {
                        item.AppPath = AppPath.AppLinks;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        AppPath = AppPath.AppLinks,
                        Path = AppPath.AppLinks.Humanize(),
                        DisplayName = AppPath.AppLinks.Humanize()
                    });
                    return crumbs;

                case PathKind.WSL:
                    for (int i = 0; i < crumbs.Count; i++)
                    {
                        crumbs[i].AppPath = i == 0 ?
                            AppPath.Drive :
                            AppPath.Folder;
                    }
                    crumbs.Insert(0, new Breadcrumb()
                    {
                        AppPath = AppPath.WSL,
                        Path = nameof(AppPath.WSL),
                        DisplayName = nameof(AppPath.WSL)
                    });
                    return crumbs;

                case PathKind.Unknown:
                default:
                    return new List<Breadcrumb>();
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

                case PathKind.AppLink:
                    return Prefix.APPLINK;

                case PathKind.WSL:
                    return Prefix.WSL;

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
                pathInfo.Type = lastCrumb.AppPath;
                pathInfo.Path = lastCrumb.Path;
                pathInfo.DisplayName = lastCrumb.DisplayName;
            }
            else
            {
                pathInfo.Type = AppPath.Unknown;
                pathInfo.Path = path;
                pathInfo.DisplayName = string.Empty;
            }
        }

        public static string GetWorkspacePath(string workspaceName)
        {
            return $"{Prefix.WORKSPACE}{workspaceName}";
        }

        public static string GetAppLinkPath(string appLinkName)
        {
            return $"{Prefix.APPLINK}{appLinkName}";
        }

    }
}
