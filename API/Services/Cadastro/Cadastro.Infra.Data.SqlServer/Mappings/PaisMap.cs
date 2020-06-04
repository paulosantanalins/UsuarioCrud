using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class PaisMap : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("tblPais");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idPais")
                .IsRequired(true);

            builder.Property(p => p.SgPais)
                .HasColumnName("sgPais")
                .IsRequired(true);

            builder.Property(p => p.NmPais)
                .HasColumnName("nmPais")
                .IsRequired(true);

            builder.HasMany(p => p.Estados)
                .WithOne(x => x.Pais)
                .HasForeignKey(x => x.IdPais)
                .IsRequired(true);

            builder.Ignore(e => e.DataAlteracao);

            builder.Ignore(e => e.Usuario);
        }
    }
}
