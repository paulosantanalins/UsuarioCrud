using Microsoft.EntityFrameworkCore;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Infra.Data.SqlServer.Mappings;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Context
{
    public class RepasseLegadoContext : DbContext
    {
        public DbSet<Repasse> Repasse { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<ClienteServico> ClienteServico { get; set; }
        public DbSet<Profissionais> Profissionais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("STFCORP");
            modelBuilder.ApplyConfiguration(new RepasseMap());
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new ClienteServicoMap());
            modelBuilder.ApplyConfiguration(new ProfissionaisMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Variables.EacessoConnection);
        }
    }
}
