using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ClienteServicoPrestadorMap : IEntityTypeConfiguration<ClienteServicoPrestador>
    {
        public void Configure(EntityTypeBuilder<ClienteServicoPrestador> builder)
        {
            builder.ToTable("TBLCLIENTESERVICOPRESTADOR");

            builder.Property(e => e.Id)
               .HasColumnName("IDCLIENTESERVICOPRESTADOR");

            builder.Property(e => e.IdPrestador)
                .HasColumnName("IDPRESTADOR");

            builder.Property(e => e.IdCelula)
                .HasColumnName("IDCELULA");

            builder.Property(e => e.IdCliente)
                .HasColumnName("IDCLIENTE");

            builder.Property(e => e.IdServico)
                .HasColumnName("IDSERVICO");

            builder.Property(e => e.IdLocalTrabalho)
                .HasColumnName("IDLOCALTRABALHO");

            builder.Property(e => e.DescricaoTrabalho)
                .HasColumnName("DESCTRABALHO");

            builder.Property(e => e.DataInicio)
                .HasColumnName("DTINICIO");

            builder.Property(e => e.DataPrevisaoTermino)
                .HasColumnName("DTPREVISAOTERMINO");

            builder.Property(e => e.ValorCusto)
                .HasColumnName("VLCUSTO");

            builder.Property(e => e.ValorVenda)
                .HasColumnName("VLVENDA");

            builder.Property(e => e.ValorRepasse)
                .HasColumnName("VLREPASSE");

            builder.Property(e => e.Ativo)
                .HasColumnName("FLATIVO");

            builder.Property(x => x.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
               .HasColumnName("LGUSUARIO");

            builder.HasOne(e => e.Prestador)
               .WithMany(p => p.ClientesServicosPrestador)
               .HasForeignKey(e => e.IdPrestador);

            builder.HasOne(e => e.Celula)
               .WithMany(p => p.ClientesServicosPrestador)
               .HasForeignKey(e => e.IdCelula);
        }
    }
}
