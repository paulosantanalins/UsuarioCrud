using EnvioEmail.Domain.EmailRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvioEmail.Infra.Data.SqlServer.Mappings.EmailMap
{
    class ParametroTemplateMap : IEntityTypeConfiguration<ParametroTemplate>
    {
        public void Configure(EntityTypeBuilder<ParametroTemplate> entity)
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TBLPARAMETROTEMPLATE");

            entity.Property(e => e.Id).HasColumnName("IDPARAMETROTEMPLATE");

            entity.Property(e => e.IdTemplate).HasColumnName("IDTEMPLATE");

            entity.Property(e => e.NomeParametro)
                .IsRequired()
                .HasColumnName("NMPARAMETRO")
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.Template)
                .WithMany(p => p.Parametros)
                .HasForeignKey(d => d.IdTemplate);
        }
    }
}
