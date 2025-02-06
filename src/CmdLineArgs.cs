using System.IO;

namespace TMech.SimpleFileSync
{
    public sealed record CmdLineArgs
    {
        public DirectoryInfo WatchFolder { get; set; }

        public CmdLineArgs()
        {
            WatchFolder = new("DOES_NOT_EXIST");
        }
    }
}
