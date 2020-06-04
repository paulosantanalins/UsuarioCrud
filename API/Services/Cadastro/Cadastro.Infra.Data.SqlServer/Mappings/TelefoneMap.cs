using Cadastro.Domain.TelefoneRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class TelefoneMap : IEntityTypeConfiguration<Telefone>
    {
        public void Configure(EntityTypeBuilder<Telefone> builder)
        {
            builder.ToTable("TBLTELEFONE");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDTELEFONE");

            builder.Property(e => e.IdNextel)
                .HasColumnName("IDNEXTEL");

            builder.Property(e => e.NumeroNextel)
                .HasColumnName("NRNEXTEL");

            builder.Property(e => e.DDD)
                .HasColumnName("NRDDD");

            builder.Property(e => e.NumeroComercialRamal)
                .HasColumnName("NRCOMERCIALRAMAL");

            builder.Property(e => e.Celular)
                .HasColumnName("NRCELULAR");

            builder.Property(e => e.NumeroComercial)
                .HasColumnName("NRCOMERCIAL");

            builder.Property(e => e.NumeroResidencial)
                .HasColumnName("NRRESIDENCIAL");

            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
