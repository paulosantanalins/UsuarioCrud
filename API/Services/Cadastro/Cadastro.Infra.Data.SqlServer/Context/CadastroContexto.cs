using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.LinkRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Context
{
    public class CadastroContexto : DbContext
    {
        private readonly IVariablesToken _variables;

        public CadastroContexto(IVariablesToken variables)
        {
            _variables = variables;
        }

        public virtual DbSet<Auditoria> Auditoria { get; set; }
        public virtual DbSet<Endereco> Endereco { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Cidade> Cidade { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Dominio> Dominio { get; set; }
        public virtual DbSet<Grupo> Grupo { get; set; }
        public virtual DbSet<TipoCelula> TipoCelula { get; set; }
        public virtual DbSet<Celula> Celula { get; set; }
        public virtual DbSet<Prestador> Prestador { get; set; }
        public virtual DbSet<Telefone> Telefone { get; set; }
        public virtual DbSet<HorasMes> HorasMes { get; set; }
        public virtual DbSet<HorasMesPrestador> HorasMesPrestador { get; set; }
        public virtual DbSet<LogHorasMesPrestador> LogHorasMesPrestador { get; set; }
        public virtual DbSet<LogTransferenciaPrestador> LogsTransferenciaPrestador { get; set; }
        public virtual DbSet<ValorPrestador> ValorPrestador { get; set; }
        public virtual DbSet<PeriodoDiaPagamento> PeriodoDiaPagamento { get; set; }
        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<EmpresaPrestador> EmpresaPrestador { get; set; }
        public virtual DbSet<AbreviaturaLogradouro> AbreviaturaLogradouro { get; set; }
        public virtual DbSet<DescontoPrestador> DescontoPrestador { get; set; }
        public virtual DbSet<PrestadorEnvioNf> PrestadorEnvioNf { get; set; }
        public virtual DbSet<InativacaoPrestador> InativacaoPrestador { get; set; }
        public virtual DbSet<ValorPrestadorBeneficio> ValorPrestadorBeneficios { get; set; }
        public virtual DbSet<ClienteServicoPrestador> ClienteServicoPrestador { get; set; }
        public virtual DbSet<Link> Link { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }
        public virtual DbSet<ObservacaoPrestador> ObservacaoPrestador { get; set; }
        public virtual DbSet<TransferenciaCltPj> TransferenciaCltPj { get; set; }
        public virtual DbSet<SocioEmpresaPrestador> SociosEmpresaPrestador { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuditoriaMap());
            modelBuilder.ApplyConfiguration(new EnderecoMap());
            modelBuilder.ApplyConfiguration(new CidadeMap());
            modelBuilder.ApplyConfiguration(new EstadoMap());
            modelBuilder.ApplyConfiguration(new PaisMap());
            modelBuilder.ApplyConfiguration(new DominioMap());
            modelBuilder.ApplyConfiguration(new GrupoMap());
            modelBuilder.ApplyConfiguration(new TipoCelulaMap());
            modelBuilder.ApplyConfiguration(new CelulaMap());
            modelBuilder.ApplyConfiguration(new PrestadorMap());
            modelBuilder.ApplyConfiguration(new TelefoneMap());
            modelBuilder.ApplyConfiguration(new HorasMesMap());
            modelBuilder.ApplyConfiguration(new HorasMesPrestadorMap());
            modelBuilder.ApplyConfiguration(new LogHorasMesPrestadorMap());
            modelBuilder.ApplyConfiguration(new LogTransferenciaPrestadorMap());
            modelBuilder.ApplyConfiguration(new ValorPrestadorMap());
            modelBuilder.ApplyConfiguration(new PeriodoDiaPagamentoMap());
            modelBuilder.ApplyConfiguration(new EmpresaMap());
            modelBuilder.ApplyConfiguration(new EmpresaPrestadorMap());
            modelBuilder.ApplyConfiguration(new AbreviaturaLogradouroMap());
            modelBuilder.ApplyConfiguration(new PrestadorEnvioNfMap());
            modelBuilder.ApplyConfiguration(new InativacaoPrestadorMap());
            modelBuilder.ApplyConfiguration(new DescontoPrestadorMap());
            modelBuilder.ApplyConfiguration(new ValorPrestadorBeneficioMap());
            modelBuilder.ApplyConfiguration(new ClienteServicoPrestadorMap());
            modelBuilder.ApplyConfiguration(new LinkMap());
            modelBuilder.ApplyConfiguration(new PessoaMap());
            modelBuilder.ApplyConfiguration(new ObservacaoPrestadorMap());
            modelBuilder.ApplyConfiguration(new ContratoPrestadorMap());
            modelBuilder.ApplyConfiguration(new ExtensaoContratoPrestadorMap());
            modelBuilder.ApplyConfiguration(new DocumentoPrestadorMap());
            modelBuilder.ApplyConfiguration(new ClienteETMap());
            modelBuilder.ApplyConfiguration(new TransferenciaPrestadorMap());
            modelBuilder.ApplyConfiguration(new TransferenciaCltPjMap());
            modelBuilder.ApplyConfiguration(new FinalizacaoContratoMap());
            modelBuilder.ApplyConfiguration(new SocioEmpresaPrestadorMap());
            modelBuilder.ApplyConfiguration(new LogFinalizacaoContratoMap());
            modelBuilder.ApplyConfiguration(new ReajusteContratoMap());
            modelBuilder.ApplyConfiguration(new LogReajusteContratoMap());
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