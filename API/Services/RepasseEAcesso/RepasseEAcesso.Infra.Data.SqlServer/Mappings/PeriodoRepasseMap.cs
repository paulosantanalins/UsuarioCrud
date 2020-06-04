using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class PeriodoRepasseMap : IEntityTypeConfiguration<PeriodoRepasse>
    {
        public void Configure(EntityTypeBuilder<PeriodoRepasse> builder)
        {
            builder.ToTable("TBLPERIODOREPASSE");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("IDPERIODOREPASSE");
            
            builder.Property(p => p.DtLancamentoInicio)
                .HasColumnName("DTLANCAMENTOINICIO");

            builder.Property(p => p.DtLancamentoFim)
                .HasColumnName("DTLANCAMENTOFIM");

            builder.Property(p => p.DtAnaliseInicio)
                .HasColumnName("DTANALISEINICIO");

            builder.Property(p => p.DtAnaliseFim)
                .HasColumnName("DTANALISEFIM");

            builder.Property(p => p.DtAprovacaoInicio)
                .HasColumnName("DTAPROVACAOINICIO");

            builder.Property(p => p.DtAprovacaoFim)
                .HasColumnName("DTAPROVACAOFIM");

            builder.Property(p => p.DtLancamento)
                .HasColumnName("DTLANCAMENTO");

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
