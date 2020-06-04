using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class TransferenciaCltPjMap : IEntityTypeConfiguration<TransferenciaCltPj>
    {
        public void Configure(EntityTypeBuilder<TransferenciaCltPj> builder)
        {
            builder.ToTable("TBLTRANSFERENCIACLTPJ");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDTRANSFERENCIACLTPJ");

            builder.Property(e => e.IdEacessoLegado)
                .HasColumnName("CDEACESSOLEGADO");

            builder.Property(e => e.IdPrestadorTransferido)
                .HasColumnName("IDPRESTADORTRANSFERIDO");

            builder.Property(e => e.NomePrestador)
                .HasColumnName("NMPRESTADORTRANSFERIDO");

            builder.Property(e => e.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(p => p.PrestadorTransferido)
                .WithMany(p => p.TransferenciasCltPj)
                .HasForeignKey(p => p.IdPrestadorTransferido);
        }
    }
}
