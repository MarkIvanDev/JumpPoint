using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Uwp.Converters;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace JumpPoint.Uwp.Converters
{
    public static class DriveStorageColorConverter
    {
        public static Brush GetBrush(ulong? capacity, ulong? usedSpace)
        {
            var uiSettings = new UISettings();
            var accent = uiSettings.GetColorValue(UIColorType.Accent);
            if (capacity.HasValue && usedSpace.HasValue)
            {
                var percentageUsed = usedSpace / (double)capacity;
                if (percentageUsed < 0.75)
                {
                    return new SolidColorBrush(accent);
                }
                else
                {
                    return new SolidColorBrush(Colors.Firebrick);
                }
            }
            else
            {
                return new SolidColorBrush(accent);
            }
        }

    }
}
