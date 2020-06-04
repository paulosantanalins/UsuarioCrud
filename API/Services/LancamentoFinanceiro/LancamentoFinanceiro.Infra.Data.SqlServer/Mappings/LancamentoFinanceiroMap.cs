using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class LancamentoFinanceiroMap : IEntityTypeConfiguration<RootLancamentoFinanceiro>
    {
        public void Configure(EntityTypeBuilder<RootLancamentoFinanceiro> entity)
        {
            entity.ToTable("TBLLANCAMENTOFINANCEIRO");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("IDLANCAMENTOFINANCEIRO");

            entity.Property(e => e.CodigoColigada)
                .HasColumnName("CDCOLIGADA")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.DescricaoOrigemLancamento)
                .IsRequired()
                .HasColumnName("DESCORIGEMLANCAMENTO")
                .HasColumnType("char(2)");

            entity.Property(e => e.DescricaoTipoLancamento)
                .IsRequired()
                .HasColumnName("DESCTIPOLANCAMENTO")
                .HasColumnType("char(1)");

            entity.Property(e => e.DtAlteracao)
                .HasColumnName("DTALTERACAO")
                .HasColumnType("date");

            entity.Property(e => e.DtBaixa)
                .HasColumnName("DTBAIXA")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.DtLancamento)
                .HasColumnName("DTLANCAMENTO")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.IdLan).HasColumnName("IDLAN");

            entity.Property(e => e.IdTipoDespesa).HasColumnName("IDTIPODESPESA");

            entity.Property(e => e.DescOrigemCompraEacesso).HasColumnName("DescOrigemCompraEacesso");

            entity.Property(e => e.LgUsuario)
                .IsRequired()
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(x => x.TipoDespesa)
                .WithMany(x => x.LancamentosFinanceiros)
                .HasForeignKey(x => x.IdTipoDespesa);


            entity.Property(e => e.VlImpDev).HasColumnName("VlImpDev");

            entity.Property(e => e.VlIr).HasColumnName("VlIr");

            entity.Property(e => e.VlIss).HasColumnName("VlIss");

            entity.Property(e => e.VlInssOp).HasColumnName("VALORINSSOP");

            entity.Property(e => e.VlDespesaRefeicao).HasColumnName("VlDespesaRefeicao");

            entity.Property(e => e.VlDespesaTelefonia).HasColumnName("VlDespesaTelefonia");

            entity.Property(e => e.VlDespesaTransporte).HasColumnName("VlDespesaTransporte");

            entity.Property(e => e.VlImpRet).HasColumnName("VlImpRet");

            entity.Property(e => e.VlDesconto).HasColumnName("VlDesconto");

            entity.Property(e => e.VlBaixado).HasColumnName("VlBaixado");

            entity.Property(e => e.VlOriginal).HasColumnName("VlOriginal");

            entity.Property(e => e.VlIrrf).HasColumnName("VlIrrf");

            entity.Property(e => e.VlJuros).HasColumnName("VlJuros");

            entity.Property(e => e.VlAdiantamento).HasColumnName("VlAdiantamento");

            entity.Property(e => e.VlInss).HasColumnName("VlInss");

        }
    }
}
