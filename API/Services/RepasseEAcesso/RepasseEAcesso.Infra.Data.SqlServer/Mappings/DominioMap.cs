using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.DominioRoot.ItensDominio;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class DominioMap : IEntityTypeConfiguration<Dominio>
    {
        public void Configure(EntityTypeBuilder<Dominio> builder)
        {
            builder.ToTable("TBLDOMINIO");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDDOMINIO")
                .IsRequired(true);

            builder.Property(p => p.ValorTipoDominio)
                .HasColumnName("VLTIPODOMINIO")
                .IsRequired(true);

            builder.Property(p => p.IdValor)
                .HasColumnName("IDVALOR")
                .IsRequired(true);

            builder.Property(p => p.DescricaoValor)
                .HasColumnName("VLDOMINIO")
                .IsRequired(true);

            builder.Property(p => p.Ativo)
                .HasColumnName("FLATIVO")
                .IsRequired(true);

            CreateDiscriminators(builder);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
        private void CreateDiscriminators(EntityTypeBuilder<Dominio> builder)
        {
            builder.HasDiscriminator(x => x.ValorTipoDominio)
                .HasValue<DomMoeda>("MOEDA");
            builder.HasDiscriminator(x => x.ValorTipoDominio)
                .HasValue<DomStatusRepasse>("STATUS_REPASSE");  
        }

    }
}
