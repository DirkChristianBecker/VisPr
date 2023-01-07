using FlaUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using FlaUI.UIA2;
using FlaUI.UIA3;
using EventHook;
using FlaUI.Core.AutomationElements;
using VisPrWindowsDesktopRecorder.Datamodel.Events;
using System.Windows.Forms;
using System.Globalization;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public class DesktopRecorder
    {
        public event EventHandler<AutomationElement> LastHoveredElementChanged;
        public event EventHandler RecordingStopped;

        public Window MainWindow { get; private set; }
        public FlaUI.Core.Application Application { get; private set; }
        private AutomationBase Automation { get; set; }

        private EventHookFactory Factory { get; set; }
        private KeyboardWatcher KeyboardWatcher { get; set; }
        private MouseWatcher MouseWatcher { get; set; }

        private UInt64 SequenceNumber { get; set; }
        private List<RecordedEvent> RecordedEvents { get; set; }
        public RecorderMessage State { get; private set; }

        private AutomationElement LastHoveredElement { get; set; }

        public DesktopRecorder(int attach_to, AutomationType backend) 
        {
            if(backend == AutomationType.UIA2)
            {
                Automation = new UIA2Automation();
            }
            else
            {
                Automation = new UIA3Automation();
            }

            Application = FlaUI.Core.Application.Attach(attach_to);
            Application.WaitWhileMainHandleIsMissing();
            MainWindow = Application.GetMainWindow(Automation);
            
            Factory = new EventHookFactory();
            KeyboardWatcher = Factory.GetKeyboardWatcher();
            MouseWatcher = Factory.GetMouseWatcher();

            SequenceNumber = 0;
            RecordedEvents = new List<RecordedEvent>();

            State = RecorderMessage.RecordingStopped;
        }

        public DesktopRecorder(string name, string args, AutomationType backend) : 
            this(FlaUI.Core.Application.Launch(name, args).ProcessId, backend)
        {
        }

        ~DesktopRecorder()
        {
            if (State != RecorderMessage.RecordingStopped)
                Stop();

            Factory.Dispose();
        }

        public void Start()
        {
            if(State == RecorderMessage.RecordingStarted || State == RecorderMessage.RecordingResumed)
            {
                throw new InvalidOperationException("Already recording");
            }

            KeyboardWatcher.Start();
            MouseWatcher.Start();

            KeyboardWatcher.OnKeyInput += OnKeyEvent;
            MouseWatcher.OnMouseInput += OnMouseEvent;

            SequenceNumber = 0;

            State = RecorderMessage.RecordingStarted;
            RecordedEvents.Clear();

            LastHoveredElement = null;

            RecordedEvents.Add(
                new RecorderEvent(
                    SequenceNumber++, 
                    RecorderMessage.RecordingStarted, 
                    null));
        }

        public void Stop()
        {
            if (State == RecorderMessage.RecordingStopped)
            {
                throw new InvalidOperationException("Already stopped");
            }

            if (!(State == RecorderMessage.RecordingStarted || 
                  State == RecorderMessage.RecordingResumed))
            {
                throw new InvalidOperationException("Not recording");
            }

            KeyboardWatcher.OnKeyInput -= OnKeyEvent;
            MouseWatcher.OnMouseInput -= OnMouseEvent;

            KeyboardWatcher.Stop();
            MouseWatcher.Stop();

            RecordedEvents.Add(
                new RecorderEvent(
                    SequenceNumber++,
                    RecorderMessage.RecordingStopped,
                    null));

            var r = new List<RecordedEvent>();
            for(int i = 0; i < RecordedEvents.Count; i++)
            {
                var element = RecordedEvents[i];
                var recorderEvent = element as RecorderEvent;
                if(recorderEvent != null) 
                {
                    continue;
                }

                var mouseButtonEvent = element as MouseButtonEvent;
                if (mouseButtonEvent != null)
                {
                    // Check if there is a button up event
                    if((i + 1) >= RecordedEvents.Count) 
                    {
                        r.Add(element);
                    }

                    var next = RecordedEvents[i + 1] as MouseButtonEvent;
                    if(next == null) 
                    {
                        r.Add(mouseButtonEvent);
                        continue;
                    }
                    if(mouseButtonEvent.IsMouseUp(next))
                    {
                        r.Add(mouseButtonEvent.ToClick());
                        i++;
                        continue;
                    }

                    r.Add(element);
                }

                var keyBoardEvent = element as TextEvent;
                if(keyBoardEvent != null) 
                {
                    // Combine all keyboard events to the same UI-Element into one
                    // write text event.
                    TextEvent nextEvent = null;
                    for (int j = i + 1; j < RecordedEvents.Count; j++)
                    {                        
                        nextEvent = RecordedEvents[j] as TextEvent;
                        if (nextEvent == null)
                        {
                            break;
                        }

                        if(!keyBoardEvent.IsText(nextEvent))
                        {
                            break;
                        }

                        keyBoardEvent = nextEvent;
                        i++;
                    }

                    WriteTextEvent x = null;
                    if(nextEvent != null)
                    {
                        x = new WriteTextEvent(
                            nextEvent.CurrentUiElmentText, 
                            nextEvent.SequenceNumber, 
                            nextEvent.ElementSelectors);
                    }
                    else
                    {
                        x = new WriteTextEvent(
                            keyBoardEvent.CurrentUiElmentText, 
                            keyBoardEvent.SequenceNumber, 
                            keyBoardEvent.ElementSelectors);
                    }
                    
                    r.Add(x);
                }
            }

            foreach(var e in r) 
            {
                Console.WriteLine(e.ToString());
            }

            LastHoveredElement = null;
            RecordingStopped?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            if (State == RecorderMessage.RecordingPaused)
            {
                throw new InvalidOperationException("Already paused");
            }

            if (!(State == RecorderMessage.RecordingStarted || 
                  State == RecorderMessage.RecordingResumed))
            {
                throw new InvalidOperationException("Not recording");
            }

            KeyboardWatcher.Stop();
            MouseWatcher.Stop();

            RecordedEvents.Add(
                new RecorderEvent(
                    SequenceNumber++, 
                    RecorderMessage.RecordingPaused,
                    null));
        }

        public void Resume()
        {
            if (State != RecorderMessage.RecordingPaused)
            {
                throw new InvalidOperationException("Recording not paused");
            }

            KeyboardWatcher.Start();
            MouseWatcher.Start();

            RecordedEvents.Add(
                new RecorderEvent(
                    SequenceNumber++, 
                    RecorderMessage.RecordingResumed,
                    null));
        }

        private void OnKeyEvent(object sender, EventHook.KeyInputEventArgs e)
        {
            var focus = Automation.FocusedElement();
            var selectors = ElementSelector.From(focus);

            RecordedEvent evt = null;
            if(focus.IsTextElement())
            {
                evt = new TextEvent(
                    focus.AsTextBox().Text,
                    SequenceNumber++,
                    e.KeyData.EventType == KeyEvent.up ? KeyButtonType.Up : KeyButtonType.Down,
                    e.KeyData.Keyname,
                    e.KeyData.UnicodeCharacter,
                    selectors);
            }
            else
            {
                evt = new KeyboardEvent(
                    SequenceNumber++,
                    e.KeyData.EventType == KeyEvent.up ? KeyButtonType.Up : KeyButtonType.Down,
                    e.KeyData.Keyname,
                    e.KeyData.UnicodeCharacter,
                    selectors);
            }
            
            RecordedEvents.Add(evt);
        }

        private AutomationElement GetHoveredElement(int x, int y)
        {
            var e = Automation.FromPoint(new System.Drawing.Point(x, y));
            if(e == null)
            {
                LastHoveredElement = null;
                LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                return null;
            }

            if(e.Properties.ProcessId.ValueOrDefault != MainWindow.Properties.ProcessId.ValueOrDefault)
            {
                LastHoveredElement = null;
                LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                return null;
            }

            if(e == LastHoveredElement)
            {
                return null;
            }

            LastHoveredElement = e;
            if(LastHoveredElementChanged != null)
            {
                LastHoveredElementChanged.Invoke(this, LastHoveredElement);
            }

            return e;
        }

        private void Record(EventHook.MouseEventArgs e, KeyButtonType type, MouseButton b)
        {
            var selectors = ElementSelector.From(LastHoveredElement);
            var evt = new MouseButtonEvent(SequenceNumber++, e.Point.x, e.Point.y, type, b, selectors);
            RecordedEvents.Add(evt);
        }

        private void OnMouseEvent(object sender, EventHook.MouseEventArgs e)
        {
            switch (e.Message)
            {
                case EventHook.Hooks.MouseMessages.WM_MOUSEMOVE:
                    {
                        GetHoveredElement(e.Point.x, e.Point.y);
                        break;
                    }
                case EventHook.Hooks.MouseMessages.WM_LBUTTONUP: { Record(e, KeyButtonType.Up, MouseButton.Left); break; }
                case EventHook.Hooks.MouseMessages.WM_LBUTTONDOWN: { Record(e, KeyButtonType.Down, MouseButton.Left); break; }
                case EventHook.Hooks.MouseMessages.WM_RBUTTONUP: { Record(e, KeyButtonType.Up, MouseButton.Right); break; }
                case EventHook.Hooks.MouseMessages.WM_RBUTTONDOWN: { Record(e, KeyButtonType.Down, MouseButton.Right); break; }
                case EventHook.Hooks.MouseMessages.WM_WHEELBUTTONUP: { Record(e, KeyButtonType.Up, MouseButton.Wheel); break; }
                case EventHook.Hooks.MouseMessages.WM_WHEELBUTTONDOWN: { Record(e, KeyButtonType.Down, MouseButton.Wheel); break; }

                default:
                    {
                        break;
                    }
            }
        }
    }
}
