using Microsoft.Owin.Hosting;
using System;
using System.Windows.Forms;


namespace VisPrWindowsDesktopRecorder
{
    public partial class MainWindow : Form
    {
        private Overlay Overlay { get; set; }
        private IDisposable WebService { get; set; }
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            System.Threading.Tasks.Task.Run(() => 
            {
                string baseAddress = "http://localhost:9000/";
                WebService = WebApp.Start<Algorithms.WebServiceStartup>(url: baseAddress);
            });

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

        public void OnClickStop(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke((MethodInvoker) delegate 
                {
                    OnClickStop(sender, e);
                });

                return;
            }

            Stop();
        }

        private void Stop()
        {
            OnStopButtons();
            if(Overlay == null)
            {
                return;
            }

            Overlay.StopRecording();
            Overlay.Close();
            Overlay = null;
        }

        public void OnClickStart(object sender, EventArgs e)
        {
            if(this.InvokeRequired)
            {
                Invoke((MethodInvoker) delegate 
                {
                    OnClickStart(sender, e);
                });

                return;
            }

            Start();
        }

        private void Start(string application = "notepad.exe", string args = null)
        {
            if (Overlay != null)
            {
                return;
            }

            OnStartButtons();

            try
            {
                Overlay = new Overlay();
                Overlay.StartRecording(application, args);
            }
            catch (Exception)
            {
                OnStopButtons();
                OnClickStop(this, EventArgs.Empty);
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
            Stop();

            WebService.Dispose();
            WebService = null;
        }

        public void StartRecording(string application, string args)
        {
            Start(application, args);
        }

        public void StopRecording()
        {

        }

        public void PauseRecording()
        {
            OnClickPause(this, EventArgs.Empty);
        }
    }
}
