using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using JumpPoint.FullTrust.IO.Interop;

namespace JumpPoint.FullTrust.IO
{
    class FileOperation : IDisposable
    {
        private bool _disposed;
        private readonly IFileOperation _fileOperation;
        private readonly FileOperationProgressSink _callbackSink;
        private readonly uint _sinkCookie;

        public FileOperation() : this(FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOFX_ADDUNDORECORD, null) { }
        public FileOperation(FileOperationFlags fileOperationFlags) : this(fileOperationFlags, null) { }
        public FileOperation(FileOperationFlags fileOperationFlags, FileOperationProgressSink callbackSink)
        {
            _callbackSink = callbackSink;
            _fileOperation = (IFileOperation)Activator.CreateInstance(_fileOperationType);

            _fileOperation.SetOperationFlags(fileOperationFlags);
            if (_callbackSink != null) _sinkCookie = _fileOperation.Advise(_callbackSink);
        }

        public void CopyItem(string source, string destination, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            using (ComReleaser<IShellItem> destinationItem = CreateShellItem(destination))
            {
                _fileOperation.CopyItem(sourceItem.Item, destinationItem.Item, newName, null);
            }
        }

        public void MoveItem(string source, string destination, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            using (ComReleaser<IShellItem> destinationItem = CreateShellItem(destination))
            {
                _fileOperation.MoveItem(sourceItem.Item, destinationItem.Item, newName, null);
            }
        }

        public void RenameItem(string source, string newName)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            {
                _fileOperation.RenameItem(sourceItem.Item, newName, null);
            }
        }

        public void DeleteItem(string source)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> sourceItem = CreateShellItem(source))
            {
                _fileOperation.DeleteItem(sourceItem.Item, null);
            }
        }

        public void NewItem(string folderName, string name, FileAttributes attrs)
        {
            ThrowIfDisposed();
            using (ComReleaser<IShellItem> folderItem = CreateShellItem(folderName))
            {
                _fileOperation.NewItem(folderItem.Item, attrs, name, string.Empty, _callbackSink);
            }
        }

        public void PerformOperations()
        {
            ThrowIfDisposed();
            _fileOperation.PerformOperations();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_callbackSink != null) _fileOperation.Unadvise(_sinkCookie);
                Marshal.FinalReleaseComObject(_fileOperation);
            }
        }

        private static ComReleaser<IShellItem> CreateShellItem(string path)
        {
            return new ComReleaser<IShellItem>(
                (IShellItem)SHCreateItemFromParsingName(path, null, ref _shellItemGuid));
        }

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig=false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern object SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);

        private static readonly Guid CLSID_FileOperation = new Guid("3ad05575-8857-4850-9277-11b85bdb8e09");
        private static readonly Type _fileOperationType = Type.GetTypeFromCLSID(CLSID_FileOperation);
        private static Guid _shellItemGuid = typeof(IShellItem).GUID;
    }
}
