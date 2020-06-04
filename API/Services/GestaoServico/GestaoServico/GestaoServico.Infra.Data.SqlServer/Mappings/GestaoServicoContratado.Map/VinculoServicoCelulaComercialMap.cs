using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map
{
    public class VinculoServicoCelulaComercialMap : IEntityTypeConfiguration<VinculoServicoCelulaComercial>
    {
        public void Configure(EntityTypeBuilder<VinculoServicoCelulaComercial> builder)
        {
            builder.ToTable("TBLVinculoServicoCelulaComercial");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("idVinculoServicoCelulaComercial");

            builder.Property(p => p.IdServicoContratado).HasColumnName("IDSERVICOCONTRATADO");

            builder.Property(p => p.IdCelulaComercial).HasColumnName("IDCELULACOMERCIAL");

            builder.Property(p => p.DataInicial).HasColumnName("DTINICIO");

            builder.Property(p => p.DataFinal).HasColumnName("DTFIM");

            builder.HasOne(p => p.ServicoContratado)
                .WithMany(p => p.VinculoServicoCelulaComercial)
                .HasForeignKey(p => p.IdServicoContratado);

            builder.HasOne(p => p.Celula)
                .WithMany(p => p.VinculoServicoCelulaComercial)
                .HasForeignKey(p => p.IdCelulaComercial);

            builder.Ignore(x => x.DataAlteracao);

            builder.Ignore(x => x.Usuario);
        }
    }
}
