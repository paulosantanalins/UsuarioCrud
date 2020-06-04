using Cadastro.Domain.CelulaRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    class GrupoMap : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.ToTable("TBLGRUPO");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDGRUPO");

            builder.Property(e => e.Descricao)
                .HasColumnName("DESCGRUPO")
                .IsUnicode(false)
                .IsRequired(false);

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
