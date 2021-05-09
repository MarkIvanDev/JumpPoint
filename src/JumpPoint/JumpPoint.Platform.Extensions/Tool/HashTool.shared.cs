using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Extensions
{
    public static partial class HashTool
    {
        public static async Task<string> ComputeHash(HashFunction hashFunction, ToolPayload payload, IProgress<double?> progress = null)
            => await PlatformComputeHash(hashFunction, payload, progress);
    }
}
