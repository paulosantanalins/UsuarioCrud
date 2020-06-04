using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class EstadoMap : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.ToTable("tblEstado");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idEstado")
                .IsRequired(true);

            builder.Property(p => p.SgEstado)
                .HasColumnName("sgEstado")
                .IsRequired(true);

            builder.Property(p => p.NmEstado)
                .HasColumnName("nmEstado")
                .IsRequired(true);

            builder.Property(p => p.IdPais)
                 .HasColumnName("idPais")
                 .IsRequired(true);

            builder.HasMany(p => p.Cidades)
                .WithOne(x => x.Estado)
                .HasForeignKey(x => x.IdEstado)
                .IsRequired(true);

            builder.Ignore(e => e.DataAlteracao);

            builder.Ignore(e => e.Usuario);
        }
    }
}
