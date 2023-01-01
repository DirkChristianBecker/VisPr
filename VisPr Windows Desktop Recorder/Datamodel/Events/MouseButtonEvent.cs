using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VisPrWindowsDesktopRecorder.Datamodel.Events
{
    public enum MouseButton
    {
        None,
        Left,
        Right,
        Wheel
    }

    public class MouseButtonEvent : RecordedEvent
    {
        public KeyButtonType ButtonType { get; set; }
        public MouseButton Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MouseButtonEvent() : this(0, 0, 0, KeyButtonType.None, MouseButton.None, new List<ElementSelector>())
        {

        }

        /// <summary>
        /// Checks, if the right hand side is the mouse up for this mouse down event.
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool IsMouseUp(MouseButtonEvent rhs)
        {
            if(ButtonType != KeyButtonType.Down)
            {
                return false;
            }

            if(rhs.ButtonType != KeyButtonType.Up)
            {
                return false;
            }

            if(Button != rhs.Button)
            {
                return false;
            }

            if(ElementSelectors.Count < 1 || rhs.ElementSelectors.Count < 1)
            {
                return true;
            }

            if (ElementSelectors[0].Property == "AutomationId" &&
                rhs.ElementSelectors[0].Property == "AutomationId")
            {
                return ((object)ElementSelectors[0].Value).Equals((object)rhs.ElementSelectors[0].Value);
            }

            if (ElementSelectors.Count != rhs.ElementSelectors.Count)
            {
                return false;
            }

            for (int i = 0; i < ElementSelectors.Count; i++)
            {
                if (ElementSelectors[i] == rhs.ElementSelectors[i])
                {
                    continue;
                }

                return false;
            }

            return SequenceNumber + 1 == rhs.SequenceNumber;
        }

        /// <summary>
        /// Turns this event into a mouse click event.
        /// 
        /// </summary>
        /// <returns></returns>
        public MouseClickEvent ToClick()
        {
            return new MouseClickEvent(SequenceNumber, Button, ElementSelectors);
        }

        public MouseButtonEvent(
            UInt64 s,
            int x,
            int y,
            KeyButtonType t,
            MouseButton b,
            List<ElementSelector> elements) : base(s, elements)
        {
            X = x;
            Y = y;
            ButtonType = t;
            Button = b;
        }

        public override string ToString()
        {
            return $"{SequenceNumber}. {ButtonType} Button: {Button} X: {X} Y: {Y} ({nameof(MouseButtonEvent)}) \n {Selectors()}";
        }
    }
}
