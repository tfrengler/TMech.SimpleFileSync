using System.IO;
using System.IO.Hashing;

namespace FileWatcher
{
    public sealed record FileRecord
    {
        public long Id { get; set; }
        public long UniqueId { get; set; }
        /// <summary>Relative path from the root watch folder</summary>
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentChecksum { get; set; } = string.Empty;
        public System.DateTime DateTimeCreated { get; set; } = System.DateTime.MinValue;
        public System.DateTime DateTimeModified { get; set; } = System.DateTime.MinValue;
        public string FullName => System.IO.Path.Combine(Path, Name);

        public FileRecord() { }

        public FileRecord(long uniqueId)
        {
            UniqueId = uniqueId;
        }

        public override string ToString()
        {
            return string.Join(System.Environment.NewLine, [
                $"Id                : {Id}",
                $"FileId            : {UniqueId}",
                $"Path              : {Path}",
                $"Name              : {Name}",
                $"Size              : {Size}",
                $"ContentChecksum   : {ContentChecksum}",
                $"DateTimeCreated   : {DateTimeCreated.ToString("o")}",
                $"DateTimeModified  : {DateTimeModified.ToString("o")}",
            ]);
        }

        public string CalculateChecksum()
        {
            var XxHash = new XxHash128();
            string ReturnData;

            using (FileStream TheFile = File.OpenRead(System.IO.Path.Combine(Path, Name)))
            {
                var Buffer = new byte[1024 * 1024 * 4];
                while (TheFile.Read(Buffer) > 0)
                {
                    XxHash.Append(Buffer);
                }

                ReturnData = System.Convert.ToHexString(XxHash.GetCurrentHash());
            }

            return ReturnData;
        }

        public string CalculateAndSetChecksum()
        {
            ContentChecksum = CalculateChecksum();
            return ContentChecksum;
        }

        public static FileRecord FromFileInfo(FileInfo fileInfo)
        {
            var ReturnData = new FileRecord();
            ReturnData.Path = fileInfo.Directory!.FullName;
            ReturnData.Name = fileInfo.Name;
            ReturnData.DateTimeCreated = fileInfo.CreationTimeUtc;
            ReturnData.DateTimeModified = fileInfo.LastWriteTimeUtc;
            ReturnData.Size = fileInfo.Length;

            return ReturnData;
        }
    }
}
