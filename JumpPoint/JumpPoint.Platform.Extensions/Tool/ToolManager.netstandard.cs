using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Extensions
{
    public static partial class ToolManager
    {

        static Task<IList<Tool>> PlatformGetTools()
            => throw new NotImplementedException();

        static Task<IList<ToolResultPayload>> PlatformRun(Tool tool, IList<JumpPointItem> items)
            => throw new NotImplementedException();

        static Task<IList<ToolPayload>> PlatformExtractPayloads(IReadOnlyDictionary<string, object> data)
            => throw new NotImplementedException();

    }
}
