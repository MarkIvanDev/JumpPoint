using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Uwp.Converters;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class AttributeVisibilityConverter : MultiConverter<Visibility>
    {
        public override Visibility Convert()
        {
            var att = Bindings[1].Value as FileAttributes? ?? FileAttributes.Normal;
            if (Bindings[0].Value is IList<FileAttributes> listAttributes)
            {
                if (listAttributes.All(a => (a & att) == 0))
                {
                    // If none of the attributes has the specified attribute, return Collapsed
                    return Visibility.Collapsed;
                }
            }
            else if (Enum.TryParse<FileAttributes>(Bindings[0].Value?.ToString(), out var attribute))
            {
                if((attribute & att) == 0)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }
    }
}
