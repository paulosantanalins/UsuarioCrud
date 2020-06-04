using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class EscopoServicoMap : IEntityTypeConfiguration<EscopoServico>
    {
        public void Configure(EntityTypeBuilder<EscopoServico> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLESCOPOSERVICO");

            builder.Property(e => e.Id).HasColumnName("IDESCOPOSERVICO");

            builder.Property(e => e.NmEscopoServico)
                .IsRequired()
                .HasColumnName("NMESCOPOSERVICO")
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.FlAtivo)
                .IsRequired()
                .HasColumnName("FLATIVO");

            builder.Property(e => e.IdPortfolioServico)
                .HasColumnName("IDPORTFOLIOSERVICO")
                .IsRequired();

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
