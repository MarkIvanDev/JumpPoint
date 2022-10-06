using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storj;
using NittyGritty.Extensions;
using uplink.NET.Models;
using uplink.NET.Services;

namespace JumpPoint.Platform.Services.Storj
{
    public static class StorjServiceExtensions
    {
        public static async Task<IList<Bucket>> GetBuckets(this Access access)
        {
            try
            {
                var bucketService = new BucketService(access);
                var result = await bucketService.ListBucketsAsync(new ListBucketsOptions());
                return result.Items;
            }
            catch (Exception)
            {
                return new List<Bucket>();
            }
        }

        public static async Task<Bucket> GetBucket(this Access access, string name)
        {
            try
            {
                var bucketService = new BucketService(access);
                var result = await bucketService.GetBucketAsync(name);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<IList<uplink.NET.Models.Object>> GetObjects(this Access access, Bucket bucket, string prefix)
        {
            try
            {
                var objectService = new ObjectService(access);
                var result = await objectService.ListObjectsAsync(bucket, new ListObjectsOptions
                {
                    System = true,
                    Prefix = prefix
                });
                return result.Items;
            }
            catch (Exception)
            {
                return new List<uplink.NET.Models.Object>();
            }
        }

        public static async Task<IList<uplink.NET.Models.Object>> GetAllObjects(this Access access, Bucket bucket)
        {
            try
            {
                var objectService = new ObjectService(access);
                var result = await objectService.ListObjectsAsync(bucket, new ListObjectsOptions
                {
                    System = true,
                    Recursive = true
                });
                return result.Items;
            }
            catch (Exception)
            {
                return new List<uplink.NET.Models.Object>();
            }
        }

        public static async Task<uplink.NET.Models.Object> GetObject(this Access access, Bucket bucket, string key)
        {
            try
            {
                var objectService = new ObjectService(access);
                if (key.EndsWith("/"))
                {
                    var result = await objectService.ListObjectsAsync(bucket, new ListObjectsOptions
                    {
                        System = true,
                        Recursive = true
                    });
                    return result.Items.FirstOrDefault(i => i.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    var result = await objectService.GetObjectAsync(bucket, key);
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Stream> GetContent(this Access access, Bucket bucket, string key)
        {
            try
            {
                var objectService = new ObjectService(access);
                var operation = await objectService.DownloadObjectAsync(bucket, key, new DownloadOptions(), false);
                await operation.StartDownloadAsync();
                if (operation.Completed)
                {
                    return operation.DownloadedBytes.ToMemoryStream();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<uplink.NET.Models.Object> CreateFolder(this Access access, Bucket bucket, string key, CreateOption option)
        {
            try
            {
                var objectService = new ObjectService(access);
                var itemKey = key;
                var existingItem = await access.GetObject(bucket, key);
                switch (option)
                {
                    case CreateOption.ReplaceExisting:
                        break;

                    case CreateOption.DoNothing:
                        if (existingItem != null)
                        {
                            return null;
                        }
                        break;

                    case CreateOption.OpenIfExists:
                        if (existingItem != null)
                        {
                            return existingItem;
                        }
                        break;

                    case CreateOption.GenerateUniqueName:
                    default:
                        var segments = key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        var name = segments.LastOrDefault();
                        var number = 2;
                        while (await access.GetObject(bucket, string.Join("/", segments).WithEnding("/")) != null)
                        {
                            segments[segments.Length - 1] = $"{name} ({number})";
                            number += 1;
                        }
                        itemKey = string.Join("/", segments).WithEnding("/");
                        break;
                }
                var operation = await objectService.UploadObjectAsync(bucket, itemKey, new UploadOptions(), Array.Empty<byte>(), false);
                await operation.StartUploadAsync();
                var newFolder = await access.GetObject(bucket, itemKey);
                newFolder.IsPrefix = true;
                return newFolder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<uplink.NET.Models.Object> CreateFile(this Access access, Bucket bucket, string key, CreateOption option, byte[] content)
        {
            try
            {
                var objectService = new ObjectService(access);
                var itemKey = key;
                var existingItem = await access.GetObject(bucket, key);
                switch (option)
                {
                    case CreateOption.ReplaceExisting:
                        break;

                    case CreateOption.DoNothing:
                        if (existingItem != null)
                        {
                            return null;
                        }
                        break;

                    case CreateOption.OpenIfExists:
                        if (existingItem != null)
                        {
                            return existingItem;
                        }
                        break;

                    case CreateOption.GenerateUniqueName:
                    default:
                        if (existingItem != null)
                        {
                            var segments = key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                            var fileName = segments.LastOrDefault();
                            var name = Path.GetFileNameWithoutExtension(fileName);
                            var ext = Path.GetExtension(fileName);
                            var number = 2;
                            while (await access.GetObject(bucket, string.Join("/", segments)) != null)
                            {
                                segments[segments.Length - 1] = $"{name} ({number}){ext}";
                                number += 1;
                            }
                            itemKey = string.Join("/", segments);
                        }
                        break;
                }
                var operation = await objectService.UploadObjectAsync(bucket, itemKey, new UploadOptions(), content ?? Array.Empty<byte>(), false);
                await operation.StartUploadAsync();
                return await access.GetObject(bucket, itemKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Rename(this Access access, Bucket bucket, string oldKey, string newKey, RenameOption option)
        {
            try
            {
                var objectService = new ObjectService(access);
                await objectService.MoveObjectAsync(bucket, oldKey, bucket, newKey);
                return newKey.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().TrimEnd('/');
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task Delete(this Access access, Bucket bucket, string key)
        {
            try
            {
                var objectService = new ObjectService(access);
                await objectService.DeleteObjectAsync(bucket, key);
            }
            catch (Exception)
            {
            }
        }

        public static StorageItemBase Convert(this Bucket bucket, StorjAccount account)
        {
            var path = $@"cloud:\Storj\{account.Name}\{bucket.Name}";
            return new StorjFolder(account, bucket, null, path);
        }

        public static StorageItemBase Convert(this uplink.NET.Models.Object @object, StorjAccount account, Bucket bucket)
        {
            var path = $@"cloud:\Storj\{account.Name}\{bucket.Name}\{@object.Key.Replace('/', '\\')}";
            if (@object.IsPrefix)
            {
                return new StorjFolder(account, bucket, @object, path);
            }
            else
            {
                return new StorjFile(account, bucket, @object, path);
            }
        }
    }
}
