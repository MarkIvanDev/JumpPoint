using System;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Services;
using NittyGritty.Commands;
using Xamarin.Essentials;

namespace JumpPoint.ViewModels.Dialogs
{
    public class RequestPermissionViewModel
    {
        private AsyncRelayCommand _allowCommand;

        /// <summary>
        /// Gets the AllowCommand.
        /// </summary>
        public AsyncRelayCommand AllowCommand
        {
            get
            {
                return _allowCommand
                    ?? (_allowCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        await Launcher.OpenAsync("ms-settings:privacy-broadfilesystemaccess");
                    }));
            }
        }
    }
}
