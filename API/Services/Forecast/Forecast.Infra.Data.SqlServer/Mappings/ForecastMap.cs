using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Forecast.Domain.ForecastRoot;

namespace Forecast.Infra.Data.SqlServer.Mappings
{
    public class ForecastMap : IEntityTypeConfiguration<ForecastET>
    {
        public void Configure(EntityTypeBuilder<ForecastET> builder)
        {
            builder.ToTable("TBLFORECAST");

            builder.HasKey(e => new { e.IdCelula, e.IdCliente, e.IdServico, e.NrAno });

            builder.Property(e => e.IdCelula)
               .HasColumnName("IDCELULA");

            builder.Property(e => e.IdCliente)
              .HasColumnName("IDCLIENTE");

            builder.Property(e => e.IdServico)
              .HasColumnName("IDSERVICO");

            builder.Property(e => e.NrAno)
              .HasColumnName("NRANO");

            builder.Property(e => e.DataAniversario)
                .IsRequired(false)
              .HasColumnName("DTANIVERSARIO");

            builder.Property(e => e.DataAplicacaoReajuste)
                .IsRequired(false)
              .HasColumnName("DTAPLICACAOREAJUSTE");

            builder.Property(e => e.DataReajusteRetroativo)
                .IsRequired(false)
              .HasColumnName("DTREAJUSTERETROATIVO");

            builder.Property(e => e.FaturamentoNaoRecorrente)
                    .IsRequired(true)
              .HasColumnName("FLFATURAMENTONAORECORRENTE");

            builder.Property(e => e.IdStatus)
                .IsRequired(false)
              .HasColumnName("IDSTATUS");

            builder.Property(e => e.DescricaoJustificativa)
              .HasColumnName("DSJUSTIFICATIVA")
              .HasMaxLength(250)
              .IsRequired(false);

            builder.Property(e => e.DataAlteracao)
                 .HasColumnName("DTALTERACAO");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);
        }
    }
}
