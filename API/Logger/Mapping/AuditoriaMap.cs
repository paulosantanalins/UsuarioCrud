using Logger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Mapping
{
    public class AuditoriaMap : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLAUDITORIA");

            builder.Property(e => e.Id)
                .HasColumnName("IDAUDITORIA");

            builder.Property(e => e.Tabela)
                .IsRequired()
                .HasColumnName("NMTABELA")
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.Property(e => e.DataAlteracao)
                .IsRequired()
                .HasColumnName("DTHORAAUDITORIA");

            builder.Property(e => e.ValoresAntigos)
                .HasColumnName("VLANTERIORCAMPO")
                .IsUnicode(false);

            builder.Property(e => e.ValoresNovos)
                .HasColumnName("VLNOVOCAMPO")
                .IsUnicode(false);

            builder.Property(e => e.IdsAlterados)
                .IsRequired()
                .HasColumnName("IDPKREGISTROALTERADO")
                .IsUnicode(false);
        }
    }
}
