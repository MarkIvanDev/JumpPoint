using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items
{
    public class Library : JumpPointItem
    {
        public Library(LibraryTemplate template) : base(JumpPointItemType.Library)
        {
            LibraryTemplate = template;
            Name = template.ToString();
            Path = template.ToString();
            DisplayName = template.ToString();
            DisplayType = Type.ToString();
        }

        public LibraryTemplate LibraryTemplate { get; }

    }
}
