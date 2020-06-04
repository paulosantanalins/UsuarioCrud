using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("TBLENDERECO");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idEndereco");

            builder.Property(p => p.IdCidade)
                .HasColumnName("idCidade");

            builder.Property(p => p.IdCliente)
                .HasColumnName("idCliente");

            builder.Property(p => p.SgAbrevLogradouro)
                .HasColumnName("sgAbrevLogradouro");

            builder.Property(p => p.NmEndereco)
                .HasColumnName("nmEndereco");

            builder.Property(p => p.NrEndereco)
                .HasColumnName("nrEndereco");

            builder.Property(p => p.NmCompEndereco)
                .HasColumnName("nmCompEndereco");

            builder.Property(p => p.NmBairro)
                .HasColumnName("nmBairro");

            builder.Property(p => p.NrCep)
                .HasColumnName("nrCep");

            //builder.Property(p => p.Referencia)
            //    .HasColumnName("DESCREFERENCIA");

            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
