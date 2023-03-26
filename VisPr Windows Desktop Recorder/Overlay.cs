using FlaUI.Core.AutomationElements;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using VisPrWindowsDesktopRecorder.Algorithms;

namespace VisPrWindowsDesktopRecorder
{
    public partial class Overlay : Form
    {
        private DesktopRecorder Recorder { get; set; }

        private bool DrawHighlight { get; set; }
        private Rectangle ScreenRect { get; set; }

        public Overlay()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            
            TopMost = true;                    
            Visible = true;

            int initialStyle = Win32.GetWindowLong(this.Handle, -20);
            Win32.SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        public void StartRecording(string application = "notepad.exe", string args = null)
        {
            if(Recorder != null) 
            {
                if(Recorder.State == Datamodel.Events.RecorderMessage.RecordingPaused) 
                {
                    Recorder.Resume();
                }
                return;
            }

            Recorder = new DesktopRecorder("notepad.exe", args, FlaUI.Core.AutomationType.UIA3);
            Recorder.Start();
            Recorder.LastHoveredElementChanged += OnLastHoveredElementChanged;

            var window = Recorder.MainWindow;
            var bounds = window.Properties.BoundingRectangle.ValueOrDefault;
            if(bounds.IsEmpty)
            {
                Console.WriteLine("Could not get the bounds of the main window.");
                StopRecording();
                return;
            }

            Location = bounds.Location;
            Size = bounds.Size;
        }

        public void StopRecording()
        {
            Recorder.LastHoveredElementChanged -= OnLastHoveredElementChanged;
            Recorder.Stop();
        }

        public void PauseRecording()
        {
            if(Recorder == null)
            {
                return;
            }

            if(Recorder.State != Datamodel.Events.RecorderMessage.RecordingPaused)
            {
                return;
            }

            Recorder.Pause();
        }

        private void OnLastHoveredElementChanged(object sender, AutomationElement e)
        {
            if(InvokeRequired) 
            {
                Invoke(new Action(() => { this.OnLastHoveredElementChanged(sender, e); }));
                return;
            }

            if(e == null)
            {
                DrawHighlight = false;
                Refresh();
                return;
            }

            DrawHighlight = true;
            var refresh = ScreenRect != e.Properties.BoundingRectangle.ValueOrDefault;
            ScreenRect = e.Properties.BoundingRectangle.ValueOrDefault;

            if(refresh)
            {
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!DrawHighlight)
            {
                return;
            }

            if(ScreenRect.IsEmpty) 
            {
                Console.WriteLine("Invalid rect");
                return; 
            }
            
            var p = new Pen(Color.Red, 3);
            var l = PointToClient(ScreenRect.Location);
            e.Graphics.DrawRectangle(p, l.X, l.Y, ScreenRect.Width, ScreenRect.Height);
        }

        // Borrowed from: 
        // https://github.com/open-rpa/openrpa/blob/master/OpenRPA.Interfaces/Overlay/OverlayWindow.cs
        protected override void CreateHandle()
        {
            try
            {
                base.CreateHandle();
            }
            catch (Exception)
            {
                return;
            }

            // Note: We need this because the Form.TopMost property does not respect
            // the "ShowWithoutActivation" flag, meaning the window will steal the
            // focus every time it is made visible.
            SetTopMost(new HandleRef(this, Handle), true);
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
        public static HandleRef HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
        public const int SWP_NOSIZE = 1;
        public const int SWP_NOMOVE = 2;
        public const int SWP_NOZORDER = 4;
        public const int SWP_NOACTIVATE = 16;
        public static void SetTopMost(HandleRef handleRef, bool value)
        {
            var key = value ? HWND_TOPMOST : HWND_NOTOPMOST;
            var result = Win32.SetWindowPos(handleRef, key, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            if (!result)
            {
                throw Win32.GetLastWin32Error(nameof(SetTopMost));
            }
        }
        
    }
}
