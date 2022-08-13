using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.FullTrust.Core.ChangeNotifier;
using Newtonsoft.Json;
using NittyGritty.Utilities;

namespace JumpPoint.Platform.Services
{
    public static partial class ChangeNotifierService
    {
        private const string PIPE_SERVER_NAME = "JumpPoint_ChangeNotifierServerPipe";
        private static ServerPipe server;
        private static readonly object listenersLock;
        private static readonly Dictionary<string, IList<Action<NotifyChange>>> listeners;

        static ChangeNotifierService()
        {
            server = null;
            listenersLock = new object();
            listeners = new Dictionary<string, IList<Action<NotifyChange>>>(StringComparer.OrdinalIgnoreCase);
        }

        public static void Connect()
        {
            Task.Run(() =>
            {
                var pipe = new NamedPipeServerStream($@"LOCAL\{PIPE_SERVER_NAME}", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                Debug.WriteLine("Waiting for connection");
                pipe.WaitForConnection();
                Debug.WriteLine("Connection established");
                server = new ServerPipe(pipe);
                Task.Run(() => Server(server));
            });
        }

        private static void Server(ServerPipe pipe)
        {
            try
            {
                Debug.WriteLine("Server thread running");
                while (pipe.Pipe.IsConnected)
                {
                    var message = pipe.Reader.ReadLine();
                    var data = CodeHelper.InvokeOrDefault(() => JsonConvert.DeserializeObject<NotifyChange>(message));
                    if (data != null)
                    {
                        lock (listenersLock)
                        {
                            if (data.Path != null && listeners.TryGetValue(data.Path, out var actions))
                            {
                                for (int i = 0; i < actions.Count; i++)
                                {
                                    actions[i](data);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Server error: {ex.Message}");
            }
        }

        public static void Disconnect()
        {
            try
            {
                server.Dispose();
                server = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Server Disconnect error: {ex.Message}");
            }
        }

        public static void Start(string path, Action<NotifyChange> action)
        {
            lock (listenersLock)
            {
                if (server != null && server.Pipe.IsConnected)
                {
                    Debug.WriteLine($"Requesting to listen for {path}");
                    var request = JsonConvert.SerializeObject(new MonitorChangeRequest
                    {
                        Path = path,
                        Release = false
                    });
                    server.Writer.WriteLine(request);
                    server.Writer.Flush();
                    Debug.WriteLine($"Listen request sent for {path}");
                }
                if (listeners.TryGetValue(path, out var actions))
                {
                    actions.Add(action);
                }
                else
                {
                    listeners.Add(path, new List<Action<NotifyChange>>() { action });
                }
            }
        }

        public static void Stop(string path, Action<NotifyChange> action)
        {
            lock (listenersLock)
            {
                if (server != null && server.Pipe.IsConnected)
                {
                    Debug.WriteLine($"Stop listening for {path}");
                    var request = JsonConvert.SerializeObject(new MonitorChangeRequest
                    {
                        Path = path,
                        Release = true
                    });
                    server.Writer.WriteLine(request);
                    Debug.WriteLine($"Stop listening request sent for {path}");
                }
                if (listeners.TryGetValue(path, out var actions))
                {
                    actions.Remove(action);
                }
            }
        }
    }

    public class ServerPipe : IDisposable
    {
        public ServerPipe(NamedPipeServerStream pipe)
        {
            Pipe = pipe;
            Reader = new StreamReader(pipe);
            Writer = new StreamWriter(pipe);
        }

        public NamedPipeServerStream Pipe { get; }

        public StreamReader Reader { get; }

        public StreamWriter Writer { get; }

        public void Dispose()
        {
            try
            {
                Pipe.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }

}
