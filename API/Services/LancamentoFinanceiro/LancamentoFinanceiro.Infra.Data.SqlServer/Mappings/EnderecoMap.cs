using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("tblEndereco");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idEndereco")
                .IsRequired();

            builder.Property(p => p.IdCidade)
                .HasColumnName("idCidade")
                .IsRequired(false);

            builder.Property(p => p.IdCliente)
                .HasColumnName("idCliente")
                .IsRequired(false);

            builder.Property(p => p.SgAbrevLogradouro)
                .HasColumnName("sgAbrevLogradouro")
                .IsRequired();

            builder.Property(p => p.NmEndereco)
                .HasColumnName("nmEndereco")
                .IsRequired();

            builder.Property(p => p.NrEndereco)
                .HasColumnName("nrEndereco")
                .IsRequired();

            builder.Property(p => p.NmCompEndereco)
                .HasColumnName("nmCompEndereco")
                .IsRequired();

            builder.Property(p => p.NmBairro)
                .HasColumnName("nmBairro")
                .IsRequired();

            builder.Property(p => p.NrCep)
                .HasColumnName("nrCep")
                .IsRequired();

            //builder.Property(p => p.Referencia)
            //    .HasColumnName("DESCREFERENCIA");

            builder.Ignore(e => e.DtAlteracao);

            builder.Ignore(e => e.LgUsuario);
        }
    }
}
