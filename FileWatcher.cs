using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TMech.SFS
{
    public sealed class FileWatcher
    {
        public FileWatcher(string watchFolder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(watchFolder);
            _watchFolder = new System.IO.DirectoryInfo(watchFolder);
            
            if (!_watchFolder.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(watchFolder);
            }

            _lastTimeChecked = DateTime.MaxValue.ToUniversalTime();

            _logger = Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()
                .ForContext<FileWatcher>();

            Log.Logger.Information("{SourceContext}: Initialized");
        }

        private readonly ILogger _logger;
        private readonly System.IO.DirectoryInfo _watchFolder;
        private DateTime _lastTimeChecked;
        private Task? _process;
        private CancellationTokenSource _processCancellationToken;

        // Temp
        Dictionary<ulong, FileRecord> manifest = new();

        public void Start()
        {
            _processCancellationToken = new();
            _process = Task.Run(() =>
            {
                while (true)
                {
                    if (_processCancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    GenerateDeltaAndCreateEvents();
                    Thread.Sleep(5000);
                }
            });
        }

        public void Stop()
        {
            if (_process is null)
            {
                return;
            }

            _processCancellationToken.Cancel();
        }

        public void GenerateDeltaAndCreateEvents()
        {
            var timer = Stopwatch.StartNew();
            IEnumerable<System.IO.FileInfo> snapshot = _watchFolder.EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories);
            //Dictionary<ulong, FileRecord> manifest = new(); // Read from database
            var threshold = _lastTimeChecked;

            foreach (System.IO.FileInfo currentFile in snapshot)
            {
                //if (currentFile.LastWriteTimeUtc <= threshold)
                //{
                //    continue;
                //}

                ulong fileId = NativeOSFileTools.GetFileId(currentFile.FullName);
                if (!manifest.TryGetValue(fileId, out FileRecord? existingFile))
                {
                    // New file
                    // Add create event
                    // Add modify event
                    Log.Logger.Information("New file: {0}", currentFile.FullName);
                    var newFileRecord = FileRecord.FromFileInfo(currentFile);
                    manifest.Add(fileId, newFileRecord);
                    
                    continue;
                }

                if (existingFile.Name != currentFile.Name)
                {
                    Log.Logger.Information("Renamed: {0} => {1}", existingFile.Name, currentFile.Name);
                    existingFile.Name = currentFile.Name;
                }

                if (existingFile.Path != currentFile.DirectoryName)
                {
                    Log.Logger.Information("Moved: {0} - {1} => {2}", existingFile.Name, existingFile.Path, currentFile.DirectoryName);
                    existingFile.Path = currentFile.DirectoryName!;
                }

                if (existingFile.DateTimeModified == currentFile.LastWriteTimeUtc)
                {
                    continue;
                }

                if (existingFile.Size != currentFile.Length)
                {
                    Log.Logger.Information("Modified: {0} - {1} => {2}", existingFile.FullName, existingFile.Size, currentFile.Length);
                    existingFile.Size = currentFile.Length;
                    existingFile.ContentChecksum = string.Empty;

                    continue;
                }
            }

            _lastTimeChecked = DateTime.UtcNow;
            Log.Logger.Information("Done, time taken: {0}", timer.Elapsed);
        }
    }
}
