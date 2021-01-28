using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class LibraryService
    {

        static async Task<IList<Library>> PlatformGetLibraries()
        {
            var libs = new List<Library>();

            foreach (var template in (LibraryTemplate[])Enum.GetValues(typeof(LibraryTemplate)))
            {
                var lib = await PlatformGetLibrary(template);
                if (!(lib is null))
                {
                    libs.Add(lib);
                }
            }

            return libs;
        }

        static async Task<Library> PlatformGetLibrary(LibraryTemplate template)
        {
            try
            {
                var library = await PlatformGetStorageLibrary(template);
                return library is null ? null : new Library(template);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task PlatformLoad(Library library)
        {
            try
            {
                var lib = await PlatformGetStorageLibrary(library.LibraryTemplate);
                //var item = lib is null ? null : await PlatformGetLocalStorageItem(lib.SaveFolder);
                //library.SaveFolder = item as IFolderBase;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task<StorageLibrary> PlatformGetStorageLibrary(LibraryTemplate template)
        {
            try
            {
                switch (template)
                {
                    case LibraryTemplate.Documents:
                        return await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
                    case LibraryTemplate.Music:
                        return await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
                    case LibraryTemplate.Pictures:
                        return await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                    case LibraryTemplate.Videos:
                        return await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
                    case LibraryTemplate.Unknown:
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task<IList<DirectoryBase>> PlatformGetLibraryFolders(LibraryTemplate template)
        {
            try
            {
                var folders = new List<DirectoryBase>();
                var lib = await PlatformGetStorageLibrary(template);
                if (!(lib is null))
                {
                    foreach (var folder in lib.Folders)
                    {
                        //var item = await StorageService.GetFolder(folder);
                        //if (item is DirectoryBase f)
                        //{
                        //    folders.Add(f);
                        //}
                    }
                }
                return folders;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return new List<DirectoryBase>();
            }
        }

        static async Task<DirectoryBase> PlatformGetSaveFolder(LibraryTemplate template)
        {
            try
            {
                var lib = await PlatformGetStorageLibrary(template);
                //return (lib is null ? null :
                //    await PlatformGetLocalStorageItem(lib.SaveFolder)) as DirectoryBase;
                return null;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task PlatformAddLibraryFolder(LibraryTemplate template)
        {
            try
            {
                var lib = await PlatformGetStorageLibrary(template);
                _ = lib is null ? null : await lib.RequestAddFolderAsync();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task PlatformRemoveLibraryFolder(LibraryTemplate template, DirectoryBase directory)
        {
            try
            {
                var lib = await PlatformGetStorageLibrary(template);
                var f = await StorageFolder.GetFolderFromPathAsync(directory.Path);
                _ = lib is null && f is null ? false :
                    await lib.RequestRemoveFolderAsync(f);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

    }
}
