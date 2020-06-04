using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ObservacaoPrestadorMap : IEntityTypeConfiguration<ObservacaoPrestador>
    {
        public void Configure(EntityTypeBuilder<ObservacaoPrestador> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLPRESTADOROBSERVACAO");

            builder.Property(e => e.Id)
                .HasColumnName("IDPRESTADOROBSERVACAO");

            builder.Property(e => e.IdPrestador)
                .HasColumnName("IDPRESTADOR");

            builder.Property(e => e.IdTipoOcorencia)
               .HasColumnName("IDTIPOOCORRENCIA");

            builder.Property(e => e.Status)
                .HasColumnName("STATUS")
                .IsUnicode(false);

            builder.Property(e => e.Observacao)
                .HasColumnName("OBSERVACAO")
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(e => e.Prestador)
                .WithMany(p => p.ObservacoesPrestador)
                .HasForeignKey(e => e.IdPrestador);

            builder.HasOne(e => e.TipoOcorrencia)
                .WithMany(p => p.ObservacaoPrestador)
                .HasForeignKey(d => d.IdTipoOcorencia);
        }
    }
}
