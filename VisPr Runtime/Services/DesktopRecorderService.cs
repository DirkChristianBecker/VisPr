using System.Diagnostics;

namespace VisPr_Runtime.Services
{
    public interface IDesktopRecorderService
    {
        Task LaunchRecorderApplication();
        Task TerminateRecorderApplication();
    }

    public class DesktopRecorderService : IDesktopRecorderService
    {
        private readonly ILogger<DesktopRecorderService> Logger;
        private IRuntimeLocation RuntimeLocation { get; set; }
        private Process? Application { get; set; }

        public DesktopRecorderService(
            ILogger<DesktopRecorderService> log,
            IRuntimeLocation location)
        {
            Logger = log;
            RuntimeLocation = location;
        }

        public async Task LaunchRecorderApplication()
        {
            await TerminateRecorderApplication();
            await Task.Run(() =>
            {
                var path = RuntimeLocation.GetWindowsRecorderExecutablePath();
                Logger.LogInformation("Launching recorder application.");
                Application = Process.Start(path);
                Application.WaitForInputIdle(2000);

                Logger.LogInformation("Recorder application started");
            });
        }

        public async Task TerminateRecorderApplication()
        {
            await Task.Run(() =>
            {
                if (Application != null)
                {
                    Application.Kill();
                    Application = null;

                    Logger.Log(LogLevel.Information, "Recorder application stopped");
                }
            });
        }
    }
}
