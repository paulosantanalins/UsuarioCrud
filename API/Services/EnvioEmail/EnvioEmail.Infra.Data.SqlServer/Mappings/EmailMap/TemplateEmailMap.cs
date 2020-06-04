using EnvioEmail.Domain.EmailRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvioEmail.Infra.Data.SqlServer.Mappings.EmailMap
{
    public class TemplateEmailMap : IEntityTypeConfiguration<TemplateEmail>
    {
        public void Configure(EntityTypeBuilder<TemplateEmail> entity)
        {

            entity.HasKey(e => e.Id);

            entity.ToTable("TBLTEMPLATE");

            entity.Property(e => e.Id).HasColumnName("IDTEMPLATE");

            entity.Property(e => e.Assunto)
                .IsRequired()
                .HasColumnName("DESCASSUNTOTEMPLATE")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.Property(e => e.Corpo)
                .IsRequired()
                .HasColumnName("DESCTEMPLATE")
                .IsUnicode(false);

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasColumnName("NMTEMPLATE")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.Property(e => e.FlagFixo)
                .IsRequired()
                .HasColumnName("FLFIXO");

            entity.Property(x => x.DataAlteracao)
               .HasColumnName("DTALTERACAO")
               .IsRequired(false);

            entity.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");
        }
    }
}
