
using ControleAcesso.Domain.BroadcastRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class BroadcastItemMap : IEntityTypeConfiguration<BroadcastItem>
    {
        public void Configure(EntityTypeBuilder<BroadcastItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLBROADCASTITEM");

            builder.Property(e => e.Id)
                .HasColumnName("IDBROADCASTITEM");

            builder.Property(e => e.Descricao)
                            .HasColumnName("DESCRICAO");

            builder.Property(e => e.Usuario)
                    .HasColumnName("LGUSUARIO")
                    .IsRequired(false);

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.HasMany(x => x.Broadcasts).WithOne(x => x.BroadcastItem);
        }
    }
}