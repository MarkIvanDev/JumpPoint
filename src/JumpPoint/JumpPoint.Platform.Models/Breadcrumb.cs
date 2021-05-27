using NittyGritty;

namespace JumpPoint.Platform.Models
{
    public class Breadcrumb : ObservableObject
    {

        private AppPath _appPath;

        public AppPath AppPath
        {
            get { return _appPath; }
            set { Set(ref _appPath, value); }
        }

        private string _path;

        public string Path
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

    }
}
