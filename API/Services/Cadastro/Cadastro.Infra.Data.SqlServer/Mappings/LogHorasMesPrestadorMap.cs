using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class LogHorasMesPrestadorMap : IEntityTypeConfiguration<LogHorasMesPrestador>
    {
        public void Configure(EntityTypeBuilder<LogHorasMesPrestador> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLLOGHORASMESPRESTADOR");

            builder.Property(e => e.Id)
                .HasColumnName("IDLOGHORASMESPRESTADOR");

            builder.Property(e => e.IdHorasMesPrestador)
                .HasColumnName("IDHORASMESPRESTADOR");

            builder.Property(e => e.SituacaoAnterior)
                .HasColumnName("NMSITUACAOANTERIOR")
                .IsRequired(false)
                .IsUnicode(false);

            builder.Property(e => e.SituacaoNova)
                .HasColumnName("NMSITUACAONOVA")
                .IsRequired(true)
                .IsUnicode(false);

            builder.Property(e => e.DescMotivo)
                .HasColumnName("DESCMOTIVO")
                .IsRequired(false)
                .IsUnicode(false)
                .HasMaxLength(500);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false)
                ;

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(e => e.HorasMesPrestador)
                .WithMany(p => p.LogsHorasMesPrestador)
                .HasForeignKey(e => e.IdHorasMesPrestador);
        }
    }
}
