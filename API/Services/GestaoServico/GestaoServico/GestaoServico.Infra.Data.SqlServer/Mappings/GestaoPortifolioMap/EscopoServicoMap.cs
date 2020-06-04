using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class EscopoServicoMap : IEntityTypeConfiguration<EscopoServico>
    {
        public void Configure(EntityTypeBuilder<EscopoServico> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLESCOPOSERVICO");

            builder.Property(e => e.Id).HasColumnName("IDESCOPOSERVICO");

            builder.Property(e => e.NmEscopoServico)
                    .IsRequired()
                    .HasColumnName("NMESCOPOSERVICO")
                    .HasMaxLength(100)
                    .IsUnicode(false);

            builder.Property(e => e.FlAtivo)
                .IsRequired()
                .HasColumnName("FLATIVO");

            builder.Property(e => e.IdPortfolioServico)
                .HasColumnName("IDPORTFOLIOSERVICO")
                .IsRequired();

            builder.HasOne(d => d.PortfolioServico)
                .WithMany(p => p.EscopoServicos)
                .HasForeignKey(d => d.IdPortfolioServico)
                .OnDelete(DeleteBehavior.ClientSetNull);

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
