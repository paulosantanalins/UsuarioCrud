using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Infra.Data.SqlServer.Mappings.EmailMap;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utils;

namespace EnvioEmail.Infra.Data.SqlServer.Context
{
    public class ServiceBContext : DbContext
    {
        private readonly IVariablesToken _variables;

        public ServiceBContext(IVariablesToken variables)
        {
            _variables = variables;
        }

        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<TemplateEmail> TemplateEmail { get; set; }
        public virtual DbSet<ParametroTemplate> ParametroTemplate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmailMap());
            modelBuilder.ApplyConfiguration(new TemplateEmailMap());
            modelBuilder.ApplyConfiguration(new ParametroTemplateMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer(Variables.DefaultConnection);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}