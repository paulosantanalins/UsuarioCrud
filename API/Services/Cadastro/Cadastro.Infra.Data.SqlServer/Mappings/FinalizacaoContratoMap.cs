using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class FinalizacaoContratoMap : IEntityTypeConfiguration<FinalizacaoContrato>
    {
        public void Configure(EntityTypeBuilder<FinalizacaoContrato> builder)
        {
            builder.ToTable("TBLFINALIZACAOCONTRATO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IDFINALIZACAOCONTRATO");

            builder.Property(x => x.IdPrestador)
                .HasColumnName("IDPRESTADOR")
                .IsRequired();

            builder.Property(x => x.DataInclusao)
                .HasColumnName("DTINCLUSAO")
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

            builder.Property(x => x.Situacao)
                .HasColumnName("SITUACAO")
                .IsRequired();

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasOne(x => x.Prestador)
                .WithMany(x => x.FinalizacoesContratos)
                .HasForeignKey(x => x.IdPrestador);
        }
    }
}
