using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TMech.SimpleFileSync
{
    internal static class Program
    {
        public static DirectoryInfo RunningDirectory { get; private set; } = null!;
        public static FileInfo DatabaseFile { get; private set; } = null!;
        public static DirectoryInfo WatchFolder { get; private set; } = null!;

        public static void Main(string[] args)
        {
            string? AssemblyPath = Assembly.GetExecutingAssembly().Location;
            Debug.Assert(!string.IsNullOrWhiteSpace(AssemblyPath));
            string RunningDirectoryPath = Path.GetDirectoryName(AssemblyPath) ?? string.Empty;
            Debug.Assert(!string.IsNullOrWhiteSpace(RunningDirectoryPath));
            RunningDirectory = new DirectoryInfo(RunningDirectoryPath);
            Debug.Assert(RunningDirectory.Exists);

            var CommandLineArgs = CmdLineParser.Parse(args);
            Debug.Assert(CommandLineArgs.WatchFolder.Exists);
            WatchFolder = CommandLineArgs.WatchFolder;

            SetupLogging();
            SetupDatabase();

            Log.Logger.Information("Watch folder: {0}", WatchFolder.FullName);
            Debugger.Test();
        }

        private static void SetupDatabase()
        {
            DatabaseFile = new FileInfo(Path.Combine(RunningDirectory.FullName, "Manifest.sdb"));

            Log.Logger.Information("Initializing database ({0})", DatabaseFile.FullName);

            if (!DatabaseFile.Exists)
            {
                Log.Logger.Information("Db file does not exist, creating");
                var fileHandle = DatabaseFile.Create();
                fileHandle.Dispose();
                DatabaseFile.Refresh();
            }

            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

#if DEBUG
            Log.Logger.Information("Generating DDL script");
            using (var db = new DatabaseContext())
            {
                var Timer = Stopwatch.StartNew();
                var Timeout = TimeSpan.FromSeconds(5.0d);
                bool Success = false;

                while (Timer.Elapsed < Timeout)
                {
                    try
                    {
                        db.Database.EnsureCreated();
                        Success = true;
                        break;
                    }
                    catch (SqliteException error)
                    {
                        if (!error.Message.Contains("unable to open database file"))
                        {
                            throw;
                        }
                    }
                }

                if (!Success)
                {
                    throw new TimeoutException("Timed out connecting to the database");
                }

                File.WriteAllText("DDL.sql", db.Database.GenerateCreateScript());
            }
#endif

            Log.Logger.Information("Database initialized");
        }

        private static void SetupLogging()
        {
            var LogFile = new FileInfo(Path.Combine(RunningDirectory.FullName, "Log.txt"));
            var LoggerConf = new LoggerConfiguration();

#if DEBUG
            LogFile.Delete();
            LoggerConf.MinimumLevel.Debug();
#endif

            Log.Logger = LoggerConf
                .WriteTo.Console()
                //.WriteTo.File(LogFile.FullName, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Logger.Information("Logging initialized ({0})", LogFile.FullName);
        }
    }
}