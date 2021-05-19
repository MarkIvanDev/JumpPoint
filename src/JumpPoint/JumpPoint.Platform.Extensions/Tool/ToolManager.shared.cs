using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Extensions
{
    public static partial class ToolManager
    {
        public static event EventHandler ExtensionCollectionChanged;

        public static async Task<IList<Tool>> GetTools()
            => await PlatformGetTools();

        public static async Task<ToolResult> Run(Tool tool, IList<JumpPointItem> items)
            => await PlatformRun(tool, items);

        public static async Task<IList<ToolPayload>> ExtractPayloads(IReadOnlyDictionary<string, object> data)
            => await PlatformExtractPayloads(data);

    }
}
