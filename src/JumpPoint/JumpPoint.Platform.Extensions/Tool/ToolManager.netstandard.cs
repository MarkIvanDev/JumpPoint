using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Extensions
{
    public static partial class ToolManager
    {

        static void PlatformStart()
            => throw new NotImplementedException();

        static void PlatformStop()
            => throw new NotImplementedException();

        static Task<IList<Tool>> PlatformGetTools()
            => throw new NotImplementedException();

        static Task<ToolResult> PlatformRun(Tool tool, IList<JumpPointItem> items)
            => throw new NotImplementedException();

        static Task<IList<ToolPayload>> PlatformExtractPayloads(IReadOnlyDictionary<string, object> data)
            => throw new NotImplementedException();

    }
}
