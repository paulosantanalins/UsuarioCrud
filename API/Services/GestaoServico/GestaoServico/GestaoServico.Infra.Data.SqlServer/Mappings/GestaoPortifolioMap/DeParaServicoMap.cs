using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class DeParaServicoMap : IEntityTypeConfiguration<DeParaServico>
    {
        public void Configure(EntityTypeBuilder<DeParaServico> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("tblDeParaServico");

            builder.Property(e => e.Id).HasColumnName("idDeParaServico");

            builder.Property(e => e.DescStatus)
             .IsRequired()
             .HasColumnName("DescStatus");

            builder.Property(e => e.IdServicoEacesso).HasColumnName("idServicoEacesso");

            builder.Property(e => e.IdServicoContratado).HasColumnName("IdServicoContratado"); 

            builder.Property(e => e.DescTipoServico).HasColumnName("DescTipoServico");

            builder.Property(e => e.NmServicoEacesso).HasColumnName("NmServicoEacesso");

            builder.HasOne(d => d.ServicoContratado)
                .WithMany(p => p.DeParaServicos)
                .HasForeignKey(d => d.IdServicoContratado)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Ignore(x => x.DataAlteracao);

            builder.Ignore(x => x.Usuario);
        }
    }
}
