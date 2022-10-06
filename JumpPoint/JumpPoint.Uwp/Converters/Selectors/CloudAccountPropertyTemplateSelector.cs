using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.CloudStorage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters.Selectors
{
    public class CloudAccountPropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Regular { get; set; }

        public DataTemplate Sensitive { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch ((item as CloudAccountProperty)?.IsSensitive)
            {
                case true:
                    return Sensitive;

                case false:
                default:
                    return Regular;
            }
        }

    }
}
