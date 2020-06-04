using Logger.Mapping;
using Logger.Model;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Logger.Context
{
    public class LogGenericoContext : DbContext
    {
        public DbSet<LogGenerico> LogGenericos { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LogGenericoMap());
            modelBuilder.ApplyConfiguration(new AuditoriaMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer(Variables.DefaultConnection);
        }
    }
}
