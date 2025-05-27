using System;

namespace FileWatcher
{
    public sealed class CreateCommand : CommandBase, ICommand
    {
        private readonly UploadManager _uploadManager;
        private readonly FileRecord _fileRecord;

        public CreateCommand(UploadManager uploadManager, FileRecord fileRecord): base(CommandDirection.UP)
        {
            ArgumentNullException.ThrowIfNull(uploadManager);
            ArgumentNullException.ThrowIfNull(fileRecord);

            _uploadManager = uploadManager;
            _fileRecord = fileRecord with { };
        }

        public void Execute()
        {
            _uploadManager.Create(_fileRecord.Name, _fileRecord.Path, _fileRecord.Size);
        }
    }

    public sealed class MoveCommand : CommandBase, ICommand
    {
        private readonly UploadManager _uploadManager;
        private readonly FileRecord _fileRecord;

        public MoveCommand(UploadManager uploadManager, FileRecord fileRecord): base(CommandDirection.UP)
        {
            ArgumentNullException.ThrowIfNull(uploadManager);
            ArgumentNullException.ThrowIfNull(fileRecord);

            _uploadManager = uploadManager;
            _fileRecord = fileRecord with { };
        }

        public void Execute()
        {
            _uploadManager.Move(_fileRecord.UniqueId, _fileRecord.Path);
        }
    }

    public sealed class RenameCommand : CommandBase, ICommand
    {
        private readonly UploadManager _uploadManager;
        private readonly FileRecord _fileRecord;

        public RenameCommand(UploadManager uploadManager, FileRecord fileRecord) : base(CommandDirection.UP)
        {
            ArgumentNullException.ThrowIfNull(uploadManager);
            ArgumentNullException.ThrowIfNull(fileRecord);

            _uploadManager = uploadManager;
            _fileRecord = fileRecord with { };
        }

        public void Execute()
        {
            _uploadManager.Rename(_fileRecord.UniqueId, _fileRecord.Name);
        }
    }

    public sealed class ModifyCommand : CommandBase, ICommand
    {
        private readonly UploadManager _uploadManager;
        private readonly FileRecord _fileRecord;
        private readonly long _chunks;
        private readonly string _checksum;

        public ModifyCommand(UploadManager uploadManager, FileRecord fileRecord, long chunks, string checksum) : base(CommandDirection.UP)
        {
            ArgumentNullException.ThrowIfNull(uploadManager);
            ArgumentNullException.ThrowIfNull(fileRecord);

            _uploadManager = uploadManager;
            _chunks = chunks;
            _checksum = checksum;
            _fileRecord = fileRecord with { };
        }

        public void Execute()
        {
            _uploadManager.Modify(_fileRecord.UniqueId, _fileRecord.Size, _chunks, _checksum);
        }
    }

    public sealed class ContentCommand : CommandBase, ICommand
    {
        private readonly UploadManager _uploadManager;
        private readonly FileRecord _fileRecord;
        private readonly byte[] _chunk;
        private readonly string _checksum;

        public ContentCommand(UploadManager uploadManager, FileRecord fileRecord, byte[] chunk, string checksum) : base(CommandDirection.UP)
        {
            ArgumentNullException.ThrowIfNull(uploadManager);
            ArgumentNullException.ThrowIfNull(fileRecord);
            ArgumentOutOfRangeException.ThrowIfZero(chunk.Length);
            ArgumentException.ThrowIfNullOrWhiteSpace(checksum);

            _uploadManager = uploadManager;
            _chunk = chunk;
            _checksum = checksum;
            _fileRecord = fileRecord with { };
        }

        public void Execute()
        {
            _uploadManager.Content(_fileRecord.UniqueId, _chunk, _checksum);
        }
    }
}
