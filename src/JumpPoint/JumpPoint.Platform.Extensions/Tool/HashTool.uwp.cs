using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace JumpPoint.Platform.Extensions
{
    public static partial class HashTool
    {
        private static readonly HashAlgorithmProvider MD5;
        private static readonly HashAlgorithmProvider SHA1;
        private static readonly HashAlgorithmProvider SHA256;
        private static readonly HashAlgorithmProvider SHA384;
        private static readonly HashAlgorithmProvider SHA512;

        static HashTool()
        {
            MD5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            SHA1 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            SHA256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            SHA384 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha384);
            SHA512 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
        }

        static async Task<string> PlatformComputeHash(HashFunction hashFunction, ToolPayload payload, IProgress<double?> progress = null)
        {
            try
            {
                progress?.Report(null);

                var hash = GetHash();
                if (hash is null) return string.Empty;

                var file = await ToolManager.GetFile(payload);
                if (file is null) return string.Empty;

                var capacity = 4096U;
                var buffer = new Buffer(capacity);

                using (var stream = await file.OpenStreamForReadAsync())
                using (var inputStream = stream.AsInputStream())
                {
                    while (true)
                    {
                        await inputStream.ReadAsync(buffer, capacity, InputStreamOptions.None);
                        if (buffer.Length == 0) break;

                        hash.Append(buffer);
                        progress?.Report((double)stream.Position / stream.Length);
                    }
                }
                progress?.Report(0);
                return CryptographicBuffer.EncodeToHexString(hash.GetValueAndReset());
            }
            catch (Exception)
            {
                return string.Empty;
            }

            CryptographicHash GetHash()
            {
                switch (hashFunction)
                {
                    case HashFunction.MD5:
                        return MD5.CreateHash();

                    case HashFunction.SHA1:
                        return SHA1.CreateHash();

                    case HashFunction.SHA256:
                        return SHA256.CreateHash();

                    case HashFunction.SHA384:
                        return SHA384.CreateHash();

                    case HashFunction.SHA512:
                        return SHA512.CreateHash();

                    case HashFunction.Unknown:
                    default:
                        return null;
                }
            }
        }

        static string PlatformSha256Hash(string text)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            var hash = SHA256.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hash);
        }

    }
}
