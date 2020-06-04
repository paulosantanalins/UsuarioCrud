using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class LogFinalizacaoContratoMap : IEntityTypeConfiguration<LogFinalizacaoContrato>
    {
        public void Configure(EntityTypeBuilder<LogFinalizacaoContrato> builder)
        {
            builder.ToTable("TBLLOGFINALIZACAOCONTRATO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IDLOGFINALIZACAOCONTRATO");

            builder.Property(x => x.IdFinalizacaoContrato)
                .HasColumnName("IDFINALIZACAOCONTRATO")
                .IsRequired();

            builder.Property(x => x.DataFimContrato)
                .HasColumnName("DTFIMCONTRATO")
                .IsRequired();

            builder.Property(x => x.RetornoPermitido)
                .HasColumnName("FLRETORNOPERMITIDO")
                .IsRequired();

            builder.Property(x => x.Motivo)
                .HasColumnName("MOTIVO")
                .IsRequired(false);

            builder.Property(x => x.MotivoCancelamento)
                .HasColumnName("MOTIVOCANCELAMENTO")
                .IsRequired(false);

            builder.Property(x => x.Situacao)
                .HasColumnName("SITUACAO")
                .IsRequired();

            builder.Property(x => x.Acao)
                .HasColumnName("ACAO")
                .IsRequired();

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasOne(x => x.FinalizacaoContrato)
                .WithMany(x => x.LogsFinalizacaoCntrato)
                .HasForeignKey(x => x.IdFinalizacaoContrato);
        }
    }
}
