using GestaoServico.Domain.GestaoContratoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings
{
    public class VinculoContratoServicoMap : IEntityTypeConfiguration<VinculoContratoServico>
    {
        public void Configure(EntityTypeBuilder<VinculoContratoServico> builder)
        {
            builder.ToTable("tblVinculoContratoServico");

            builder.HasKey(x => new
            {
                x.IdContrato,
                x.IdServico
            });

            builder.Property(p => p.IdContrato)
                .HasColumnName("idContrato")
                .IsRequired(true);

            builder.Property(p => p.IdServico)
                .HasColumnName("idServico")
                .IsRequired(true);
        }
    }
}
