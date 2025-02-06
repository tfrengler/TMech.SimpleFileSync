using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TMech.SimpleFileSync
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<FileRecord> Files { get; set; }
        public DbSet<FileChange> FileChanges { get; set; }

        private static readonly string ConnectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = Program.DatabaseFile.FullName,
            ForeignKeys = true,
            Mode = SqliteOpenMode.ReadWrite
        }.ToString();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileChange>()
                .HasMany(e => e.Events)
                .WithOne(e => e.FileChange)
                .HasForeignKey(e => e.FileChangeId)
                .IsRequired();

            modelBuilder.Entity<FileChange>()
                .HasOne(e => e.File)
                .WithOne();
        }

        /*public List<SyncTransaction> GetTransactions()
        {
            var ReturnData = new List<SyncTransaction>();
            var transactions = Transactions.ToList();

            foreach(SyncTransactionDb current in transactions)
            {
                FileRecord AssociatedFile = Files
                    .AsNoTracking()
                    .Where(x => x.Id == current.FileId)
                    .Single();

                List<SyncEventDb> AssociatedEvents = Events
                    .AsNoTracking()
                    .Where(x => x.SyncTransactionId == current.Id)
                    .OrderBy(x => x.Order)
                    .ToList();

                ReturnData.Add(SyncTransaction.FromDb(current, AssociatedFile, AssociatedEvents));
            }

            return ReturnData;
        }*/
    }
}
