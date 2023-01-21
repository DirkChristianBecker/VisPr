using System;
using System.Windows.Forms;
using VisPrWindowsDesktopRecorder.Algorithms;

namespace VisPrWindowsDesktopRecorder
{
    public partial class MainWindow : Form
    {
        private Overlay Overlay { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            btnStartRecording.Enabled = true;
            btnStopRecording.Enabled = false;
            btnPauseRecording.Enabled = false;
        }      

        private void OnClickPause(object sender, EventArgs e)
        {
            if (Overlay == null)
            {
                return;
            }

            OnPauseButtons();

            Overlay.PauseRecording();
        }

        private void OnClickStop(object sender, EventArgs e)
        {
            if (Overlay == null)
            {
                return;
            }

            OnStopButtons();

            Overlay.StoppRecording();
            Overlay.Close();
            Overlay = null;
        }

        private void OnClickStart(object sender, EventArgs e)
        {
            if(Overlay != null)
            {
                return;
            }

            OnStartButtons();

            try
            {
                Overlay = new Overlay();
                Overlay.StartRecording();
            }
            catch(Exception) 
            {
                /// Todo: Send a message back to caller.
                OnStopButtons();
                OnClickStop(sender, e);
            }
        }

        private void OnStopButtons()
        {
            btnStartRecording.Enabled = true;
            btnStopRecording.Enabled = false;
            btnPauseRecording.Enabled = false;
        }

        private void OnStartButtons()
        {
            btnStartRecording.Enabled = false;
            btnStopRecording.Enabled = true;
            btnPauseRecording.Enabled = true;
        }

        private void OnPauseButtons()
        {
            btnStartRecording.Enabled = true;
            btnStopRecording.Enabled = true;
            btnPauseRecording.Enabled = false;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
