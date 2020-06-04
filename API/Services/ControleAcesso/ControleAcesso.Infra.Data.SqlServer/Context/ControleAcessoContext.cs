using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using ControleAcesso.Infra.Data.SqlServer.Mappings;
using ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso;
using Logger.Model;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Context
{
    public class ControleAcessoContext : DbContext
    {
        private readonly IVariablesToken _variables;
        private readonly IAuditoriaRepository _auditoriaRepository;

        public ControleAcessoContext(IVariablesToken variables,
                                     IAuditoriaRepository auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }
        
        public DbSet<Celula> Celulas { get; set; }
        public DbSet<Broadcast> Broadcasts { get; set; }
        public DbSet<BroadcastItem> BroadcastItems { get; set; }
        public DbSet<Funcionalidade> Funcionalidades { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Perfil> Perfils { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfils { get; set; }
        public DbSet<VinculoPerfilFuncionalidade> VinculoPerfilFuncionalidades { get; set; }
        public DbSet<VinculoTipoCelulaTipoContabil> VinculoTipoCelulaTipoContabil { get; set; }
        public DbSet<VisualizacaoCelula> VisualizacaoCelula { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<TipoCelula> TipoCelula { get; set; }
        public virtual DbSet<Dominio> Dominio { get; set; }
        public virtual DbSet<MonitoramentoBack> MonitoramentoBack { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DominioMap());
            modelBuilder.ApplyConfiguration(new CelulaMap());
            modelBuilder.ApplyConfiguration(new BroadcastMap());
            modelBuilder.ApplyConfiguration(new BroadcastItemMap());
            modelBuilder.ApplyConfiguration(new FuncionalidadeMap());
            modelBuilder.ApplyConfiguration(new GrupoMap());
            modelBuilder.ApplyConfiguration(new PerfilMap());
            modelBuilder.ApplyConfiguration(new UsuarioPerfilMap());
            modelBuilder.ApplyConfiguration(new VinculoPerfilFuncionalidadeMap());
            modelBuilder.ApplyConfiguration(new VinculoTipoCelulaTipoContabilMap());
            modelBuilder.ApplyConfiguration(new VisualizacaoCelulaMap());
            modelBuilder.ApplyConfiguration(new PessoaMap());
            modelBuilder.ApplyConfiguration(new TipoCelulaMap());
            modelBuilder.ApplyConfiguration(new MonitoramentoBackMap());
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
                                if (!property.CurrentValue.Equals(property.OriginalValue))
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

