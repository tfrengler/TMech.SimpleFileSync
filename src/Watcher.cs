using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TMech.SimpleFileSync
{
    public sealed class Watcher
    {
        private readonly DirectoryInfo RootFolder;

        public Watcher(DirectoryInfo rootFolder)
        {
            RootFolder = rootFolder;
        }

        public List<FileRecord> GenerateSnapshot()
        {
            var ReturnData = new List<FileRecord>();

            foreach (var CurrentFile in RootFolder.EnumerateFiles())
            {
                ReturnData.Add(FileRecord.FromFileInfo(CurrentFile));
            }

            foreach (var CurrentDir in RootFolder.EnumerateDirectories())
            {
                foreach(var CurrentFile in CurrentDir.EnumerateFiles())
                {
                    ReturnData.Add(FileRecord.FromFileInfo(CurrentFile));
                }
            }

            return ReturnData;
        }

        public List<FileRecord> GetManifest()
        {
            using (var database = new DatabaseContext())
            {
                return database.Files.ToList();
            }
        }
        /*
        public List<SyncTransaction> CorrelateChanges()
        {
            var Snapshot = GenerateSnapshot();
            var Manifest = GetManifest();

            var NewFiles = CorrelateSnapshotWithManifest(Snapshot, Manifest);

            var ReturnData = NewFiles
                .Concat(CorrelateManifestWithSnapshot(Snapshot, Manifest))
                .ToList();

            using (var db = new DatabaseContext())
            {
                db.Files.AddRange(NewFiles.Select(x => x.File));
                db.SaveChanges();
            }

            return ReturnData;
        }

        /// <summary>
        /// Correlates a snapshot of files from the watch-folder with the current manifest and produces a list of files that are new.
        /// </summary>
        public static List<SyncTransaction> CorrelateSnapshotWithManifest(List<FileRecord> snapshot, List<FileRecord> manifest)
        {
            var NewRecords = new List<SyncTransaction>();

            
            foreach(var currentSnapshotRecord in snapshot)
            {
                bool Exists = manifest.Find(x => x.FullName.Equals(currentSnapshotRecord.FullName, System.StringComparison.InvariantCulture)) is not null;
                if (Exists) continue;

                currentSnapshotRecord.CalculateAndSetChecksum();
                NewRecords.Add(SyncTransaction.NewFile(currentSnapshotRecord));
            }

            return NewRecords;
        }

        public static List<SyncTransaction> CorrelateManifestWithSnapshot(List<FileRecord> snapshot, List<FileRecord> manifest)
        {
            var ChangedRecords = new List<SyncTransaction>();

            using (var db = new DatabaseContext())
            {
                foreach (var currentManifestRecord in manifest)
                {
                    int SnapshotRecordIndex = snapshot.FindIndex(x => x.FullName.Equals(currentManifestRecord.FullName, StringComparison.InvariantCulture));
                    if (SnapshotRecordIndex == -1)
                    {
                        Log.Logger.Information("File does not exist in snapshot, has been deleted ({0})", currentManifestRecord.FullName);
                        ChangedRecords.Add(SyncTransaction.RemovedFile(currentManifestRecord));
                        db.Files.Remove(currentManifestRecord);
                        continue;
                    }

                    FileRecord SnapshotRecord = snapshot[SnapshotRecordIndex];

                    //if (currentManifestRecord.DateTimeCreated != SnapshotRecord.DateTimeCreated)
                    //{
                    //    currentManifestRecord.DateTimeCreated = SnapshotRecord.DateTimeCreated;
                    //    // Case: creation time changed
                    //    ChangedRecords.Add(currentManifestRecord);
                    //}

                    if (SnapshotRecord.Size != currentManifestRecord.Size)
                    {
                        Log.Logger.Information("Sizes do not match ({0} vs {1})", SnapshotRecord.Size, currentManifestRecord.Size);
                        ChangedRecords.Add(OnContentModified(currentManifestRecord, SnapshotRecord));
                        db.Files.Update(currentManifestRecord);
                        continue;
                    }

                    if (SnapshotRecord.DateTimeModified != currentManifestRecord.DateTimeModified)
                    {
                        Log.Logger.Information("DateTimeModified do not match ({0} vs {1})", SnapshotRecord.DateTimeModified, currentManifestRecord.DateTimeModified);
                        ChangedRecords.Add(OnContentModified(currentManifestRecord, SnapshotRecord));
                        db.Files.Update(currentManifestRecord);
                        continue;
                    }

                    if (SnapshotRecord.CalculateChecksum() != currentManifestRecord.ContentChecksum)
                    {
                        Log.Logger.Information("Checksums do not match ({0} vs {1})", SnapshotRecord.ContentChecksum, currentManifestRecord.ContentChecksum);
                        ChangedRecords.Add(OnContentModified(currentManifestRecord, SnapshotRecord));
                        System.Diagnostics.Debug.Assert(currentManifestRecord.ContentChecksum == SnapshotRecord.ContentChecksum);
                        db.Files.Update(currentManifestRecord);
                    }
                }

                //db.SaveChanges();
            }

            return ChangedRecords;
        }

        public static SyncTransaction OnContentModified(FileRecord manifestRecord, FileRecord snapshotRecord)
        {
            Log.Logger.Information("Content modified (file: {0})", manifestRecord.FullName);

            manifestRecord.CalculateAndSetChecksum();
            manifestRecord.Size = snapshotRecord.Size;
            manifestRecord.DateTimeModified = snapshotRecord.DateTimeModified;
            
            return SyncTransaction.FileContentChange(manifestRecord);
        }
        */
    }
}
