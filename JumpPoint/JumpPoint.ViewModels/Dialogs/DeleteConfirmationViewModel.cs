using NittyGritty;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Dialogs
{
    public class DeleteConfirmationViewModel : ObservableObject
    {
        public DeleteConfirmationViewModel()
        {
            
        }

        private bool _deletePermanently;

        public bool DeletePermanently
        {
            get { return _deletePermanently; }
            set { Set(ref _deletePermanently, value); }
        }

    }
}
