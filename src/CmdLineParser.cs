using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace TMech.SimpleFileSync
{
    public static class CmdLineParser
    {
        public static CmdLineArgs Parse(string[] args)
        {
            Debug.Assert(args is not null);

            if (args.Length == 0)
            {
                throw new Exception("No args passed");
            }

            var CleanedArgs = new List<string>();
            foreach (string current in args)
            {
                CleanedArgs.Add(current.TrimStart('-'));
            }

            var ReturnData = new CmdLineArgs();

            (bool Exists, string WatchFolder) = FindArg("watchfolder", CleanedArgs);
            if (!Exists || WatchFolder == string.Empty)
            {
                throw new Exception("Expected cmd line arg 'watchfolder' to exist and have a value");
            }

            ReturnData.WatchFolder = new System.IO.DirectoryInfo(WatchFolder);
            return ReturnData;
        }

        private static Tuple<bool, string> FindArg(string name, List<string> args)
        {
            string? ArgValue = args.Find(x => x.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
            if (ArgValue is null) return new(false, string.Empty);

            string[] Parts = ArgValue.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (Parts.Length == 1)
            {
                return new(true, string.Empty);
            }

            return new(true, Parts[1]);
        }
    }
}
