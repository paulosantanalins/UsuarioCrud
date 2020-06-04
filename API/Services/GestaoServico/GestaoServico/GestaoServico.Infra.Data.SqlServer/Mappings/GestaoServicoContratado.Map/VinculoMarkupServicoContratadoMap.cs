using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map
{
    public class VinculoMarkupServicoContratadoMap : IEntityTypeConfiguration<VinculoMarkupServicoContratado>
    {
        public void Configure(EntityTypeBuilder<VinculoMarkupServicoContratado> builder)
        {
            builder.ToTable("TBLVINCULOMARKUPSERVICOCONTRATADO");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("idVinculoMarkupServicoContratado");

            builder.Property(p => p.VlMarkup).HasColumnName("vlMarkup");

            builder.Property(p => p.DtInicioVigencia).HasColumnName("dtInicioVigencia");

            builder.Property(p => p.DtFimVigencia).HasColumnName("dtFimVigencia");

            builder.Property(p => p.IdServicoContratado).HasColumnName("idServicoContratado");

            builder.HasOne(p => p.ServicoContratado)
                .WithMany(p => p.VinculoMarkupServicosContratados)
                .HasForeignKey(p => p.IdServicoContratado);

            builder.Ignore(x => x.DataAlteracao);

            builder.Ignore(x => x.Usuario);
        }
    }
}
