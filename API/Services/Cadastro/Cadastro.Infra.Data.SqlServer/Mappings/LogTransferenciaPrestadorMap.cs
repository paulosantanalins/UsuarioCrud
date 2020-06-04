using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class LogTransferenciaPrestadorMap : IEntityTypeConfiguration<LogTransferenciaPrestador>
    {
        public void Configure(EntityTypeBuilder<LogTransferenciaPrestador> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLLOGTRANSFERENCIAPRESTADOR");

            builder.Property(e => e.Id)
                .HasColumnName("IDLOGTRANSFERENCIAPRESTADOR");

            builder.Property(e => e.MotivoNegacao)
                .HasColumnName("DESCMOTIVONEGACAO");

            builder.Property(e => e.Status)
                .HasColumnName("STATUS");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO");

            builder.Property(e => e.DataAlteracao)            
                .HasColumnName("DTALTERACAO");

            builder.Property(e => e.IdTransferenciaPrestador)
                .HasColumnName("IDTRANSFERENCIAPRESTADOR");

            builder.HasOne(x => x.Transferencia)
                .WithMany(x => x.LogsTransferenciaPrestador)
                .HasForeignKey(x => x.IdTransferenciaPrestador);
        }
    }
}
