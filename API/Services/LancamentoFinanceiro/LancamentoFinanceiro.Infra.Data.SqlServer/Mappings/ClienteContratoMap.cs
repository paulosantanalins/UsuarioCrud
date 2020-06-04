using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class ClienteContratoMap : IEntityTypeConfiguration<ClienteContrato>
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

            builder.Ignore(x => x.DtAlteracao);
            builder.Ignore(x => x.LgUsuario);
        }
    }
}
