using Cadastro.Domain.LinkRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class LinkMap : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.ToTable("TBLLINK");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDLINK");

            builder.Property(p => p.Nome)
                .HasColumnName("NMLINK");

            builder.Property(p => p.Url)
                .HasColumnName("VLURL");

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
    }
}
