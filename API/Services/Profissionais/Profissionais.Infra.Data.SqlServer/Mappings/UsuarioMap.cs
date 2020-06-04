using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UsuarioApi.Domain.DominioRoot.Entity;

namespace UsuarioApi.Infra.Data.SqlServer.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {       
            builder.ToTable("USUARIO");

            builder.HasKey(x => x.Id);


            builder.Property(p => p.Nome)
                .HasColumnName("NOME")
                .IsRequired();

            builder.Property(p => p.Sobrenome)
                .HasColumnName("SOBRENOME")
                .IsRequired();

            builder.Property(p => p.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(100);

            builder.Property(p => p.DataNasc)
                .HasColumnName("DTNASCIMENTO");

            builder.Property(p => p.Escolaridade)
                .HasColumnName("ESCOLARIDADE")
                .IsRequired();
        }
    }
}
