using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using NittyGritty;
using NittyGritty.Commands;

namespace JumpPoint.ViewModels.Dialogs
{
    public class AppLinkLaunchViewModel : ObservableObject
    {
        public AppLinkLaunchViewModel(AppLink appLink)
        {
            AppLink = appLink;
        }

        public AppLink AppLink { get; }

        private AppLinkLaunchTypes _launchType;

        public AppLinkLaunchTypes LaunchType
        {
            get { return _launchType; }
            set { Set(ref _launchType, value); }
        }

    }
}
