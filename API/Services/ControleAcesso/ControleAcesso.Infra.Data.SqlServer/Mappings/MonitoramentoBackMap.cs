using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings
{
    public class MonitoramentoBackMap : IEntityTypeConfiguration<MonitoramentoBack>
    {
        public void Configure(EntityTypeBuilder<MonitoramentoBack> builder)
        {
            builder.ToTable("TBLLOGGENERICO");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDLOGGENERICO")
                .IsRequired(true);

            builder.Property(p => p.TipoLog)
                .HasColumnName("NMTIPOLOG")
                .IsRequired(true);

            builder.Property(p => p.Origem)
                .HasColumnName("NMORIGEM")
                .IsRequired(true);

            builder.Property(p => p.DetalheLog)
                .HasColumnName("DESCLOGGENERICO")
                .IsRequired(true);

            builder.Property(p => p.StackTrace)
                .HasColumnName("DESCEXCECAO")
                .IsRequired(true);

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("DTHORALOGGENERICO")
                .IsRequired(true);

            builder.Ignore(x => x.Usuario);
        }
    }
}
