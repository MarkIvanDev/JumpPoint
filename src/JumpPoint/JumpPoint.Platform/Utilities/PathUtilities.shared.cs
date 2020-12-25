using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Models;

namespace JumpPoint.Platform.Utilities
{
    public static class PathUtilities
    {
        public static string GetRoot(string path)
        {
            var workingPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (workingPath.StartsWith(@"\\?\")) // Unmounted storage
            {
                var index = workingPath.IndexOf('\\', 4);
                return index != -1 ?
                    workingPath.Substring(0, index) :
                    workingPath;
            }
            else if (workingPath.StartsWith(@"\\")) // Network path
            {
                var index = workingPath.IndexOf('\\', 2);
                return index != -1 ?
                    workingPath.Substring(0, index) :
                    workingPath;
            }
            else if (workingPath.Length >= 2 && char.IsLetter(workingPath[0]) && workingPath[1] == ':') // Mounted
            {
                return Path.GetPathRoot(workingPath);
            }
            return null;
        }

        public static PathKind GetPathKind(string path)
        {
            var workingPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (workingPath.StartsWith(@"\\?\")) // Unmounted storage
            {
                return PathKind.Unmounted;
            }
            else if (workingPath.StartsWith(@"\\")) // Network path
            {
                return PathKind.Network;
            }
            else if (workingPath.Length >= 2 && workingPath[1] == ':') // Mounted
            {
                return PathKind.Mounted;
            }
            return PathKind.Unknown;
        }

        public static IList<Breadcrumb> GetCrumbs(string path)
        {
            var workingPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

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
            var parent = Path.GetDirectoryName(workingPath);
            if (!string.IsNullOrEmpty(parent))
            {
                while (!string.IsNullOrEmpty(parent))
                {
                    var displayName = Path.GetFileName(parent);
                    displayName = string.IsNullOrEmpty(displayName) ? parent : displayName;

                    crumbs.Insert(0, new Breadcrumb()
                    {
                        PathType = string.IsNullOrEmpty(Path.GetDirectoryName(parent)) ? PathType.Drive : PathType.Folder,
                        Path = $"{prefix}{parent}",
                        DisplayName = displayName
                    });

                    parent = Path.GetDirectoryName(parent);
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

    public enum PathKind
    {
        Unknown = 0,
        Mounted = 1,
        Unmounted = 2,
        Network = 3,
        Cloud = 4
    }
}
