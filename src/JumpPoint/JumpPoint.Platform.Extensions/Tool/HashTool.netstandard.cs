using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Extensions
{
    public static partial class HashTool
    {
        static Task<string> PlatformComputeHash(HashFunction hashFunction, ToolPayload payload, IProgress<double?> progress = null)
            => throw new NotImplementedException();

        static string PlatformSha256Hash(string text)
            => throw new NotImplementedException();
    }
}
