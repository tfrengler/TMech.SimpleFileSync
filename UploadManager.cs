using System;

namespace TMech.SFS
{
    public sealed class UploadManager
    {
        public void Create(string name, string path, long size)
        {
            // Relevant data:
            // Name, Path, Size
            // Returned by server:
            // Status 201
            // UniqueId of the file
            Console.WriteLine($"Creating file on the server: {path + name} ({size} bytes)");
        }

        public void Move(long fileId, string newPath)
        {
            // Relevant data:
            // UniqueId, newPath
            // Returned by server:
            // Status 204
            Console.WriteLine($"Moving file {fileId} on the server to {newPath}");
        }

        public void Rename(long fileId, string newName)
        {
            // Relevant data:
            // UniqueId, newName
            // Returned by server:
            // Status 204
            Console.WriteLine($"Renaming file {fileId} on the server to {newName}");
        }

        public void Delete(long fileId)
        {
            // Relevant data:
            // UniqueId
            // Returned by server:
            // Status 204
            Console.WriteLine($"Creating file {fileId} on the server");
        }

        public void Modify(long fileId, long newSize, long chunks, string checksum)
        {
            // Relevant data:
            // UniqueId, Size
            // Returned by server:
            // Status 204
            Console.WriteLine($"Adjusting file {fileId} on the server to be {newSize} bytes, uploaded in {chunks}");
        }

        public void Content(long fileId, byte[] chunk, string checksum)
        {
            // Relevant data:
            // UniqueId, chunk
            // Returned by server:
            // Status 204 until all content uploaded and then 201
            Console.WriteLine($"Uploading {chunk.Length} bytes of content to file {fileId} on the server");
        }
    }
}
