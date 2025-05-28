using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace TMech.SFS
{
    internal static partial class NativeOSFileTools
    {
        public enum Win32Error
        {
            UNKNOWN         = 0,
            FILE_NOT_FOUND  = 0x00000002,
            PATH_NOT_FOUND  = 0x00000003,
            FILE_LOCKED     = 0x000000DC
        }

        /// <summary>
        /// Returns the native OS specific id of a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ulong GetFileId(string path)
        {
            if (!new System.IO.FileInfo(path).Exists)
            {
                return 0;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetFileIdLinux(path) ?? 0;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException();
            }

            if (!GetFileHandle(path, out SafeFileHandle? handle))
            {
                return 0;
            }

            if (!GetFileInformationByHandleInternal(handle, out FileIdInfo? fileInfo))
            {
                return 0;
            }

            ulong returnData = ((ulong)fileInfo.Value.FileIndexHigh << 32) | fileInfo.Value.FileIndexLow;
            Debug.Assert(returnData > 0);

            return returnData;
        }

        #region WINDOWS

        private static bool GetFileHandle(string path, [NotNullWhen(true)] out SafeFileHandle? fileHandle)
        {
            Marshal.SetLastPInvokeError(0);

            SafeFileHandle handle = CreateFileW(
                @"\\?\" + path,     // lpFileName
                0x80000000,         // dwDesiredAccess
                0x00000001,         // dwShareMode
                IntPtr.Zero,        // lpSecurityAttributes
                3,                  // dwCreationDisposition
                0,                  // dwFlagsAndAttributes
                IntPtr.Zero         // hTemplateFile
            );

            if (GetWin32Error(out Win32Error error))
            { 
                Trace.TraceError("Windows > CreateFileW error: " + error);
                fileHandle = null;
                return false;
            }

            Debug.Assert(!handle.IsInvalid);
            fileHandle = handle;

            return true;
        }

        private static bool GetFileInformationByHandleInternal(SafeFileHandle file, [NotNullWhen(true)] out FileIdInfo? fileInfo)
        {
            Debug.Assert(!file.IsInvalid);
            Marshal.SetLastPInvokeError(0);

            if (GetFileInformationByHandle(file.DangerousGetHandle(), out var returnData))
            {
                file.Dispose();
                fileInfo = returnData;
                return true;
            }

            if (GetWin32Error(out Win32Error error))
            {
                Trace.TraceError("Windows > GetFileInformationByHandle: " + error);
                fileInfo = null;
                return false;
            }

            file.Dispose();
            fileInfo = null;
            return false;
        }

        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial SafeFileHandle CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetFileInformationByHandle(IntPtr hFile, out FileIdInfo fileInfo);

        // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information
        [StructLayout(LayoutKind.Sequential)]
        public struct FileIdInfo
        {
            public uint FileAttributes { get; set; }
            public FILETIME CreationTime { get; set; }
            public FILETIME LastAccessTime { get; set; }
            public FILETIME LastWriteTime { get; set; }
            public uint VolumeSerialNumber { get; set; }
            public uint FileSizeHigh { get; set; }
            public uint FileSizeLow { get; set; }
            public uint NumberOfLinks { get; set; }
            public uint FileIndexHigh { get; set; }
            public uint FileIndexLow { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint LowDateTime { get; set; }
            public uint HighDateTime { get; set; }
        }

        private static bool GetWin32Error(out Win32Error error)
        {
            int LatestError = Marshal.GetLastWin32Error();
            if (LatestError == 0)
            {
                error = Win32Error.UNKNOWN;
                return false;
            }

            if (Enum.IsDefined(typeof(Win32Error), LatestError))
            {
                error = (Win32Error)LatestError;
                return true;
            }

            error = Win32Error.UNKNOWN;
            return true;
        }

        #endregion

        #region UNIX

        // ---------- Linux/macOS: Inode Number ----------
        [StructLayout(LayoutKind.Sequential)]
        private struct Stat
        {
            public ulong st_dev;
            public ulong st_ino;   // Inode number
            public ulong st_nlink;
            public uint st_mode;
            public uint st_uid;
            public uint st_gid;
            public ulong st_rdev;
            public long st_size;
            public long st_blksize;
            public long st_blocks;
            public long st_atime;
            public long st_mtime;
            public long st_ctime;
        }

        [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial int stat(string path, out Stat buf);

        private static ulong? GetFileIdLinux(string path)
        {
            if (stat(path, out Stat fileStat) == 0)
            {
                return fileStat.st_ino;
            }

            return null;
        }

        #endregion
    }
}
