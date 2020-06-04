using Cliente.Infra.Data.SqlServer.Mappings;
using GestaoServico.Domain.AuditoriaRoot.Entity;
using GestaoServico.Domain.GestaoFilialRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.OperacaoMigradaRoot.Entity;
using GestaoServico.Domain.PessoaRoot.Entity;
using GestaoServico.Infra.Data.SqlServer.Mappings.AuditoriaMap;
using GestaoServico.Infra.Data.SqlServer.Mappings.GestaoFilialMap;
using GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPacoteServicoMap;
using GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap;
using GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map;
using GestaoServico.Infra.Data.SqlServer.Mappings.PessoaMap;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Context
{
    public class GestaoServicoContext : DbContext
    {
        private readonly IVariablesToken _variables;

        public GestaoServicoContext(IVariablesToken variables)
        {
            _variables = variables;
        }

        //public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Celula> Celulas { get; set; }
        public virtual DbSet<Contrato> Contratos { get; set; }
        //public virtual DbSet<ClienteET> ClientesET { get; set; }
        public virtual DbSet<ServicoContratado> ServicoContratado { get; set; }
        //public virtual DbSet<CategoriaContabil> Categorias { get; set; }
        public virtual DbSet<PortfolioServico> PortifolioServicos { get; set; }
        //public virtual DbSet<Delivery> Deliveries { get; set; }        
        //public virtual DbSet<ClassificacaoContabil> ClassificacaoContabil { get; set; }
        public virtual DbSet<Auditoria> Auditoria { get; set; }
        public virtual DbSet<Repasse> Repasses { get; set; }
        public virtual DbSet<EscopoServico> EscopoServicos { get; set; }
        public virtual DbSet<DeParaServico> DeParaServicos { get; set; }
        public virtual DbSet<VinculoMarkupServicoContratado> VinculoMarkupServicoContratados { get; set; }
        public virtual DbSet<VinculoServicoCelulaComercial> VinculoServicoCelulaComercial { get; set; }
        public virtual DbSet<OperacaoMigrada> OperacaoMigrada { get; set; }
        public virtual DbSet<ClienteContrato> ClienteContrato { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new EmpresaMap());
            modelBuilder.ApplyConfiguration(new CelulaMap());
            modelBuilder.ApplyConfiguration(new ContratoMap());
            modelBuilder.ApplyConfiguration(new ServicoContratadoMap());
            //modelBuilder.ApplyConfiguration(new CategoriaMap());
            modelBuilder.ApplyConfiguration(new PortifolioServicoMap());
            //modelBuilder.ApplyConfiguration(new DeliveryMap());            
            //modelBuilder.ApplyConfiguration(new ClassificacaoContabilMap());
            //modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new AuditoriaMap());
            modelBuilder.ApplyConfiguration(new RepasseMap());
            modelBuilder.ApplyConfiguration(new EscopoServicoMap());
            modelBuilder.ApplyConfiguration(new DeParaServicoMap());
            modelBuilder.ApplyConfiguration(new VinculoMarkupServicoContratadoMap());
            modelBuilder.ApplyConfiguration(new VinculoServicoCelulaComercialMap());
            modelBuilder.ApplyConfiguration(new OperacaoMigradaMap());
            modelBuilder.ApplyConfiguration(new ClienteContratoMap());
            modelBuilder.ApplyConfiguration(new PessoaMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer(Variables.DefaultConnection);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Auditoria || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    Tabela = entry.Metadata.Relational().TableName.ToUpper(),
                    Usuario = _variables.UserName
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.IdsAlterados[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.ValoresNovos[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.ValoresAntigos[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                if ((property.CurrentValue != null && property.OriginalValue != null) && (!property.CurrentValue.Equals(property.OriginalValue)))
                                {
                                    auditEntry.ValoresAntigos[propertyName] = property.OriginalValue;
                                    auditEntry.ValoresNovos[propertyName] = property.CurrentValue;
                                }
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Auditoria.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private void OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;
    
            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.IdsAlterados[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.ValoresNovos[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                Auditoria.Add(auditEntry.ToAudit());
            }

            SaveChanges();
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

    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string Tabela { get; set; }
        public string Usuario { get; set; }
        public Dictionary<string, object> IdsAlterados { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> ValoresAntigos { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> ValoresNovos { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public Auditoria ToAudit()
        {
            var auditoria = new Auditoria
            {
                Tabela = Tabela,
                Usuario = Usuario,
                DataAlteracao = DateTime.Now.AddHours(-3),
                IdsAlterados = JsonConvert.SerializeObject(IdsAlterados),
                ValoresAntigos = ValoresAntigos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresAntigos),
                ValoresNovos = ValoresNovos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresNovos)
            };
            return auditoria;
        }
    }
}
