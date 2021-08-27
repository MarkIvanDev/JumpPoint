using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Helpers
{
    public class ViewLayoutStateTrigger : StateTriggerBase
    {
        public string LayoutName
        {
            get { return (string)GetValue(LayoutNameProperty); }
            set { SetValue(LayoutNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutNameProperty =
            DependencyProperty.Register("LayoutName", typeof(string), typeof(ViewLayoutStateTrigger), new PropertyMetadata(null));

        public string CurrentLayout
        {
            get { return (string)GetValue(CurrentLayoutProperty); }
            set { SetValue(CurrentLayoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentLayout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentLayoutProperty =
            DependencyProperty.Register("CurrentLayout", typeof(string), typeof(ViewLayoutStateTrigger), new PropertyMetadata(null, OnCurrentLayoutChanged));

        private static void OnCurrentLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as ViewLayoutStateTrigger;
            var isActive = trigger.LayoutName == trigger.CurrentLayout;
            trigger.SetActive(isActive);
        }

        private static void TriggerStateCheck(ViewLayoutStateTrigger elementTypeTrigger, string dataValue, string triggerValue)
        {
            elementTypeTrigger.SetActive(dataValue == triggerValue);
        }
    }
}
