using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Utilities
{
    public static class FileUtilities
    {
        private static readonly HashSet<string> builtInIconFileTypes;

        static FileUtilities()
        {
            builtInIconFileTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Common
                ".dll",
                ".ini",
                ".pdf",
                ".txt",
                ".zip",

                // Data formats
                ".csv",
                ".json",
                ".xml",
                ".db",
                ".srt",

                // Code
                ".cpp",
                ".cs",
                ".vb",
                ".xaml",
                ".html",
                ".js",
                ".css",
                ".py",

                // Fonts
                ".otf",
                ".ttf",
                ".woff"
            };
        }

        public static bool HasBuiltInIcon(string fileType)
        {
            return builtInIconFileTypes.Contains(fileType.ToLower());
        }
    }
}
