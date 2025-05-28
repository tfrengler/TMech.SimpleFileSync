using Serilog;
using System;

namespace TMech.SFS
{
    class MyClassCS
    {
        static void Main()
        {
            var watcher = new FileWatcher(@"C:\Dev\Temp");
            watcher.Start();

            Log.Logger.Information("Press any key to exit");
            Console.ReadKey();
        }
    }
}