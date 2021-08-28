﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace JumpPoint.Uwp.Helpers
{
    [ContentProperty(Name = nameof(Actions))]
    public class TimeoutAction : DependencyObject, IAction
    {
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actions = (ActionCollection)base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    SetValue(ActionsProperty, actions);
                }
                return actions;
            }
        }
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(nameof(Actions), typeof(ActionCollection),
                typeof(TimeoutAction), new PropertyMetadata(null));

        public int Milliseconds
        {
            get { return (int)GetValue(MillisecondsProperty); }
            set { SetValue(MillisecondsProperty, value); }
        }

        public static readonly DependencyProperty MillisecondsProperty =
            DependencyProperty.Register(nameof(Milliseconds), typeof(int),
                typeof(TimeoutAction), new PropertyMetadata(5000));

        public object Execute(object sender, object parameter)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Milliseconds) };
            timer.Tick += Timer_Tick;
            timer.Start();

            return null;
        }

        private void Timer_Tick(object sender, object e)
        {
            var timer = sender as DispatcherTimer;
            timer.Tick -= Timer_Tick;
            timer.Stop();

            Interaction.ExecuteActions(sender, Actions, null);
        }
    }
}
