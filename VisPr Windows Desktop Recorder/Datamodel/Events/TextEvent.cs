using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class TextEvent : KeyboardEvent
    {
        public string CurrentUiElmentText { get; set; }
        public TextEvent(
            string text,
            UInt64 n,
            KeyButtonType type,
            string keyname,
            string character,
            List<ElementSelector> elements) : base(n, type, keyname, character, elements)
        {
            CurrentUiElmentText = text;
        }

    }
}
