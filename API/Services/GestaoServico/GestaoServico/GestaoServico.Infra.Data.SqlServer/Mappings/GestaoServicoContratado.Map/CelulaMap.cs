using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPacoteServicoMap
{
    public class CelulaMap : IEntityTypeConfiguration<Celula>
    {
        public void Configure(EntityTypeBuilder<Celula> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLCELULA");

            builder.Property(e => e.Id).HasColumnName("IDCELULA");

            builder.Property(e => e.DescCelula)
                .IsRequired()
                .HasColumnName("DESCCELULA")
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTULTIMAALTERACAO")
                .IsRequired(false);

            builder.Property(e => e.FlHabilitarRepasseMesmaCelula)
             .HasColumnName("FLHABILITARREPASSEMESMACELULA");

            builder.Property(e => e.FlHabilitarRepasseEPM)
                .HasColumnName("FLHABILITARREPASSEEPM");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsRequired(true);

            builder.Property(e => e.IdCelulaPai).HasColumnName("IDCELULAPAI");

            builder.HasOne(d => d.CelulaPai)
                .WithMany(p => p.CelulasSubordinadas)
                .HasForeignKey(d => d.IdCelulaPai)
                .HasConstraintName("FK_CELULA");
        }
    }
}
