using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class TransferenciaPrestadorMap : IEntityTypeConfiguration<TransferenciaPrestador>
    {
        public void Configure(EntityTypeBuilder<TransferenciaPrestador> builder)
        {
            builder.ToTable("TBLTRANSFERENCIAPRESTADOR");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IDTRANSFERENCIAPRESTADOR");

            builder.Property(x => x.IdPrestador)
                .HasColumnName("IDPRESTADOR")
                .IsRequired();

            builder.Property(x => x.Situacao)
                .HasColumnName("SITUACAO")
                .IsRequired();

            builder.Property(x => x.DataTransferencia)
                .HasColumnName("DTREFERENCIA")
                .IsRequired();

            builder.Property(x => x.IdEmpresaGrupo)
                .HasColumnName("CDCOLIGADA")
                .IsRequired(false);

            builder.Property(x => x.IdFilial)
                .HasColumnName("CDFILIAL")
                .IsRequired(false);

            builder.Property(x => x.IdCelula)
                .HasColumnName("IDCELULA")
                .IsRequired(false);

            builder.Property(x => x.IdCliente)
                .HasColumnName("CDCLIENTE")
                .IsRequired(false);

            builder.Property(x => x.IdServico)
                .HasColumnName("CDSERVICO")
                .IsRequired(false);

            builder.Property(x => x.IdLocalTrabalho)
                .HasColumnName("CDLOCALTRABALHO")
                .IsRequired(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .HasMaxLength(30)
                .IsRequired();

            builder.HasOne(x => x.Prestador)
                .WithMany(x => x.TransferenciasPrestador)
                .HasForeignKey(x => x.IdPrestador);
        }
    }
}
