using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public class RecordedEvent
    {
        public UInt64 SequenceNumber { get; set; }
        public List<ElementSelector> ElementSelectors { get; set; }

        public RecordedEvent(UInt64 sequenceNumber, List<ElementSelector> elements)
        {
            SequenceNumber = sequenceNumber;
            ElementSelectors = elements;
        }

        private RecordedEvent() : this(0, new List<ElementSelector>()) { }

        protected string Selectors()
        {
            if(ElementSelectors == null || ElementSelectors.Count < 1)
            {
                return "\tNo Selectors";
            }

            var r = new StringBuilder();
            foreach (var element in ElementSelectors) 
            {
                r.Append("\t");
                r.AppendLine(element.ToString());
            }

            return r.ToString();
        }
    }
}
