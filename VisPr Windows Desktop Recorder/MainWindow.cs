using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            btnStartRecording.Enabled = true;
            btnStopRecording.Enabled = true;
            btnPauseRecording.Enabled = false;
            Overlay.PauseRecording();
        }

        private void OnClickStop(object sender, EventArgs e)
        {
            if(Overlay == null)
            {
                return;
            }

            btnStartRecording.Enabled = true;
            btnStopRecording.Enabled = false;
            btnPauseRecording.Enabled = false;

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

            btnStartRecording.Enabled = false;
            btnStopRecording.Enabled = true;
            btnPauseRecording.Enabled = true;

            Overlay = new Overlay();
            Overlay.StartRecording();
        }
    }
}
