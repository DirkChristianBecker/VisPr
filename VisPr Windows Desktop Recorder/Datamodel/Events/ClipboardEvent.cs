using EventHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class ClipboardEvent : RecordedEvent
    {
        public object Data { get; set; }
        public ClipboardContentTypes Type { get; set; }

        public ClipboardEvent(object data, ClipboardContentTypes type, UInt64 s, List<ElementSelector> elements) : base(s, elements)
        {
            Data = data;
            Type = type;
        }

        public override string ToString()
        {
            return $"{SequenceNumber}. ({nameof(ClipboardEvent)}) \nType:{Type} \nData:\n{Data} \n {Selectors()}";
        }
    }
}
