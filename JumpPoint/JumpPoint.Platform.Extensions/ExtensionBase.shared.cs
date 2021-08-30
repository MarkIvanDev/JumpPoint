using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NittyGritty;
using NittyGritty.Platform.Storage;
using Xamarin.Essentials;
#if UWP
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation;
using Windows.Storage;
#endif

namespace JumpPoint.Platform.Extensions
{
    public abstract class ExtensionBase : ObservableObject
    {
        private readonly string extensionGroup;

        public ExtensionBase(string extensionGroup)
        {
            this.extensionGroup = extensionGroup;
        }

        private string _packageId;

        public string PackageId
        {
            get { return _packageId; }
            set { Set(ref _packageId, value); }
        }

        private string _package;

        public string Package
        {
            get { return _package; }
            set { Set(ref _package, value); }
        }

        private string _extensionId;

        public string ExtensionId
        {
            get { return _extensionId; }
            set { Set(ref _extensionId, value); }
        }

        public string Identifier
        {
            get { return $"{PackageId}!{ExtensionId}"; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private Stream _logo;

        public Stream Logo
        {
            get { return _logo; }
            set { Set(ref _logo, value); }
        }

        private string _publisher;

        public string Publisher
        {
            get { return _publisher; }
            set { Set(ref _publisher, value); }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { Set(ref _version, value); }
        }

        private ExtensionSignature _signature;

        public ExtensionSignature Signature
        {
            get { return _signature; }
            set { Set(ref _signature, value); }
        }

        private IFolder _folder;

        public IFolder Folder
        {
            get { return _folder; }
            set { Set(ref _folder, value); }
        }

        public bool IsEnabled
        {
            get { return Preferences.Get(Identifier, true, extensionGroup); }
            set
            {
                Preferences.Set(Identifier, value, extensionGroup);
                RaisePropertyChanged();
            }
        }

        private bool _isAvailable;

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { Set(ref _isAvailable, value); }
        }

        #if UWP
        public static async Task<T> Extract<T>(AppExtension extension) where T : ExtensionBase, new()
        {
            var extensionBase = new T();
            extensionBase.Name = extension.DisplayName;
            extensionBase.Description = extension.Description;
            extensionBase.PackageId = extension.Package.Id.FamilyName;
            extensionBase.Package = extension.Package.DisplayName;
            extensionBase.ExtensionId = extension.Id;
            extensionBase.Publisher = extension.Package.PublisherDisplayName;
            extensionBase.Version = $"{extension.Package.Id.Version.Major}.{extension.Package.Id.Version.Minor}.{extension.Package.Id.Version.Build}.{extension.Package.Id.Version.Revision}";
            extensionBase.IsAvailable = extension.Package.Status.VerifyIsOK();
            extensionBase.Signature = (ExtensionSignature)extension.Package.SignatureKind;

            var folder = await extension.GetPublicFolderAsync();
            extensionBase.Folder = folder is null ? null : new NGFolder(folder);

            var logo = folder != null && await folder.TryGetItemAsync("Logo.png") is StorageFile logoFile ?
                await logoFile.OpenReadAsync() :
                await extension.AppInfo.DisplayInfo.GetLogo(new Size(1, 1)).OpenReadAsync();
            extensionBase.Logo = logo.AsStream();

            return extensionBase;
        }
        #endif

    }

    public enum ExtensionSignature
    {
        None = 0,
        Developer = 1,
        Enterprise = 2,
        Store = 3,
        System = 4
    }

}
