using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;
using VisPrRuntime.Services;
using VisPrCore.Datamodel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VisPr_Runtime.Services.DesktopRecorder
{
    public enum RecoderState
    {
        Stopped,
        Recording,
        Paused,
    }

    public class DesktopRecorderService
    {
        private ILogger<DesktopRecorder> Logger { get; set; }
        private IConfiguration Configuration { get; set; }
        private IQueryApplication QueryApplication { get; set; }
        private CancellationTokenSource CancellationToken { get; set; }

        public event EventHandler StopRecording = default!;

        public DesktopRecorderService(ILogger<DesktopRecorder> log, IConfiguration config, IQueryApplication apps)
        {
            Logger = log;
            Configuration = config;
            QueryApplication = apps;

            CancellationToken = new CancellationTokenSource();
        }

        public void Start(int application_id)
        {
            Task.Run(() => 
            {
                var recorder = new DesktopRecorder(Logger, Configuration, QueryApplication, CancellationToken.Token);
                StopRecording += recorder.Stop;
                recorder.Start(application_id);
                Task.Run(() => System.Windows.Forms.Application.Run());
            });
        }

        public void Stop()
        {
            CancellationToken.Cancel();
            StopRecording?.Invoke(this, new EventArgs());
            Task.Run(() => System.Windows.Forms.Application.Exit());
        }
    }

    public class DesktopRecorder
    {
        public RecoderState State { get; private set; }
        private ILogger<DesktopRecorder> Logger { get; set; }
        private IQueryApplication Applications { get; set; }

        private System.Drawing.Color HighlightColor { get; set; }
        private int HighlightTimeout { get; set; }

        private Application? Application { get; set; }
        private AutomationBase? Automation { get; set; }

        private List<RecordedEvent> RecordedEvents { get; set; }
        private AutomationElement? LastHoveredElement { get; set; }
        private CancellationToken CancellationToken { get; set; }

        public DesktopRecorder(ILogger<DesktopRecorder> log, IConfiguration config, IQueryApplication apps, CancellationToken cancellationToken)
        {
            Logger = log;
            Applications = apps;

            RecordedEvents = new List<RecordedEvent>();

            HighlightTimeout = config.ReadHighlightTimeout();
            HighlightColor = config.ReadHighlightColor();

            CancellationToken = cancellationToken;
        }

        public void Start(int applicationId)
        {
            lock (Applications)
            {
                if (State != RecoderState.Stopped)
                {
                    throw new Exception("Desktop recorder is already recording.");
                }

                State = RecoderState.Recording;
                try
                {
                    var (autom, app) = Applications[applicationId];
                    Application = app;
                    Automation = autom;

                    RecordedEvents.Clear();

                    LastHoveredElement = null;

                    Logger.Log(LogLevel.Information, "Recorder startet");
                }
                catch (Exception)
                {
                    State = RecoderState.Stopped;
                    RecordedEvents.Clear();
                    LastHoveredElement = null;

                    throw;
                }
            }
        }

        public void Stop(object? sender, EventArgs args)
        {
            lock (Applications)
            {
                State = RecoderState.Stopped;
                LastHoveredElement = null;

                foreach (var e in RecordedEvents)
                {
                    Logger.Log(LogLevel.Information, e.ToString());
                }

                RecordedEvents.Clear();

                Logger.Log(LogLevel.Information, "Recorder stopped");
            }
        }
    }
}
