using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class GrupoMap : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.ToTable("TBLGRUPO");

            builder.HasKey(p => new { p.Id });
            
            builder.Property(p => p.Id)
                .HasColumnName("IDGRUPO")
                .IsRequired();

            builder.Property(x => x.DescGrupo)
               .HasMaxLength(200)
               .IsRequired(false)
               .IsUnicode(false)
               .HasColumnName("DESCGRUPO");

            builder.Property(e => e.Ativo)
                .HasColumnName("FLATIVO");

            builder.Property(e => e.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);
        }
    }
}
