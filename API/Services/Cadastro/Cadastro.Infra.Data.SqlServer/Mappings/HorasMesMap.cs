using Cadastro.Domain.HorasMesRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class HorasMesMap : IEntityTypeConfiguration<HorasMes>
    {
        public void Configure(EntityTypeBuilder<HorasMes> builder)
        {
            builder.ToTable("tblHorasMes");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDHORASMES");

            builder.Property(p => p.Mes)
                .HasColumnName("VLMES");

            builder.Property(p => p.Ano)
                .HasColumnName("VLANO");

            builder.Property(p => p.Horas)
                .HasColumnName("VLHORAS");

            builder.Property(p => p.Ativo)
                .HasColumnName("FLATIVO");

            builder.Property(x => x.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);
        }
    }
}
