using Microsoft.EntityFrameworkCore;
using RepasseEAcesso.Domain.CelulaRoot.Entity;
using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Infra.Data.SqlServer.Mappings;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Context
{
    public class RepasseEAcessoContext : DbContext
    {
        public DbSet<PeriodoRepasse> PeriodoRepasses { get; set; }
        public DbSet<RepasseNivelUm> RepasseNivelUm { get; set; }
        public DbSet<Celula> Celula { get; set; }
        public DbSet<Dominio> Dominio { get; set; }
        public DbSet<LogRepasse> LogRepasse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PeriodoRepasseMap());
            modelBuilder.ApplyConfiguration(new RepasseNivelUmMap());
            modelBuilder.ApplyConfiguration(new CelulaMap());
            modelBuilder.ApplyConfiguration(new DominioMap());
            modelBuilder.ApplyConfiguration(new RepasseMap());
            modelBuilder.ApplyConfiguration(new LogRepasseMap());

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Variables.DefaultConnection);
        }
    }
}