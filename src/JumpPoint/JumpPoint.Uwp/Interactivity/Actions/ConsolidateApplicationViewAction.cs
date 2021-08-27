using Microsoft.Xaml.Interactivity;
using NittyGritty.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Interactivity.Actions
{
    public class ConsolidateApplicationViewAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            ConsolidateCommand.TryExecute();
            return true;
        }

        private RelayCommand _Consolidate;
        public RelayCommand ConsolidateCommand => _Consolidate ?? (_Consolidate = new RelayCommand(
            async () =>
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            }));

    }
}
