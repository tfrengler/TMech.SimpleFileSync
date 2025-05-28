using System.IO;
using System.IO.Hashing;

namespace TMech.SFS
{
    public sealed record FileRecord
    {
        public long Id { get; set; }
        public long UniqueId { get; set; }
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

        /// <summary>Creates a new instance from a FileInfo-instance but does not calculate and set the checksum.</summary>
        public static FileRecord FromFileInfo(FileInfo fileInfo)
        {
            var ReturnData = new FileRecord
            {
                Path = fileInfo.Directory!.FullName,
                Name = fileInfo.Name,
                DateTimeCreated = fileInfo.CreationTimeUtc,
                DateTimeModified = fileInfo.LastWriteTimeUtc,
                Size = fileInfo.Length
            };

            return ReturnData;
        }
    }
}
