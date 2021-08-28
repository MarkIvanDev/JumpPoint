using System;
using System.Collections.Generic;
using System.Text;
using Humanizer;

namespace JumpPoint.ViewModels.Helpers
{
    public static class PropertyHelper
    {
        public static string ListOfStrings(IList<string> strings)
        {
            return string.Join("; ", strings);
        }

        public static string ConvertToKilo(uint? data, string unit)
        {
            if (data != null)
            {
                return $"{Math.Round(data.Value / 1000.0, 2)} {unit}";
            }
            return null;
        }

        public static string BitsPerSecondToKilobitsPerSecond(uint? bitrate)
        {
            if(bitrate != null)
            {
                return $"{Math.Round(bitrate.Value / 1000.0, 2)} kbps";
            }
            return null;
        }

        public static string Coordinates(double[] coordinates)
        {
            if (coordinates != null && coordinates.Length == 3)
            {
                return $"{coordinates[0]}° {coordinates[1]}\' {coordinates[2]}\"";
            }
            return null;
        }

        public static string Buffer(byte[] buffer)
        {
            if (buffer != null)
            {
                return Convert.ToBase64String(buffer);
            }
            return null;
        }

    }
}
