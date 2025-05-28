using System;
using System.IO;

namespace TMech.SFS
{
    public static class FileSystemTools
    {
        public static bool IsDirectory(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            return new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Directory);
        }
    }
}
