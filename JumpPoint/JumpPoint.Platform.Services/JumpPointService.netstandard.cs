using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using NGStorage = NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class JumpPointService
    {
        static Task<NGStorage.IStorageItem> PlatformConvert(StorageItemBase item)
            => throw new NotImplementedException();

        static Task<IList<NGStorage.IStorageItem>> PlatformConvert(IEnumerable<JumpPointItem> items)
            => throw new NotImplementedException();

        static Task<bool> PlatformOpenFile(FileBase file, bool useDefaultHandler)
            => throw new NotImplementedException();

        static Task PlatformOpenInFileExplorer(string path, IEnumerable<StorageItemBase> itemsToSelect = null)
            => throw new NotImplementedException();

        static Task PlatformOpenProperties(Collection<Seed> seeds)
            => throw new NotImplementedException();

        static Task<bool> PlatformRate()
            => throw new NotImplementedException();
    }
}
