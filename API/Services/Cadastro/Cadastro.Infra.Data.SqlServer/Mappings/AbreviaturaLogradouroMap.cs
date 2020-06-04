using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class AbreviaturaLogradouroMap : IEntityTypeConfiguration<AbreviaturaLogradouro>
    {
        public void Configure(EntityTypeBuilder<AbreviaturaLogradouro> builder)
        {
            builder.ToTable("TBLABREVIATURALOGRADOURO");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDABREVIATURALOGRADOURO");

            builder.Property(e => e.Sigla)
                .HasColumnName("NMSIGLA")
                .IsUnicode(false)
                .IsRequired(true);

            builder.Property(e => e.Descricao)
                .HasColumnName("NMDESCRICAO")
                .IsUnicode(false)
                .IsRequired(true);

            builder.HasMany(p => p.Enderecos)
                .WithOne(x => x.AbreviaturaLogradouro)
                .HasForeignKey(p => p.SgAbrevLogradouro)
                .HasPrincipalKey(x => x.Sigla);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
    }
}
