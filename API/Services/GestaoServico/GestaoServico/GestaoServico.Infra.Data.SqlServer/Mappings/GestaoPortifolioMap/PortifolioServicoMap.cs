using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class PortifolioServicoMap : IEntityTypeConfiguration<PortfolioServico>
    {
        public void Configure(EntityTypeBuilder<PortfolioServico> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLPORTFOLIOSERVICO");

            builder.Property(e => e.Id).HasColumnName("IDPORTFOLIOSERVICO");

            builder.Property(e => e.NmServico)
                .IsRequired()
                .HasColumnName("NMPORTFOLIOSERVICO")
                .HasMaxLength(70)
                .IsUnicode(false);

            builder.Property(e => e.DescServico)
                .IsRequired()
                .HasColumnName("DESCPORTFOLIOSERVICO")
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.Property(e => e.FlStatus)
             .IsRequired()
             .HasColumnName("FLSTATUS");

            builder.Property(e => e.IdDelivery).HasColumnName("IDDELIVERY");
                       
            builder.HasOne(d => d.Delivery)
                .WithMany(p => p.PortifolioServicos)
                .HasForeignKey(d => d.IdDelivery)
                .OnDelete(DeleteBehavior.ClientSetNull);

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
