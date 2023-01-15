using System.Reflection;
using VisPrCore.Datamodel;

namespace VisPr_Runtime.Services
{
    public interface IRuntimeLocation
    {
        /// <summary>
        /// Get the directory the runtime was started from.
        /// 
        /// </summary>
        /// <returns></returns>
        string GetExecutableDirectory();

        /// <summary>
        /// Get the absolute path of the windows recorder executable. It will interpret
        /// the WindowsRecorderPath value from the configuration as a relative path
        /// from the current path to the executable of the runtime.
        /// 
        /// </summary>
        /// <returns></returns>
        string GetWindowsRecorderExecutablePath();
    }

    public class RuntimeLocation : IRuntimeLocation
    {
        private IConfiguration Configuration { get; set; }
        private ILogger<RuntimeLocation> Logger { get; set; }

        public RuntimeLocation(IConfiguration config, ILogger<RuntimeLocation> logger) 
        {
            Configuration = config;
            Logger = logger;
        }

        public string GetExecutableDirectory()
        {
            string strExeFilePath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(strExeFilePath) ?? "Could not get directory name from assembly path.";
        }

        public string GetWindowsRecorderExecutablePath()
        {
            var path = Configuration.ReadWindowsRecorderPath();
            var r =  Path.Combine(GetExecutableDirectory(), path);

            Logger.LogInformation("Path to recorder: " + r);

            return r;
        }
    }
}
