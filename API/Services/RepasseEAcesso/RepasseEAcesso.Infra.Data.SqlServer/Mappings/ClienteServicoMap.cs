using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class ClienteServicoMap : IEntityTypeConfiguration<ClienteServico>
    {
        public void Configure(EntityTypeBuilder<ClienteServico> builder)
        {
            builder.ToTable("tblClientesServicos");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("IdServico");
            builder.Property(p => p.Nome)
                .HasColumnName("Nome");
            builder.Property(p => p.IdCliente)
                .HasColumnName("IdCliente");
            builder.Property(p => p.IdCelula)
                .HasColumnName("IdCelula");
            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
