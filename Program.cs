using FileWatcher;
using System;
using System.IO;

namespace MyNamespace
{
    class MyClassCS
    {
        static void Main()
        {
            //ulong id = NativeOSFileTools.GetFileId(@"E:\setup-1.bin");
            //Console.WriteLine(id);

            // "E:\setup-1.bin"
            /*
                FileAttributes: 32
                VolumeSerialNumber: 3200592009
                FileSizeHigh: 9
                FileSizeLow: 2763339251
                NumberOfLinks: 1
                FileIndexHigh: 79888384
                FileIndexLow: 65

                File size: 41418044915
            */

            using var watcher = new FileSystemWatcher(@"C:\Temp\Files");

            watcher.NotifyFilter = 
                                 //NotifyFilters.Attributes
                                 //| NotifyFilters.CreationTime
                                 NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 //| NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 //| NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            //watcher.Filter = "*.txt";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            if (FileSystemTools.IsDirectory(e.FullPath))
            {
                return;
            }

            ulong? fileId = NativeOSFileTools.GetFileId(e.FullPath);
            Console.WriteLine($"Changed: {e.FullPath} ({fileId})");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            ulong? fileId = NativeOSFileTools.GetFileId(e.FullPath);
            string value = $"Created: {e.FullPath} ({fileId})";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            ulong? fileId = NativeOSFileTools.GetFileId(e.FullPath);
            Console.WriteLine($"Deleted ({fileId}): {e.FullPath}");
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            ulong? fileId = NativeOSFileTools.GetFileId(e.FullPath);
            Console.WriteLine($"Renamed ({fileId}):");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}