using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class MouseClickEvent : RecordedEvent
    {
        public MouseButton Button { get; set; }

        public MouseClickEvent() : this(0, MouseButton.None, new List<ElementSelector>())
        {

        }

        public MouseClickEvent(UInt64 id, MouseButton b, List<ElementSelector> selectors) : base(id, selectors)
        {
            Button = b;
        }

        public override string ToString()
        {
            return $"{SequenceNumber}. Button: {Button} ({nameof(MouseClickEvent)}) \n {Selectors()}";
        }
    }
}
