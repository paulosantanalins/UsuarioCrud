using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ReajusteContratoMap : IEntityTypeConfiguration<ReajusteContrato>
    {
        public void Configure(EntityTypeBuilder<ReajusteContrato> builder)
        {
            builder.ToTable("TBLREAJUSTECONTRATO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IDREAJUSTECONTRATO");

            builder.Property(x => x.IdPrestador)
                .HasColumnName("IDPRESTADOR")
                .IsRequired();

            builder.Property(x => x.IdTipoContrato)
                .HasColumnName("IDTIPOCONTRATO")
                .IsRequired();

            builder.Property(x => x.QuantidadeHorasContrato)
                .HasColumnName("QTDHORASCONTRATO")
                .IsRequired();

            builder.Property(x => x.ValorContrato)
                .HasColumnName("VALORCONTRATO")
                .IsRequired();

            builder.Property(x => x.DataInclusao)
                .HasColumnName("DTINCLUSAO")
                .IsRequired();

            builder.Property(x => x.DataReajuste)
                .HasColumnName("DTREAJUSTE")
                .IsRequired();

            builder.Property(x => x.Situacao)
                .HasColumnName("SITUACAO")
                .IsRequired();

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasOne(x => x.Prestador)
                .WithMany(x => x.ReajustesContratos)
                .HasForeignKey(x => x.IdPrestador);
        }
    }
}
