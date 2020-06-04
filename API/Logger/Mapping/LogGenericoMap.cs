using Logger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Mapping
{
    public class LogGenericoMap : IEntityTypeConfiguration<LogGenerico>
    {
        public void Configure(EntityTypeBuilder<LogGenerico> builder)
        {
            builder.ToTable("tblLogGenerico");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idLogGenerico")
                .IsRequired(true);

            builder.Property(p => p.NmTipoLog)
                .HasColumnName("nmTipoLog")
                .IsRequired(true);

            builder.Property(p => p.NmOrigem)
                .HasColumnName("nmOrigem")
                .IsRequired(true);

            builder.Property(p => p.DescLogGenerico)
                .HasColumnName("descLogGenerico")
                .IsRequired(true);

            builder.Property(p => p.DtHoraLogGenerico)
                .HasColumnName("dtHoraLogGenerico")
                .HasColumnType("datetime")
                .IsRequired(true);

            builder.Property(p => p.DescExcecao)
                .HasColumnName("descExcecao")
                .IsRequired(true);
        }
    }
}
