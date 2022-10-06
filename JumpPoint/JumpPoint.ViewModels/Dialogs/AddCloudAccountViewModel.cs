using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty;
using NittyGritty.Collections;
using NittyGritty.Models;

namespace JumpPoint.ViewModels.Dialogs
{
    public class AddCloudAccountViewModel : ObservableObject
    {
        public AddCloudAccountViewModel(CloudStorageProvider provider, IList<CloudAccountProperty> keys)
        {
            Properties = new TrackableCollection<CloudAccountProperty>(keys, true);
            Properties.ItemPropertyChanged += OnItemPropertyChanged;
            Provider = provider;
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsValid));
        }

        public CloudStorageProvider Provider { get; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                Set(ref _name, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                Set(ref _email, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public TrackableCollection<CloudAccountProperty> Properties { get; }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Name) && Name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1 &&
            !string.IsNullOrWhiteSpace(Email) &&
            Properties.Where(p => p.IsRequired).All(p => !string.IsNullOrWhiteSpace(p.Value));

        public IDictionary<string, string> GetData()
        {
            var data = new Dictionary<string, string>
            {
                { nameof(Name), Name },
                { nameof(Email), Email }
            };
            foreach (var item in Properties)
            {
                data.Add(item.Name, item.Value);
            }
            return data;
        }
    }

}
