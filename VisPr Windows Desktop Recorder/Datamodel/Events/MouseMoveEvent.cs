using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class MouseMoveEvent : RecordedEvent
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MouseMoveEvent() : this(0, 0, 0, new List<ElementSelector>())
        {

        }

        public MouseMoveEvent(
            UInt64 s,
            int x,
            int y,
            List<ElementSelector> elements) : base(s, elements)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{SequenceNumber}. X: {X} Y: {Y} ({nameof(MouseMoveEvent)}) \n {Selectors()}";
        }
    }
}
