using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.CelulaRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class CelulaMap : IEntityTypeConfiguration<Celula>
    {
        public void Configure(EntityTypeBuilder<Celula> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLCELULA");

            builder.Property(e => e.Id).HasColumnName("IDCELULA");

            builder.Property(e => e.FlHabilitarRepasseMesmaCelula)
               .HasColumnName("FLHABILITARREPASSEMESMACELULA");

            builder.Property(e => e.FlHabilitarRepasseEpm)
                .HasColumnName("FLHABILITARREPASSEEPM");

            builder.Property(e => e.DataAlteracao)
               .HasColumnName("DTULTIMAALTERACAO")
               .IsRequired(false);

            builder.Property(e => e.Usuario)
               .HasColumnName("LGUSUARIO")
               .IsRequired(true);   

            builder.Property(e => e.DescCelula)
                .IsRequired()
                .HasColumnName("DESCCELULA")
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);
        }
    }
}
