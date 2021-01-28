using System;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items
{
    public abstract class JumpPointItem : ObservableObject
    {
        public JumpPointItem(JumpPointItemType type)
        {
            Type = type;
        }

        public JumpPointItemType Type { get; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _path;

        public virtual string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { Set(ref _displayName, value); }
        }

        private string _displayType;

        public string DisplayType
        {
            get { return _displayType; }
            set { Set(ref _displayType, value); }
        }

        private bool _isFavorite;

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { Set(ref _isFavorite, value); }
        }

    }
}
