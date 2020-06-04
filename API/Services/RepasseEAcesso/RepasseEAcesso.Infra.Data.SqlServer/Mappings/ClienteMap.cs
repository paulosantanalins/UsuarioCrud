using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("tblClientes");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("IdCliente");

            builder.Property(p => p.CNPJ)
                .HasColumnName("CNPJ");
            builder.Property(p => p.NomeFantasia)
                .HasColumnName("NomeFantasia");
            builder.Property(p => p.RazaoSocial)
                .HasColumnName("RazaoSocial");
            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
