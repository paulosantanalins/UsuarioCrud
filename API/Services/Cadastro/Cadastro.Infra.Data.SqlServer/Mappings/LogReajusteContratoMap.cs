using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class LogReajusteContratoMap : IEntityTypeConfiguration<LogReajusteContrato>
    {
        public void Configure(EntityTypeBuilder<LogReajusteContrato> builder)
        {
            builder.ToTable("TBLLOGREAJUSTECONTRATO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IDLOGREAJUSTECONTRATO");

            builder.Property(x => x.IdReajusteContrato)
                .HasColumnName("IDREAJUSTECONTRATO")
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

            builder.Property(x => x.Acao)
                .HasColumnName("ACAO")
                .IsRequired();

            builder.Property(x => x.Motivo)
                .HasColumnName("MOTIVO")
                .IsRequired(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasOne(x => x.ReajusteContrato)
                .WithMany(x => x.LogsReajusteContratos)
                .HasForeignKey(x => x.IdReajusteContrato);
        }
    }
}
