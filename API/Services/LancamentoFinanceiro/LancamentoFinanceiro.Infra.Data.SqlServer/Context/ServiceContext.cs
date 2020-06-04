using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LancamentoFinanceiro.Infra.Data.SqlServer.Mappings;
using System.IO;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Utils;
using Logger.Model;
using Logger.Repository.Interfaces;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Context
{
    public class ServiceContext : DbContext
    {
        private readonly IVariablesToken _variables;
        private readonly IAuditoriaRepository _auditoriaRepository;
        public ServiceContext(IVariablesToken variables,
                              IAuditoriaRepository auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public DbSet<RootLancamentoFinanceiro> LancamentoFinanceiro { get; set; }
        public DbSet<ItemLancamentoFinanceiro> ItemLancamentoFinanceiro { get; set; }
        public DbSet<TipoDespesa> TipoDespesa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LancamentoFinanceiroMap());
            modelBuilder.ApplyConfiguration(new ItemLancamentoFinanceiroMap());
            modelBuilder.ApplyConfiguration(new TipoDespesaMap());
            modelBuilder.ApplyConfiguration(new ClienteContratoMap());
            modelBuilder.ApplyConfiguration(new ContratoMap());
            modelBuilder.ApplyConfiguration(new DeParaServicoMap());
            modelBuilder.ApplyConfiguration(new EscopoServicoMap());
            modelBuilder.ApplyConfiguration(new ServicoContratadoMap());
            modelBuilder.ApplyConfiguration(new VinculoMarkupServicoContratadoMap());
            modelBuilder.ApplyConfiguration(new CidadeMap());
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new EnderecoMap());
            modelBuilder.ApplyConfiguration(new GrupoClienteMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer(Variables.DefaultConnection);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var auditEntries = OnBeforeSaveChanges();

            //foreach (var item in auditEntries)
            //{
            //    item.Usuario = "Eacesso";
            //}

            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = ChangeTracker.Entries();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
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
                                if ((property.CurrentValue == null && property.OriginalValue != null) || !property.CurrentValue.Equals(property.OriginalValue))
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
                _auditoriaRepository.AddLog(auditEntry.ToAudit()).Wait();
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

                _auditoriaRepository.AddLog(auditEntry.ToAudit()).Wait();
            }

            SaveChanges();
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

            public Logger.Model.Auditoria ToAudit()
            {
                var auditoria = new Logger.Model.Auditoria
                {
                    Tabela = Tabela,
                    Usuario = Usuario.Substring(0, 2),
                    DataAlteracao = DateTime.Now.AddHours(-3),
                    IdsAlterados = JsonConvert.SerializeObject(IdsAlterados),
                    ValoresAntigos = ValoresAntigos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresAntigos),
                    ValoresNovos = ValoresNovos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresNovos)
                };
                return auditoria;
            }
        }
    }
}