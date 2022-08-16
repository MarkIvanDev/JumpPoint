using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using JumpPoint.FullTrust.Core.ChangeNotifier;
using Newtonsoft.Json;
using NittyGritty.Utilities;
using Windows.Storage;

namespace JumpPoint.FullTrust.ChangeNotifier
{
    class Program
    {
        private static ClientPipe? client = null;
        private static readonly object watchersLock = new object();
        private static readonly Dictionary<string, WatcherItem> watchers = new Dictionary<string, WatcherItem>(StringComparer.OrdinalIgnoreCase);

        static void Main(string[] args)
        {
            Task.Run(Connect);

            Process.GetCurrentProcess().WaitForExit();
        }

        private static void Connect()
        {
            // Get the sid stored in local settings with key "PackageSid"
            var pipe = new NamedPipeClientStream(".",
                $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\{ApplicationData.Current.LocalSettings.Values["PackageSid"]}\\JumpPoint_ChangeNotifierServerPipe",
                PipeDirection.InOut, PipeOptions.Asynchronous);
            pipe.Connect(5000);

            client = new ClientPipe(pipe);
            Task.Run(() => Client(client));
        }

        private static void Client(ClientPipe pipe)
        {
            try
            {
                while (pipe.Pipe.IsConnected)
                {
                    var message = pipe.Reader.ReadLine();
                    if (string.IsNullOrEmpty(message)) continue;

                    var data = CodeHelper.InvokeOrDefault(() => JsonConvert.DeserializeObject<MonitorChangeRequest>(message));
                    if (data != null)
                    {
                        lock (watchersLock)
                        {
                            if (!data.Release)
                            {
                                if (watchers.TryGetValue(data.Path, out var item))
                                {
                                    item.ListenerCount += 1;
                                }
                                else
                                {
                                    watchers.Add(data.Path, new WatcherItem(StartWatcher(data.Path)));
                                }
                            }
                            else
                            {
                                if (watchers.TryGetValue(data.Path, out var item))
                                {
                                    if (item.ListenerCount == 1)
                                    {
                                        StopWatcher(item.Watcher);
                                        watchers.Remove(data.Path);
                                    }
                                    else
                                    {
                                        item.ListenerCount -= 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Cleanup();
                Process.GetCurrentProcess().Kill();
            }
        }

        private static void Cleanup()
        {
            foreach (var item in watchers)
            {
                StopWatcher(item.Value.Watcher);
            }
        }

        private static FileSystemWatcher StartWatcher(string path)
        {
            var watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Attributes |
                NotifyFilters.Size |
                NotifyFilters.LastWrite |
                NotifyFilters.LastAccess |
                NotifyFilters.CreationTime;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static void StopWatcher(FileSystemWatcher watcher)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Changed -= OnChanged;
            watcher.Created -= OnCreated;
            watcher.Deleted -= OnDeleted;
            watcher.Renamed -= OnRenamed;
            watcher.Error -= OnError;
            watcher.Dispose();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;
            SendChange(new NotifyChange
            {
                ChangeType = ChangeType.Changed,
                FullPath = e.FullPath,
                Name = e.Name
            });
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            var attributes = CodeHelper.InvokeOrDefault(() => File.GetAttributes(e.FullPath), (System.IO.FileAttributes?)null);
            if (attributes.HasValue)
            {
                SendChange(new NotifyChange
                {
                    ChangeType = ChangeType.Created,
                    FullPath = e.FullPath,
                    Name = e.Name,
                    IsDirectory = (attributes.Value & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory
                });
            }
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            SendChange(new NotifyChange
            {
                ChangeType = ChangeType.Deleted,
                FullPath = e.FullPath,
                Name = e.Name,
            });
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            SendChange(new NotifyChange
            {
                ChangeType = ChangeType.Renamed,
                FullPath = e.FullPath,
                Name = e.Name,
                OldFullPath = e.OldFullPath,
                OldName = e.OldName
            });
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            SendChange(new NotifyChange
            {
                ChangeType = ChangeType.Unknown
            });
        }

        private static void SendChange(NotifyChange message)
        {
            if (client != null && client.Pipe.IsConnected)
            {
                message.Path = Path.GetDirectoryName(message.FullPath);
                var change = JsonConvert.SerializeObject(message);
                client.Writer.WriteLine(change);
                client.Writer.Flush();
            }
        }
    }

    public class WatcherItem
    {
        public WatcherItem(FileSystemWatcher watcher)
        {
            Watcher = watcher;
            ListenerCount = 1;
        }

        public FileSystemWatcher Watcher { get; }

        public int ListenerCount { get; set; }
    }

    public class ClientPipe : IDisposable
    {
        public ClientPipe(NamedPipeClientStream pipe)
        {
            Pipe = pipe;
            Reader = new StreamReader(pipe);
            Writer = new StreamWriter(pipe);
        }

        public NamedPipeClientStream Pipe { get; }

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