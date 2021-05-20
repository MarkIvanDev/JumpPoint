using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public static class LayoutModes
    {
        public static string Grid { get; } = nameof(Grid);

        public static string Details { get; } = nameof(Details);

        public static string Tiles { get; } = nameof(Tiles);

        public static string List { get; } = nameof(List);
    }
}
