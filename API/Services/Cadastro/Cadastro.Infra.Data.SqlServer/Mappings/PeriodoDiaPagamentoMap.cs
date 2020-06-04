using Cadastro.Domain.HorasMesRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class PeriodoDiaPagamentoMap : IEntityTypeConfiguration<PeriodoDiaPagamento>
    {
        public void Configure(EntityTypeBuilder<PeriodoDiaPagamento> builder)
        {
            builder.ToTable("TBLPERIODODIAPAGAMENTO");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDPERIODODIAPAGAMENTO");

            builder.Property(p => p.IdPeriodo)
                .HasColumnName("IDHORASMES");

            builder.Property(p => p.IdDiaPagamento)
                .HasColumnName("IDDIAPAGAMENTO");

            builder.Property(p => p.DiaLimiteLancamentoHoras)
                .HasColumnName("DTLIMITELANCAMENTOHORAS");

            builder.Property(p => p.DiaLimiteAprovacaoHoras)
                .HasColumnName("DTLIMITEAPROVACAOHORAS");

            builder.Property(p => p.DiaLimiteEnvioNF)
                .HasColumnName("DTLIMITEENVIONF");

            builder.Property(x => x.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(x => x.Periodo)
                .WithMany(x => x.PeriodosDiaPagamento)
                .HasForeignKey(x => x.IdPeriodo);

            builder.HasOne(x => x.DiaPagamento)
                .WithMany(x => x.PeriodosDiaPagamento)
                .HasForeignKey(x => x.IdDiaPagamento);
        }
    }
}
