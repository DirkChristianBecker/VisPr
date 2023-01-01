using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public enum RecorderMessage
    {
        None,
        RecordingStarted,
        RecordingPaused,
        RecordingStopped,
        RecordingResumed
    }

    public class RecorderEvent : RecordedEvent
    {
        public RecorderMessage Message { get; set; }
        public RecorderEvent(
            UInt64 sequenceNumber,
            RecorderMessage message,
            List<ElementSelector> elements) : base(sequenceNumber, elements)
        {
            Message = message;
        }

        public RecorderEvent() : this(0, RecorderMessage.None, new List<ElementSelector>())
        {

        }

        public override string ToString()
        {
            return $"{SequenceNumber}. {Message} ({nameof(RecorderEvent)}) \n {Selectors()}";
        }
    }
}
