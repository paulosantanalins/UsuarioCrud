using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class ContratoMap : IEntityTypeConfiguration<Contrato>
    {
        public void Configure(EntityTypeBuilder<Contrato> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLCONTRATO");

            builder.Property(e => e.Id).HasColumnName("IDCONTRATO");

            builder.Property(e => e.DtFinalizacao)
                .HasColumnName("DTFIMCONTRATO");

            builder.Property(e => e.DescContrato)
                .HasColumnName("DESCCONTRATO")
                .HasColumnType("VARCHAR(300)");

            builder.Property(e => e.DtInicial)
                .HasColumnName("DTINICIOCONTRATO");

            builder.Property(e => e.DescStatusSalesForce)
                .HasColumnName("DescStatusSalesForce")
                .HasColumnType("VARCHAR(60)");

            builder.Property(e => e.NrAssetSalesForce).HasColumnName("NRASSETSALESFORCE");

            builder.Property(e => e.IdMoeda).HasColumnName("IDMOEDA");

            builder.Property(x => x.DtAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.LgUsuario)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

        }
    }
}
