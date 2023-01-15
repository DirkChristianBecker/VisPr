using System.Collections.Concurrent;
using System.IO.Pipes;
using VisPrCore.Datamodel;

namespace VisPr_Runtime.Services
{
    /// <summary>
    /// The named pipe server handles the communication between the windows recorder application
    /// (which allows us to visualize interaction with an application) and the runtime application.
    /// 
    /// </summary>
    public interface INamedPipeServer
    {
        event EventHandler OnServerStarted;
        event EventHandler OnServerStopped;

        event EventHandler OnClientConnected;
        event EventHandler OnClientDisconnected;

        event EventHandler OnRecorderStarted;
        event EventHandler OnRecorderStopped;

        bool IsClientConnected { get; }

        void SendStartRecorderMessage(string app_path, string? param);
        void SendStopRecorderMessage();
        void SendPauseRecorderMessage();
    }
    public class NamedPipeServer : IHostedService, INamedPipeServer
    {
        private static NamedPipeServer? mInstance;
        public static NamedPipeServer? Instance
        { 
            get 
            { 
                return mInstance; 
            }
        }

        public event EventHandler OnServerStarted = default!;
        public event EventHandler OnServerStopped = default!;

        public event EventHandler OnClientConnected = default!;
        public event EventHandler OnClientDisconnected = default!;

        public event EventHandler OnRecorderStarted = default!;
        public event EventHandler OnRecorderStopped = default!;

        public bool IsClientConnected { get; private set; }

        private ConcurrentQueue<string> Messages { get; set; }
        private ILogger<NamedPipeServer> Logger { get; set; }

        private NamedPipeServerStream? PipeServerStream { get; set; }
        private StreamReader? StreamReader { get; set; }
        private StreamWriter? StreamWriter { get; set; }


        public NamedPipeServer(ILogger<NamedPipeServer> logger) 
        {
            Messages = new ConcurrentQueue<string>();
            Logger = logger;

            mInstance = this;
        }

        private void SendServerStarted()
        {
            Logger.LogInformation("Name pipe server starting...");
            OnServerStarted?.Invoke(this, EventArgs.Empty);
        }

        private void SendServerStopped()
        {
            Logger.LogInformation("Name pipe server stopped.");
            OnServerStopped?.Invoke(this, EventArgs.Empty);
        }

        private void SendClientConnected()
        {
            Logger.LogInformation("Desktop recorder connected.");
            OnClientConnected?.Invoke(this, EventArgs.Empty);
            IsClientConnected = true;
        }

        private void SendClientDisconnected()
        {
            Logger.LogInformation("Desktop recorder disconnected.");
            OnClientDisconnected?.Invoke(this, EventArgs.Empty);
            IsClientConnected = false;
        }

        private (NamedPipeServerStream, StreamReader, StreamWriter) Restart()
        {
            if(PipeServerStream != null)
            {
                //StreamReader?.Close();
                //StreamWriter?.Close();
                PipeServerStream.Close();

                //StreamReader?.Dispose();
                //StreamWriter?.Dispose();
                PipeServerStream.Dispose();
            }

            SendServerStopped();
            var pipe = new NamedPipeServerStream(Names.WindowsRecorderPipeName);
            var reader = new StreamReader(pipe);
            var writer = new StreamWriter(pipe);
            SendServerStarted();

            return (pipe, reader, writer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                (PipeServerStream, StreamReader, StreamWriter) = Restart();
                while (true)
                {
                    if(!PipeServerStream.IsConnected) 
                    {
                        try
                        {
                            PipeServerStream.WaitForConnection();
                        }
                        catch(IOException ex)
                        {
                            if(ex.Message == "Pipe is broken.")
                            {
                                Logger.LogInformation($"Restarting pipes");
                                (PipeServerStream, StreamReader, StreamWriter) = Restart();
                                continue;
                            }

                            Logger.LogWarning($"{ex.GetType()}: {ex.Message}");
                        }
                        catch (Exception ex) 
                        {
                            Logger.LogWarning($"{ex.GetType()}: {ex.Message}");
                        }
                        
                        continue;
                    }

                    try
                    {
                        var line = StreamReader.ReadLine();
                        HandleMessage(line);

                        if (Messages.TryDequeue(out string? message))
                        {
                            StreamWriter.WriteLine(message);
                            StreamWriter.Flush();
                        }
                    }
                    catch(InvalidOperationException ex)
                    {
                        Logger.LogInformation($"InvalidOperationException: {ex.Message}");
                    }
                    catch(Exception ex) 
                    {
                        Logger.LogInformation($"Exception: {ex.Message}\nShutting down piped name server.");
                        break;
                    }
                }

                SendServerStopped();
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void HandleMessage(string? message) 
        {
            if(string.IsNullOrEmpty(message)) 
            {
                return;
            }

            string message1 = $"Received message: {message}";
            Logger.LogInformation(message1);

            switch(message) 
            {
                case "Hello?":
                    {
                        Messages.Enqueue("Hello!");
                        SendClientConnected();
                        break;
                    }
                case "Bye!":
                    {
                        SendClientDisconnected();
                        break;
                    }
            }
        }

        public void SendStartRecorderMessage(string app_path, string? param)
        {
            string message;
            if (param == null)
            {
                message = $"start {app_path}";
            }
            else
            {
                message = $"start {app_path} {param}";
            }

            Messages.Enqueue(message);
        }

        public void SendStopRecorderMessage()
        {
            Messages.Enqueue("stop");
        }

        public void SendPauseRecorderMessage()
        {
            Messages.Enqueue("pause");
        }
    }
}
