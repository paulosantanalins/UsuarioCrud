using Cliente.Domain.ClienteRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cliente.Infra.Data.SqlServer.Mappings
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("tblEndereco");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idEndereco")
                .IsRequired(true);

            builder.Property(p => p.IdCidade)
                .HasColumnName("idCidade")
                .IsRequired(false);

            builder.Property(p => p.IdCliente)
                .HasColumnName("idCliente")
                .IsRequired(false);

            builder.Property(p => p.SgAbrevLogradouro)
                .HasColumnName("sgAbrevLogradouro")
                .IsRequired(true);

            builder.Property(p => p.NmEndereco)
                .HasColumnName("nmEndereco")
                .IsRequired(true);

            builder.Property(p => p.NrEndereco)
                .HasColumnName("nrEndereco")
                .IsRequired(true);

            builder.Property(p => p.NmCompEndereco)
                .HasColumnName("nmCompEndereco")
                .IsRequired(true);

            builder.Property(p => p.NmBairro)
                .HasColumnName("nmBairro")
                .IsRequired(true);

            builder.Property(p => p.NrCep)
                .HasColumnName("nrCep")
                .IsRequired(true);

            //builder.Property(p => p.Referencia)
            //    .HasColumnName("DESCREFERENCIA");

            builder.Ignore(e => e.DataAlteracao);

            builder.Ignore(e => e.Usuario);
        }
    }
}
