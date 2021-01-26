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

        
    }
}
