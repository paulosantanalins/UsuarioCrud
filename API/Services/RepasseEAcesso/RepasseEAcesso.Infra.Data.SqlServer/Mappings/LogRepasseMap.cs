using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class LogRepasseMap : IEntityTypeConfiguration<LogRepasse>
    {
        public void Configure(EntityTypeBuilder<LogRepasse> builder)
        {
            builder.ToTable("TBLLOGREPASSE");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDLOGREPASSE");

            builder.Property(p => p.IdStatusRepasse)
           .HasColumnName("IDSTATUS");

            builder.Property(p => p.IdRepasse)
           .HasColumnName("IDREPASSENIVELUM");

            builder.Property(p => p.IdMotivoRepasse)
                .HasColumnName("IDMOTIVO");

            builder.Property(p => p.Descricao)
                .HasColumnName("DESCMOTIVO")
                 .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("DTALTERACAO");

            builder.Property(p => p.Usuario)
                .HasColumnName("LGUSUARIO")
                 .HasMaxLength(30);

            builder.HasOne(x => x.Repasse)
               .WithMany(x => x.LogsRepasse)
               .HasForeignKey(x => x.IdRepasse);

            builder.HasOne(x => x.StatusRepasse)
                .WithMany(d => d.LogsRepasse)
                .HasForeignKey(x => x.IdStatusRepasse);
        }
    }
}
