using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class DeliveryMap : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLDELIVERY");

            builder.Property(e => e.Id).HasColumnName("IDDELIVERY");

            builder.Property(e => e.DescDelivery)
                .IsRequired()
                .HasColumnName("DESCDELIVERY")
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.Property(e => e.SgDelivery)
                .IsRequired()
                .HasColumnName("SGDELIVERY")
                 .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(e => e.FlStatus)
             .IsRequired()
             .HasColumnName("FLSTATUS")
             .HasColumnType("char(1)");

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
