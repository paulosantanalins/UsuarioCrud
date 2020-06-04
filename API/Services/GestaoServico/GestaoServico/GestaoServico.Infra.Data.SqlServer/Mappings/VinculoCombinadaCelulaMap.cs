using GestaoServico.Domain.GestaoCelulaRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings
{
    public class VinculoCombinadaCelulaMap : IEntityTypeConfiguration<VinculoCombinadaCelula>
    {
        public void Configure(EntityTypeBuilder<VinculoCombinadaCelula> builder)
        {
            builder.ToTable("tblVinculoCombinadaCelula");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idCombinada")
                .IsRequired(true);

            builder.Property(p => p.DtInicio)
                .HasColumnName("dtInicio")
                .IsRequired(true);

            builder.Property(p => p.DtFim)
                .HasColumnName("dtFim")
                .IsRequired(false);

            builder.Property(p => p.IdCelula)
                .HasColumnName("idCelula")
                .IsRequired(true);

        }
    }
}
