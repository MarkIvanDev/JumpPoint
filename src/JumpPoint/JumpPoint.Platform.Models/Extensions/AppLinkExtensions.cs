using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Models.Extensions
{
    public static class AppLinkExtensions
    {
        public static bool IsLaunchUriAvailable(this AppLinkLaunchTypes launchTypes)
        {
            return (launchTypes & AppLinkLaunchTypes.Uri) == AppLinkLaunchTypes.Uri;
        }

        public static bool IsLaunchUriForResultsAvailable(this AppLinkLaunchTypes launchTypes)
        {
            return (launchTypes & AppLinkLaunchTypes.UriForResults) == AppLinkLaunchTypes.UriForResults;
        }

        public static bool IsLaunchTypeDefined(this int launchType)
        {
            switch (launchType)
            {
                case 1:
                case 2:
                case 3:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsHexColor(this string hexColor)
        {
            try
            {
                _ = ColorConverters.FromHex(hexColor);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Color ToColor(this string hexColor)
        {
            try
            {
                return ColorConverters.FromHex(hexColor);
            }
            catch (Exception)
            {
                return Color.Transparent;
            }
        }

        public static string[] RemoveEmptyEntries(this string[] array)
        {
            return array.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
        }

        public static Collection<ValueInfo> ToQuery(this string[] queryKeys)
        {
            var query = new Collection<ValueInfo>();

            if (queryKeys is null || queryKeys.Length == 0) return query;

            foreach (var item in queryKeys)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    query.Add(new ValueInfo
                    {
                        Key = item,
                        DataType = DataType.String
                    });
                }
            }
            return query;
        }

        public static Collection<ValueInfo> ToInputKeys(this string[] inputKeys)
        {
            var keys = new Collection<ValueInfo>();

            if (inputKeys is null || inputKeys.Length == 0) return keys;

            foreach (var item in inputKeys)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    keys.Add(new ValueInfo
                    {
                        Key = item,
                        DataType = DataType.String
                    });
                }
            }

            return keys;
        }

    }
}
