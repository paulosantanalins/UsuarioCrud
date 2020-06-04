using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class RepasseNivelUmMap : IEntityTypeConfiguration<RepasseNivelUm>
    {
        public void Configure(EntityTypeBuilder<RepasseNivelUm> builder)
        {
            builder.ToTable("TBLREPASSENIVELUM");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("IDREPASSENIVELUM");

            builder.Property(p => p.IdCelulaOrigem)
                .HasColumnName("IDCELULAORIGEM");

            builder.Property(p => p.IdClienteOrigem)
                .HasColumnName("IDCLIENTEORIGEM");

            builder.Property(p => p.NomeClienteOrigem)
               .HasColumnName("NMCLIENTEORIGEM");

            builder.Property(p => p.IdServicoOrigem)
                .HasColumnName("IDSERVICOORIGEM");

            builder.Property(p => p.NomeServicoOrigem)
                .HasColumnName("NMSERVICOORIGEM");

            builder.Property(p => p.IdCelulaDestino)
                .HasColumnName("IDCELULADESTINO");

            builder.Property(p => p.IdClienteDestino)
                .HasColumnName("IDCLIENTEDESTINO");

            builder.Property(p => p.NomeClienteDestino)
                .HasColumnName("NMCLIENTEDESTINO");

            builder.Property(p => p.IdServicoDestino)
                .HasColumnName("IDSERVICODESTINO");

            builder.Property(p => p.NomeServicoDestino)
                .HasColumnName("NMSERVICODESTINO");

            builder.Property(p => p.DataRepasse)
                .HasColumnName("DTREPASSE");

            builder.Property(p => p.IdProfissional)
                .HasColumnName("IDPROFISSIONAL")
                .IsRequired(false);

            builder.Property(p => p.NomeProfissional)
                .HasColumnName("NMPROFISSIONAL");

            builder.Property(p => p.ValorCustoProfissional)
                .HasColumnName("VLCUSTOPROFISSIONAL")
                .IsRequired(false);

            builder.Property(p => p.ValorUnitario)
                .HasColumnName("VLUNITARIO")
                .IsRequired(false);

            builder.Property(p => p.QuantidadeItens)
                .HasColumnName("QTDITENS")
                .IsRequired(false);

            builder.Property(p => p.ValorTotal)
                .HasColumnName("VLTOTAL")
                .IsRequired(false);

            builder.Property(p => p.IdMoeda)
                .HasColumnName("IDMOEDA");

            builder.Property(p => p.DescricaoProjeto)
                .HasColumnName("DESCPROJETO")
                .IsRequired(false);

            builder.Property(p => p.DataLancamento)
             .HasColumnName("DT_LANCAMENTO");             

            builder.Property(p => p.MotivoNegacao)
                .HasColumnName("DESCMOTIVONEGACAO")
                .IsRequired(false);

            builder.Property(p => p.Justificativa)
                .HasColumnName("DESCJUSTIFICATIVA")
                .IsRequired(false);

            builder.Property(p => p.Status)
                .HasColumnName("NMSTATUS");

            builder.Property(p => p.IdRepasseMae)
                .HasColumnName("IDREPASSEMAE")
                .IsRequired(false);

            builder.Property(p => p.IdRepasseMaeEAcesso)
                .HasColumnName("IDREPASSEMAELEGADO")
                .IsRequired(false);

            builder.Property(p => p.DataRepasseMae)
                .HasColumnName("DTREPASSEMAE")
                .IsRequired(false);

            builder.Property(p => p.IdEpm)
                .HasColumnName("IDEPM")
                .IsRequired(false);
            builder.Property(p => p.IdRepasseEacesso)
                .HasColumnName("IDREPASSELEGADO")
                .IsRequired(false);
            builder.Property(p => p.IdOrigem)
                .HasColumnName("IDORIGEM")
                .IsRequired(false);

            builder.Property(p => p.RepasseInterno)
                .HasColumnName("FLREPASSEINTERNO");

            builder.Property(p => p.ParcelaRepasse)
                .HasColumnName("PARCELAREPASSE");

            builder.Property(x => x.DataAlteracao)
                      .HasColumnName("DTALTERACAO")
                      .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(x => x.CelulaOrigem)
                .WithMany(x => x.RepassesOrigem)
                .HasForeignKey(x => x.IdCelulaOrigem);

            builder.HasOne(x => x.CelulaDestino)
                .WithMany(x => x.RepassesDestino)
                .HasForeignKey(x => x.IdCelulaDestino);

            builder.HasOne(x => x.Moeda)
                .WithMany(x => x.RepassesNivelUm)
                .HasForeignKey(x => x.IdMoeda);

            builder.HasMany(x => x.LogsRepasse)
                .WithOne(x => x.Repasse);
        }
    }
}
