using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public class StartRecordingEventArgs : EventArgs
    {
        public string ApplicationPath { get; set; }
        public string StartupParameters { get; set; }
    }

    internal class RecorderPipeClient
    {
        private string PipeName { get; set; }
        private NamedPipeClientStream Client { get; set; }
        private CancellationTokenSource CancellationSource { get; set; }
        private Task NetworkListener { get; set; }

        /// <summary>
        /// Messages we send to the server
        /// </summary>
        private ConcurrentQueue<string> Messages { get; set; }


        /// <summary>
        /// Invoked, if the we receive a start recording message.
        /// </summary>
        public event EventHandler<StartRecordingEventArgs> OnStartRecordingRequested;
        public event EventHandler OnStopRecordingRequested;
        public event EventHandler OnPauseRecordingRequested;
        public event EventHandler OnResumeRecordingRequested;

        public RecorderPipeClient() : this("WindowsRecorderPipe")
        {

        }

        public RecorderPipeClient(string pipe) 
        {
            PipeName = pipe;
            CancellationSource = new CancellationTokenSource();
            Messages = new ConcurrentQueue<string>();
        }

        public void Start()
        {
            if(Client != null)
            {
                Stop();
            }

            Client = new NamedPipeClientStream(PipeName);
            NetworkListener = Task.Factory.StartNew(() => 
            {
                var cancel = CancellationSource.Token;
                Client.Connect();
                StreamReader reader = new StreamReader(Client);
                StreamWriter writer = new StreamWriter(Client);
                Messages.Enqueue("Hello?");

                while (!cancel.IsCancellationRequested)
                {
                    string message;
                    if(Messages.TryDequeue(out message))
                    {
                        Console.WriteLine($"Sending message: {message}.");
                        writer.WriteLine(message);
                        writer.Flush();
                    }

                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line)) 
                    {
                        HandleMessage(line);
                    }
                }

                writer.WriteLine("Bye!");
                writer.Flush();

                Client.Close();
            });
        }

        public void Stop()
        {
            if (Client == null)
            {
                return;
            }

            CancellationSource.Cancel();
            NetworkListener.Wait(2000);

            Client = null;
        }

        public void SendMessage(string message)
        {
            if(string.IsNullOrEmpty(message))
            {
                return;
            }

            Messages.Enqueue(message);
        }

        public void HandleMessage(string message)
        {
            var tokens = message.Split(' ');
            if(tokens.Length < 1) 
            {
                return;
            }

            switch(tokens[0]) 
            {
            case "start":
                {
                    if(tokens.Length == 1)
                    {
                        if (OnResumeRecordingRequested != null)
                        {
                            OnResumeRecordingRequested.Invoke(this, null);
                        }

                        Messages.Enqueue("resumed");
                        break;
                    }
                    if(tokens.Length < 2) 
                    {
                        return;
                    }

                    if(OnStartRecordingRequested == null)
                    {
                        return;
                    }

                    var args = new StartRecordingEventArgs();
                    args.ApplicationPath = tokens[1];
                    if(tokens.Length == 3) 
                    {
                        args.StartupParameters = tokens[2];
                    }

                    OnStartRecordingRequested.Invoke(this, args);
                    Messages.Enqueue("started");

                    break;
                }
            case "stop":
                {
                    if(OnStopRecordingRequested != null) 
                    {
                        OnStopRecordingRequested(this, null);
                    }

                    Messages.Enqueue("stopped");

                    break;
                }
            case "pause":
                {
                    if(OnPauseRecordingRequested != null)
                    {
                        OnPauseRecordingRequested.Invoke(this, null);
                    }
                    Messages.Enqueue("paused");

                    break;
                }
            case "Hello!":
                {
                    Console.WriteLine("Connected to the server");
                    break;
                }
            }
        }
    }
}
