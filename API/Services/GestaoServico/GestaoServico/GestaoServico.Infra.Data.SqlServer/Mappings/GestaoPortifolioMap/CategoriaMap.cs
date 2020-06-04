using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class CategoriaMap : IEntityTypeConfiguration<CategoriaContabil>
    {
        public void Configure(EntityTypeBuilder<CategoriaContabil> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLCATEGORIACONTABIL");

            builder.Property(e => e.Id).HasColumnName("IDCATEGORIACONTABIL");

            builder.Property(e => e.DescCategoria)
                    .IsRequired()
                    .HasColumnName("DESCCATEGORIACONTABIL")
                    .HasMaxLength(150)
                    .IsUnicode(false);

            builder.Property(e => e.SgCategoriaContabil)
                .IsRequired(false)
                .HasColumnName("SGCATEGORIACONTABIL")
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(e => e.FlStatus)
                .IsRequired()
                .HasColumnName("FLSTATUS");
            
            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");
        }
    }
}
