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
using VisPr_Core.Datamodel.Events;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    internal class DesktopRecorder
    {
        private Window MainWindow { get; set; }
        private FlaUI.Core.Application Application { get; set; }
        private AutomationBase Automation { get; set; }

        private EventHookFactory Factory { get; set; }
        private KeyboardWatcher KeyboardWatcher { get; set; }
        private MouseWatcher MouseWatcher { get; set; }

        private UInt64 SequenceNumber { get; set; }
        private List<RecordedEvent> RecordedEvents { get; set; }
        private RecorderMessage State { get; set; }

        public DesktopRecorder(int attach_to, AutomationType backend) 
        {
            Automation = backend == AutomationType.UIA2 ? new UIA2Automation() : new UIA3Automation();
            Application = FlaUI.Core.Application.Attach(attach_to);
            MainWindow = Application.GetMainWindow(Automation);

            Factory = new EventHookFactory();
            KeyboardWatcher = Factory.GetKeyboardWatcher();
            MouseWatcher = Factory.GetMouseWatcher();

            SequenceNumber = 0;
            RecordedEvents = new List<RecordedEvent>();

            State = RecorderMessage.RecordingStopped;
        }

        ~DesktopRecorder()
        {
            if(State != RecorderMessage.RecordingStopped)
                Stop();

            Factory.Dispose();
        }

        public DesktopRecorder(string name, AutomationType backend) : 
            this(FlaUI.Core.Application.Launch(name).ProcessId, backend)
        {
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
            RecordedEvents.Add(new RecorderEvent(SequenceNumber++, RecorderMessage.RecordingStarted));

            State = RecorderMessage.RecordingStarted;
            RecordedEvents.Clear();
        }

        public void Stop()
        {
            if (State == RecorderMessage.RecordingStopped)
            {
                throw new InvalidOperationException("Already stopped");
            }

            if (!(
                State == RecorderMessage.RecordingStarted || 
                State == RecorderMessage.RecordingResumed))
            {
                throw new InvalidOperationException("Not recording");
            }

            KeyboardWatcher.OnKeyInput -= OnKeyEvent;
            MouseWatcher.OnMouseInput -= OnMouseEvent;

            KeyboardWatcher.Stop();
            MouseWatcher.Stop();

            RecordedEvents.Add(new RecorderEvent(SequenceNumber++, RecorderMessage.RecordingStopped));

            foreach(var e in RecordedEvents)
            {
                Console.WriteLine(e);
            }
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

            RecordedEvents.Add(new RecorderEvent(SequenceNumber++, RecorderMessage.RecordingPaused));
        }

        public void Resume()
        {
            if (State != RecorderMessage.RecordingPaused)
            {
                throw new InvalidOperationException("Recording not paused");
            }

            KeyboardWatcher.Start();
            MouseWatcher.Start();

            RecordedEvents.Add(new RecorderEvent(SequenceNumber++, RecorderMessage.RecordingResumed));
        }

        private void OnKeyEvent(object? sender, EventHook.KeyInputEventArgs e)
        {
            var evt = new KeyboardEvent(
                SequenceNumber++, 
                e.KeyData.EventType == KeyEvent.up ? KeyButtonType.Up : KeyButtonType.Down,
                e.KeyData.Keyname,
                e.KeyData.UnicodeCharacter);
            
            RecordedEvents.Add(evt);
        }

        private void Record(EventHook.MouseEventArgs e, KeyButtonType type, MouseButton b)
        {
            var evt = new MouseButtonEvent(SequenceNumber++, e.Point.x, e.Point.y, type, b);
            RecordedEvents.Add(evt);
        }

        private void OnMouseEvent(object? sender, EventHook.MouseEventArgs e)
        {
            switch (e.Message)
            {
                case EventHook.Hooks.MouseMessages.WM_MOUSEMOVE:
                    {
                        var evt = new MouseMoveEvent(SequenceNumber++, e.Point.x, e.Point.y);
                        RecordedEvents.Add(evt);
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
