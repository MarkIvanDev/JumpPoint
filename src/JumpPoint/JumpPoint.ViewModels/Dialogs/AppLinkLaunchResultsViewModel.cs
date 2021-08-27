using System;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class AppLinkLaunchResultsViewModel : ObservableObject
    {
        public AppLinkLaunchResultsViewModel(AppLink appLink, Collection<ValueInfo> results)
        {
            AppLink = appLink;
            Results = results;
        }

        public AppLink AppLink { get; }

        public Collection<ValueInfo> Results { get; }

    }
}
