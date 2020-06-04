
using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class CidadeMap : IEntityTypeConfiguration<Cidade>
    {
        public void Configure(EntityTypeBuilder<Cidade> builder)
        {
            builder.ToTable("tblCidade");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idCidade");

            builder.Property(p => p.IdEstado)
                .HasColumnName("idEstado");

            builder.Property(p => p.NmCidade)
                .HasColumnName("nmCidade");

            builder.Property(p => p.CodIBGE)
              .HasColumnName("nmCidade")
              .IsRequired(false);

            builder.HasMany(p => p.Enderecos)
                .WithOne(x => x.Cidade)
                .HasForeignKey(x => x.IdCidade);

            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
