using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class TipoServicoMap : IEntityTypeConfiguration<TipoServico>
    {
        public void Configure(EntityTypeBuilder<TipoServico> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLTIPOSERVICO");

            builder.Property(e => e.Id).HasColumnName("IDTIPOSERVICO");


            builder.Property(e => e.DescTipoServico)
                .IsRequired()
                .HasColumnName("DESCTIPOSERVICO")
                .HasMaxLength(150)
                .IsUnicode(false);

            builder.Property(e => e.FlStatus)
               .IsRequired()
               .HasColumnName("FLSTATUS")
               .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");
        }
    }
}
