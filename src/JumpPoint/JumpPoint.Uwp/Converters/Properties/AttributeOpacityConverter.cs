using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JumpPoint.Uwp.Helpers;
using NittyGritty.UI.Converters;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class AttributeOpacityConverter : MultiConverter<double>
    {
        public override double Convert()
        {
            var att = Bindings[1].Value as FileAttributes? ?? FileAttributes.Normal;
            if (Bindings[0].Value is IList<FileAttributes> listAttributes)
            {
                if (listAttributes.All(a => (a & att) == att))
                {
                    // If all attributes has the specified attribute, return 1.0
                    return 1.0;
                }
                else if (listAttributes.Any(a => (a & att) == att))
                {
                    // If only some attributes has the specified attribute, return 0.5
                    return 0.5;
                }
                else
                {
                    // If none of the attributes has the specified attribute, return 0.0
                    return 0.0;
                }
            }
            else if (Enum.TryParse<FileAttributes>(Bindings[0].Value?.ToString(), out var attribute))
            {
                if((attribute & att) == att)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
            return 0.0;
        }

    }
}
