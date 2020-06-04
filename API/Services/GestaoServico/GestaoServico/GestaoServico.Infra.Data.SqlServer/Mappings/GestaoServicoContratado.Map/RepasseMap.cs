using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPacoteServicoMap
{
    public class RepasseMap : IEntityTypeConfiguration<Repasse>
    {
        public void Configure(EntityTypeBuilder<Repasse> builder)
        {
            builder.HasKey(e => new { e.Id, e.DtRepasse });

            builder.ToTable("TBLREPASSE");

            builder.Property(e => e.Id)
                .HasColumnName("IDREPASSE")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IdRepasseMae)
                .HasColumnName("IDREPASSEMAE");

            builder.Property(e => e.IdProfissional)
                .HasColumnName("IDPROFESSIONAL");

            builder.Property(e => e.DtRepasse)
                .HasColumnName("DTREPASSE")
                .HasColumnType("smalldatetime");

            builder.Property(e => e.DescProjeto)
                .HasColumnName("DESCPROJETO")
                .IsUnicode(false);

            builder.Property(e => e.DescMotivoNegacao)
                .HasColumnName("DESCMOTIVONEGACAO")
                .IsUnicode(false);

            builder.Property(e => e.DescJustificativa)
                .HasColumnName("DESCJUSTIFICATIVA")
                .IsUnicode(false);

            builder.Property(e => e.QtdRepasse)
                .HasColumnName("QTDITENS");

            builder.Property(e => e.VlUnitario)
                .HasColumnName("VLUNITARIO")
                .HasColumnType("decimal(11, 2)");

            builder.Property(e => e.VlTotal)
                .HasColumnName("VLTOTAL")
                .HasColumnType("decimal(11, 2)");

            builder.Property(e => e.VlCustoProfissional)
                .HasColumnName("VLCUSTOPROFISSIONAL")
                .HasColumnType("decimal(11, 2)");

            builder.Property(e => e.FlStatus)
                .HasColumnName("FLSTATUS")
                .HasColumnType("char(2)");

            builder.Property(e => e.DtRepasseMae)
                .HasColumnName("DTREPASSEMAE")
                .HasColumnType("smalldatetime");

            builder.Property(e => e.IdEpm)
                .HasColumnName("IDEPM");

            builder.Property(e => e.IdMoeda)
                .HasColumnName("IDMOEDA")
                .IsRequired();

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.Property(e => e.NrParcela)
                .HasColumnName("NRPARCELA");

            builder.Property(e => e.IdServicoContratadoOrigem)
                .HasColumnName("IDSERVICOCONTRATADOORIGEM");

            builder.Property(e => e.IdServicoContratadoDestino)
                .HasColumnName("IDSERVICOCONTRATADODESTINO");

            builder.Property(e => e.IdRepasseEacesso)
                .HasColumnName("IDREPASSELEGADO");

            builder.Property(e => e.FlRepasseInterno)
                .HasColumnName("FLREPASSEINTERNO");

            builder.Property(e => e.IdPessoaAlteracao)
                .HasColumnName("IDPESSOAALTERACAO");

            builder.HasOne(d => d.ServicoContratadoDestino)
                .WithMany(p => p.RepasseDestinos)
                .HasForeignKey(d => d.IdServicoContratadoDestino);

            builder.HasOne(d => d.ServicoContratadoOrigem)
                .WithMany(p => p.RepasseOrigens)
                .HasForeignKey(d => d.IdServicoContratadoOrigem);

            //builder.HasOne(d => d.TipoDespesa)
            //    .WithMany(p => p.Repasses)
            //    .HasForeignKey(d => d.IdTipoDespesa);

            builder.HasOne(d => d.RepasseMae)
                .WithMany(p => p.RepasseFilhos)
                .HasForeignKey(d => new { d.IdRepasseMae, d.DtRepasseMae });

        }
    }
}
