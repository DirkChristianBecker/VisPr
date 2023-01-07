using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class WriteTextEvent : RecordedEvent
    {
        public string Text { get; set; }
        public WriteTextEvent(string text, UInt64 s, List<ElementSelector> elements) : base(s, elements) 
        {
            Text = text;
        }

        public override string ToString() 
        {
            return $"{SequenceNumber}. ({nameof(WriteTextEvent)}) \nText:\n{Text} \n {Selectors()}";
        }
    }
}
