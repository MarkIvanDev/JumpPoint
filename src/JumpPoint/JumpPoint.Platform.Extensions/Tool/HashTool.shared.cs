using System;
using System.Threading.Tasks;
using JumpPoint.Extensions.Tools;

namespace JumpPoint.Platform.Extensions
{
    public static partial class HashTool
    {
        public static async Task<string> ComputeHash(HashFunction hashFunction, ToolPayload payload, IProgress<double?> progress = null)
            => await PlatformComputeHash(hashFunction, payload, progress);

        public static string Sha256Hash(string text)
            => PlatformSha256Hash(text);
    }
}
