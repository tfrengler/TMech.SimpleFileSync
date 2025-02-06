using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using System.Security.AccessControl;

namespace TMech.SimpleFileSync
{
    internal static class Debugger
    {
        public static void Test()
        {
            var Watcher = new Watcher(Program.WatchFolder);
            //using var db = new DatabaseContext();
            //var Snapshot = Watcher.GenerateSnapshot();
            //var Manifest = db.Files.ToList();

            /*
            foreach(var file in Files)
            {
                //Console.WriteLine(file);
                //Console.WriteLine("-----------------------");

                file.CalculateAndSetChecksum();
                file.FileId = Guid.NewGuid().ToString();
            }

            using (var db = new DatabaseContext())
            {
                db.Files.AddRange(Files);
                db.SaveChanges();
            }
            */

            /*var Changes = Watcher.CorrelateChanges();

            Console.WriteLine(Changes.Count);
            foreach(var test in Changes)
            {
                Console.WriteLine(test);
            }*/
            
            using (var db = new DatabaseContext())
            {
                //var File = db.Files.AsNoTracking().ToList().First();
                var file = FileRecord.FromFileInfo(new System.IO.FileInfo(@"C:\Dev\tools\Temp\TestFolder3\TestFile6.txt"));

                db.Files.Add(file);
                db.SaveChanges();

                var test = FileChangeBuilder.CreateNewFile(file);
                db.FileChanges.Add(test);
                db.SaveChanges();
            }

            Log.Logger.Warning("DEBUGGER DONE");
        }
    }
}
