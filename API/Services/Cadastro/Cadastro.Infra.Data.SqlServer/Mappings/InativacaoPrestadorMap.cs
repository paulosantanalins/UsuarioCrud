using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class InativacaoPrestadorMap : IEntityTypeConfiguration<InativacaoPrestador>
    {
        public void Configure(EntityTypeBuilder<InativacaoPrestador> builder)
        {
            builder.ToTable("TBLINATIVACAOPRESTADOR");

            builder.Property(e => e.Id)
               .HasColumnName("IDINATIVACAOPRESTADOR");

            builder.Property(e => e.IdPrestador)
                .HasColumnName("IDPRESTADOR");

            builder.Property(e => e.CodEacessoLegado)
                .HasColumnName("CDEACESSOLEGADO");

            builder.Property(e => e.FlagIniciativaDesligamento)
                .HasColumnName("FLINICIATIVADESLIGAMENTOEMPRESA");

            builder.Property(e => e.FlagRetorno)
             .HasColumnName("FLRETORNOPERMITIDO");

            builder.Property(e => e.DataDesligamento)
                .HasColumnName("DTDESLIGAMENTO");

            builder.Property(e => e.Motivo)
             .HasColumnName("MOTIVO");

            builder.Property(x => x.Responsavel)
              .HasColumnName("NMRESPONSAVEL");

            builder.Property(x => x.DataAlteracao)
               .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
               .HasColumnName("LGUSUARIO")
               .IsUnicode(false);

            builder.HasOne(e => e.Prestador)
               .WithMany(p => p.InativacoesPrestador)
               .HasForeignKey(e => e.IdPrestador);
        }
    }
}
