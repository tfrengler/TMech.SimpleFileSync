using System;
using System.Collections.Generic;
using System.Linq;

namespace TMech.SimpleFileSync
{
    public enum ChangeType
    {
        CREATE, DELETED, CONTENT_CHANGE, ATTRIBUTE_CHANGE
    }

    public class FileChange
    {
        public long Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public DateTime Created { get; set; }

        public long FileId { get; set; }
        public FileRecord File { get; set; } = null!; // To shut the compiler up

        public List<ChangeEvent> Events { get; } = [];
        public bool Complete => Events.All(x => x.Complete);
    }

    public class ChangeEvent
    {
        public long Id { get; set; }
        public ChangeType Type { get; set; }
        public bool Complete { get; set; }
        public int Order { get; set; }

        public long FileChangeId { get; set; }
        public FileChange FileChange { get; set; } = null!; // To shut the compiler up

        public static ChangeEvent Create(int order) => new ChangeEvent() { Type = ChangeType.CREATE, Order = order };
        public static ChangeEvent Delete(int order) => new ChangeEvent() { Type = ChangeType.DELETED, Order = order };
        public static ChangeEvent ContentChange(int order) => new ChangeEvent() { Type = ChangeType.CONTENT_CHANGE, Order = order };
        public static ChangeEvent AttributeChange(int order) => new ChangeEvent() { Type = ChangeType.ATTRIBUTE_CHANGE, Order = order };
    }

    public static class FileChangeBuilder
    {
        public static FileChange CreateNewFile(FileRecord file)
        {
            var ReturnData = new FileChange()
            {
                Created = DateTime.UtcNow,
                FileId = file.Id
            };

            ReturnData.Events.Add(ChangeEvent.Create(1));
            ReturnData.Events.Add(ChangeEvent.AttributeChange(2));
            ReturnData.Events.Add(ChangeEvent.ContentChange(3));

            return ReturnData;
        }

        public static FileChange ChangeContent(FileRecord file)
        {
            var ReturnData = new FileChange()
            {
                Created = DateTime.UtcNow,
                FileId = file.Id
            };

            ReturnData.Events.Add(ChangeEvent.AttributeChange(1));
            ReturnData.Events.Add(ChangeEvent.ContentChange(2));

            return ReturnData;
        }

        public static FileChange ChangeAttributes(FileRecord file)
        {
            var ReturnData = new FileChange()
            {
                Created = DateTime.UtcNow,
                FileId = file.Id
            };

            ReturnData.Events.Add(ChangeEvent.AttributeChange(1));

            return ReturnData;
        }

        public static FileChange DeleteFile(FileRecord file)
        {
            var ReturnData = new FileChange()
            {
                Created = DateTime.UtcNow,
                FileId = file.Id
            };

            ReturnData.Events.Add(ChangeEvent.Delete(1));

            return ReturnData;
        }
    }
}
