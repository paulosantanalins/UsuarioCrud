using GestaoServico.Domain.GestaoServicoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings
{
    public class VinculoCombinadaServicoMap : IEntityTypeConfiguration<VinculoCombinadaServico>
    {
        public void Configure(EntityTypeBuilder<VinculoCombinadaServico> builder)
        {
            builder.ToTable("tblVinculoCombinadaServico");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idCombinada")
                .IsRequired(true);

            builder.Property(p => p.DtMigracao)
                .HasColumnName("dtMigracao")
                .IsRequired(true);

            builder.Property(p => p.IdServico)
                .HasColumnName("idServico")
                .IsRequired(true);

            builder.Property(p => p.FlStatus)
                .HasColumnName("flStatus")
                .HasColumnType("char(1)")
                .IsRequired(true);

        }
    }
}
