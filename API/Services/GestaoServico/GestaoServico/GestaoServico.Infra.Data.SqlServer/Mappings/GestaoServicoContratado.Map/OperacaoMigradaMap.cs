using GestaoServico.Domain.OperacaoMigradaRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map
{
    class OperacaoMigradaMap : IEntityTypeConfiguration<OperacaoMigrada>
    {
        public void Configure(EntityTypeBuilder<OperacaoMigrada> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLOPERACAO");

            builder.Property(e => e.Id).HasColumnName("IDOPERACAO");

            builder.Property(e => e.IdCombinadaCelula)
                .HasColumnName("IdCombinadaCelula");

            builder.Property(e => e.IdGrupoDelivery)
                .HasColumnName("IdGrupoDelivery");

            builder.Property(e => e.DescricaoOperacao)
                .HasColumnName("DescOperacao");

            builder.Property(e => e.IdCelula)
                .HasColumnName("IdCelula");

            builder.Property(e => e.IdServico)
                .HasColumnName("IdServico");

            builder.Property(e => e.IdCliente)
                .HasColumnName("IdCliente");

            builder.Property(e => e.IdTipoCelula)
                .HasColumnName("IdTipoCelula");

            builder.Property(e => e.NomeCliente)
                .HasColumnName("NMCLIENTE");

            builder.Property(e => e.DescricaoServico)
                .HasColumnName("DESCSERVICO");

            builder.Property(e => e.Status)
                .HasColumnName("FLSTATUS");

            builder.Property(e => e.Ativo)
                .HasColumnName("FLATIVO");

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
