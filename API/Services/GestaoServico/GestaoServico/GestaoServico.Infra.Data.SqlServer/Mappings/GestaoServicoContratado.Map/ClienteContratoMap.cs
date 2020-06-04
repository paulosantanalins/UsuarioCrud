using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map
{
    class ClienteContratoMap : IEntityTypeConfiguration<ClienteContrato>
    {
        public void Configure(EntityTypeBuilder<ClienteContrato> builder)
        {
            builder.ToTable("TBLCLIENTECONTRATO");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDCLIENTECONTRATO");
            builder.Property(e => e.IdCliente).HasColumnName("IDCLIENTE");
            builder.Property(e => e.IdContrato).HasColumnName("IDCONTRATO");

            builder.HasOne(x => x.Contrato)
                .WithMany(x => x.ClientesContratos)
                .HasForeignKey(x => x.IdContrato);

            builder.Ignore(x => x.DataAlteracao);
            builder.Ignore(x => x.Usuario);
        }
    }
}
