using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPacoteServicoMap
{
    public class TipoDespesaMap : IEntityTypeConfiguration<TipoDespesa>
    {
        public void Configure(EntityTypeBuilder<TipoDespesa> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLTIPODESPESA");

            builder.Property(e => e.Id).HasColumnName("IDTIPODESPESA");

            builder.Property(e => e.DescTipoDespesa)
                .IsRequired()
                .HasColumnName("DESCTIPODESPESA")
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.SgTipoDespesa)
                .IsRequired()
                .HasColumnName("SGTIPODESPESA")
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
    }
}
