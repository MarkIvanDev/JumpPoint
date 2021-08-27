using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace JumpPoint.Uwp.Helpers
{
    public static class DragDropHelper
    {
        public static bool HasControl(this DragDropModifiers modifiers)
        {
            return (modifiers & DragDropModifiers.Control) == DragDropModifiers.Control;
        }

        public static bool HasShift(this DragDropModifiers modifiers)
        {
            return (modifiers & DragDropModifiers.Shift) == DragDropModifiers.Shift;
        }

        public static bool HasAlt(this DragDropModifiers modifiers)
        {
            return (modifiers & DragDropModifiers.Alt) == DragDropModifiers.Alt;
        }

        public static bool HasModifier(this DragDropModifiers modifiers, DragDropModifiers toCheck)
        {
            return (modifiers & toCheck) == toCheck;
        }
    }
}
