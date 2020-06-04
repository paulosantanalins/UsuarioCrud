using GestaoServico.Domain.GestaoServicoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings
{
    public class VinculoServicoTipoServicoMap : IEntityTypeConfiguration<VinculoServicoTipoServico>
    {
        public void Configure(EntityTypeBuilder<VinculoServicoTipoServico> builder)
        {
            builder.ToTable("tblVinculoServicoTipoServico");

            builder.HasKey(p => new
            {
                p.IdServico,
                p.IdTipoServico
            });

            builder.Property(p => p.IdServico)
                .HasColumnName("idServico")
                .IsRequired(true);

            builder.Property(p => p.IdTipoServico)
                .HasColumnName("idTipoServico")
                .IsRequired(true);

        }
    }
}
