using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ValorPrestadorMap : IEntityTypeConfiguration<ValorPrestador>
    {
        public void Configure(EntityTypeBuilder<ValorPrestador> builder)
        {
            builder.ToTable("TBLVALORPRESTADOR");
            builder.HasKey(e => e.Id);


            builder.Property(e => e.Id)
                .HasColumnName("IDVALORPRESTADOR");

            builder.Property(e => e.IdPrestador)
                .HasColumnName("IDPRESTADOR");

            builder.Property(e => e.ValorHora)
                .HasColumnName("VLHORA")
                .IsUnicode(false);

            builder.Property(e => e.ValorMes)
                .HasColumnName("VLMES")
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.DataReferencia)
                .HasColumnName("DTREFERENCIA");

            builder.Property(e => e.IdTipoRemuneracao)
               .HasColumnName("IDTIPONUMERACAO");

            builder.Property(e => e.IdMoeda)
              .HasColumnName("IDMOEDA");

            builder.Property(e => e.Quantidade)
              .HasColumnName("QTDVALORPRESTADOR");

            builder.Property(e => e.ValorClt)
              .HasColumnName("VLCLT");

            builder.Property(e => e.ValorPericulosidade)
              .HasColumnName("VLPERICULOSIDADE");

            builder.Property(e => e.ValorPropriedadeIntelectual)
              .HasColumnName("VLPROPINTELECTUAL");

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(e => e.Prestador)
                .WithMany(p => p.ValoresPrestador)
                .HasForeignKey(e => e.IdPrestador);

            builder.HasOne(e => e.TipoRemuneracao)
                .WithMany(p => p.ValoresPrestador)
                .HasForeignKey(d => d.IdTipoRemuneracao);
        }
    }
}
