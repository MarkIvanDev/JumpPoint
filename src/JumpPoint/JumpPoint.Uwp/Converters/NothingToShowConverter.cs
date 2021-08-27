using NittyGritty.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters
{
    public class NothingToShowConverter : MultiConverter<Visibility>
    {
        public override Visibility Convert()
        {
            if (Bindings[0].Value is bool isLoading &&
                Bindings[1].Value is int itemsCount)
            {
                return !isLoading && itemsCount == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
    }
}
