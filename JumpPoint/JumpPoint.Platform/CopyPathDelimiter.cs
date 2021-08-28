using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum CopyPathDelimiter
    {
        NewLine = 0,
        Tab = 1,
        Comma = 2,
        Pipe = 3,
        Colon = 4,
        Semicolon = 5,
        Slash = 6,
        Backslash = 7,
        Plus = 8,
        Hyphen = 9
    }

    public static class CopyPathDelimiterExtensions
    {
        public static string ToDelimiter(this CopyPathDelimiter delimiter)
        {
            switch (delimiter)
            {
                case CopyPathDelimiter.NewLine:
                default:
                    return Environment.NewLine;

                case CopyPathDelimiter.Tab:
                    return "\t";

                case CopyPathDelimiter.Comma:
                    return ",";

                case CopyPathDelimiter.Pipe:
                    return "|";

                case CopyPathDelimiter.Colon:
                    return ":";

                case CopyPathDelimiter.Semicolon:
                    return ";";

                case CopyPathDelimiter.Slash:
                    return "/";

                case CopyPathDelimiter.Backslash:
                    return "\\";

                case CopyPathDelimiter.Plus:
                    return "+";

                case CopyPathDelimiter.Hyphen:
                    return "-";
            }
        }
    }
}
