using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlaUI.Core.AutomationElements;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public static class AutomationElementExtensions
    {
        public static bool IsTextElement(this AutomationElement element) 
        {
            return 
                element.Patterns.Text.IsSupported ||
                element.Patterns.Text2.IsSupported ||
                element.Patterns.TextEdit.IsSupported ||
                element.Patterns.TextChild.IsSupported;
        }
    }
}
