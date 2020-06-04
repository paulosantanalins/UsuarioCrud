using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class ItemLancamentoFinanceiroMap : IEntityTypeConfiguration<ItemLancamentoFinanceiro>
    {
        public void Configure(EntityTypeBuilder<ItemLancamentoFinanceiro> entity)
        {
            entity.ToTable("TBLITEMLANCAMENTOFINANCEIRO");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("IDITEMLANCAMENTOFINANCEIRO");

            entity.Property(e => e.DtAlteracao)
                .HasColumnName("DTALTERACAO")
                .HasColumnType("date");

            entity.Property(e => e.DtRepasse)
                .HasColumnName("DTREPASSE")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.IdLancamentoFinanceiro).HasColumnName("IDLANCAMENTOFINANCEIRO");

            entity.Property(e => e.IdRepasse).HasColumnName("IDREPASSE");

            entity.Property(e => e.IdServicoContratado).HasColumnName("IDSERVICOCONTRATADO");

            entity.Property(e => e.LgUsuario)
                .IsRequired()
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.VlLancamento)
                .HasColumnName("VLLANCAMENTO")
                .HasColumnType("decimal(9, 2)");

            entity.Property(e => e.VlInc)
                .HasColumnName("VLINC")
                .HasColumnType("decimal(9, 2)");

            entity.Property(e => e.VlDesc)
                .HasColumnName("VLDESCONTO")
                .HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.LancamentoFinanceiro)
                .WithMany(p => p.ItensLancamentoFinanceiro)
                .HasForeignKey(d => d.IdLancamentoFinanceiro)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
