using GestaoServico.Domain.GestaoCelulaRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings
{
    public class VinculoCelulaServicoMap : IEntityTypeConfiguration<VinculoCelulaServico>
    {
        public void Configure(EntityTypeBuilder<VinculoCelulaServico> builder)
        {
            builder.ToTable("tblVinculoCelulaServico");

            builder.HasKey(p => new
            {
                p.IdServico,
                p.IdCelula
            });

            builder.Property(p => p.IdServico)
                .HasColumnName("idServico")
                .IsRequired(true);

            builder.Property(p => p.IdCelula)
                .HasColumnName("idCelula")
                .IsRequired(true);

        }
    }
}
