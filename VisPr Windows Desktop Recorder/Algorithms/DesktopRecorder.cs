using EventHook;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA2;
using FlaUI.UIA3;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;

using VisPrWindowsDesktopRecorder.Datamodel.Events;

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
        private ClipboardWatcher ClipboardWatcher { get; set; }

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
            ClipboardWatcher = Factory.GetClipboardWatcher();

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
            ClipboardWatcher.Start();

            KeyboardWatcher.OnKeyInput += OnKeyEvent;
            MouseWatcher.OnMouseInput += OnMouseEvent;
            ClipboardWatcher.OnClipboardModified += OnClipboardEvent;

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

        public string Stop()
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
            ClipboardWatcher.OnClipboardModified -= OnClipboardEvent;

            KeyboardWatcher.Stop();
            MouseWatcher.Stop();
            ClipboardWatcher.Stop();

            RecordedEvents.Add(
                new RecorderEvent(
                    SequenceNumber++,
                    RecorderMessage.RecordingStopped,
                    null));

            // Collect events and combine those which logically fit together like
            // clicks, keyboard input etc.
            var r = new List<RecordedEvent>();
            for(int i = 0; i < RecordedEvents.Count; i++)
            {
                var element = RecordedEvents[i];
                if(element is RecorderEvent recorderEvent)
                {
                    continue;
                }

                if(element is MouseButtonEvent mouseButtonEvent)
                {
                    // Check if there is a button up event
                    if ((i + 1) >= RecordedEvents.Count)
                    {
                        r.Add(element);
                    }

                    if (!(RecordedEvents[i + 1] is MouseButtonEvent next))
                    {
                        r.Add(mouseButtonEvent);
                        continue;
                    }
                    if (mouseButtonEvent.IsMouseUp(next))
                    {
                        r.Add(mouseButtonEvent.ToClick());
                        i++;
                        continue;
                    }

                    r.Add(element);
                }

                if(element is TextEvent keyBoardEvent)
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

                        if (!keyBoardEvent.IsText(nextEvent))
                        {
                            break;
                        }

                        keyBoardEvent = nextEvent;
                        i++;
                    }

                    WriteTextEvent x = null;
                    if (nextEvent != null)
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
                    continue;
                }

                r.Add(element);
            }

            var jsonString = JsonSerializer.Serialize(r);
            Console.WriteLine("JSON-Result: " + jsonString);

            LastHoveredElement = null;
            RecordingStopped?.Invoke(this, new EventArgs());

            return jsonString;
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

        private AutomationElement GetHoveredElement(int x, int y)
        {
            var e = Automation.FromPoint(new System.Drawing.Point(x, y));
            if(e == null)
            {
                if (LastHoveredElement != null)
                {
                    LastHoveredElement = null;
                    LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                }
                return null;
            }

            if(!e.Properties.ProcessId.IsSupported)
            {
                if (LastHoveredElement != null)
                {
                    LastHoveredElement = null;
                    LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                }
                return null;
            }

            if(e.Properties.ProcessId.ValueOrDefault != MainWindow.Properties.ProcessId.ValueOrDefault)
            {
                if (LastHoveredElement != null)
                {
                    LastHoveredElement = null;
                    LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                }

                return null;
            }

            if(e == LastHoveredElement)
            {
                return null;
            }

            LastHoveredElement = e;
            LastHoveredElementChanged?.Invoke(this, LastHoveredElement);

            return e;
        }

        private AutomationElement GetFocusedElement()
        {
            var focus = Automation.FocusedElement();
            if (focus == null || !focus.Properties.CenterPoint.IsSupported)
            {
                if (LastHoveredElement != null)
                {
                    LastHoveredElement = null;
                    LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                }

                return null;
            }

            var point = focus.Properties.CenterPoint.ValueOrDefault;
            if(point.IsEmpty)
            {
                if (LastHoveredElement != null)
                {
                    LastHoveredElement = null;
                    LastHoveredElementChanged.Invoke(this, LastHoveredElement);
                }

                return null;
            }

            return GetHoveredElement(point.X, point.Y);
        }

        private void Record(EventHook.MouseEventArgs e, KeyButtonType type, MouseButton b)
        {
            var evt = new MouseButtonEvent(
                SequenceNumber++, 
                e.Point.x, 
                e.Point.y, 
                type, 
                b, 
                LastHoveredElement.Seletors());

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

        private void OnKeyEvent(object sender, EventHook.KeyInputEventArgs e)
        {
            var focus = GetFocusedElement();
            if(focus == null)
            {
                return;
            }

            RecordedEvent evt;
            if (focus.IsTextElement())
            {
                evt = new TextEvent(
                    focus.AsTextBox().Text,
                    SequenceNumber++,
                    e.KeyData.EventType == KeyEvent.up ? KeyButtonType.Up : KeyButtonType.Down,
                    e.KeyData.Keyname,
                    e.KeyData.UnicodeCharacter,
                    focus.Seletors());
            }
            else
            {
                evt = new KeyboardEvent(
                    SequenceNumber++,
                    e.KeyData.EventType == KeyEvent.up ? KeyButtonType.Up : KeyButtonType.Down,
                    e.KeyData.Keyname,
                    e.KeyData.UnicodeCharacter,
                    focus.Seletors());
            }

            RecordedEvents.Add(evt);
        }

        private void OnClipboardEvent(object sender, ClipboardEventArgs e)
        {
            var focus = GetFocusedElement();
            if (focus == null)
            {
                return;
            }

            // var evt = new ClipboardEvent(e.Data, e.DataFormat, SequenceNumber++, focus.Seletors());
        }
    }
}
