using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Forecast.Domain.ForecastRoot;

namespace Forecast.Infra.Data.SqlServer.Mappings
{
    public class ValorForecastMap : IEntityTypeConfiguration<ValorForecast>
    {
        public void Configure(EntityTypeBuilder<ValorForecast> builder)
        {
            //builder.ToTable("TBLFORECAST");
            builder.ToTable("TBLVALORFORECAST");

            builder.HasKey(e => new { e.IdCelula, e.IdCliente, e.IdServico, e.NrAno});           

            builder.Property(e => e.IdCelula)
               .HasColumnName("IDCELULA");

            builder.Property(e => e.IdCliente)
              .HasColumnName("IDCLIENTE");

            builder.Property(e => e.IdServico)
              .HasColumnName("IDSERVICO");

            builder.Property(e => e.NrAno)
              .HasColumnName("NRANO");
            
            builder.Property(e => e.VlPercentual)
                .IsRequired(false)
              .HasColumnName("VLPERCENTUAL");
                       
            builder.Property(e => e.ValorJaneiro)
              .HasColumnName("VLJANEIRO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorFevereiro)
              .HasColumnName("VLFEVEREIRO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorMarco)
              .HasColumnName("VLMARCO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorAbril)
              .HasColumnName("VLABRIL")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorMaio)
              .HasColumnName("VLMAIO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorJunho)
              .HasColumnName("VLJUNHO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorJulho)
              .HasColumnName("VLJULHO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorAgosto)
              .HasColumnName("VLAGOSTO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorSetembro)
              .HasColumnName("VLSETEMBRO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorOutubro)
              .HasColumnName("VLOUTUBRO")
              .IsRequired(false)
              .IsUnicode(false);

            builder.Property(e => e.ValorNovembro)
             .HasColumnName("VLNOVEMBRO")
             .IsRequired(false)
             .IsUnicode(false);

            builder.Property(e => e.ValorDezembro)
             .HasColumnName("VLDEZEMBRO")
             .IsRequired(false)
             .IsUnicode(false);

            builder.Property(e => e.ValorAjuste)
             .HasColumnName("VLAJUSTE")
             .IsRequired(false)
             .IsUnicode(false);

            
            builder.Property(x => x.DataAlteracao)
                 .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);
        }
    }
}
