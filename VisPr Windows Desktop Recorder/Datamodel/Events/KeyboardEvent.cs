using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public enum KeyButtonType
    {
        None,
        Up,
        Down
    }

    public class KeyboardEvent : RecordedEvent
    {
        public KeyButtonType KeyType { get; set; }
        public string KeyName { get; set; }
        public string UnicodeCharacter { get; set; }

        public KeyboardEvent(
            UInt64 n,
            KeyButtonType type,
            string keyname,
            string character,
            List<ElementSelector> elements) : base(n, elements)
        {
            KeyType = type;
            KeyName = keyname;
            UnicodeCharacter = character;
        }

        public KeyboardEvent() : this(0, KeyButtonType.None, "", "", new List<ElementSelector>())
        {

        }

        public override string ToString()
        {
            return $"{SequenceNumber}. {KeyType} Name: {KeyName} Character: {UnicodeCharacter} ({nameof(KeyboardEvent)}) \n {Selectors()}";
        }
    }
}
