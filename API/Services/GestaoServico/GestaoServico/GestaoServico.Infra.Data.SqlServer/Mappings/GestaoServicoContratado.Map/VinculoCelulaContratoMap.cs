using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoServicoContratado.Map
{
    public class VinculoCelulaContratoMap : IEntityTypeConfiguration<VinculoCelulaContrato>
    {
        public void Configure(EntityTypeBuilder<VinculoCelulaContrato> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLVINCULOCELULACONTRATO");

            builder.Property(e => e.Id).HasColumnName("IDVINCULOCELULACONTRATO");

            builder.Property(e => e.DescTipoCelula)
                .IsRequired()
                .HasColumnName("DESCTIPOCELULA")
                .HasColumnType("char(3)");

            builder.Property(e => e.DtFim)
                .HasColumnName("DTFIMVIGENCIA")
                .HasColumnType("smalldatetime");

            builder.Property(e => e.DtInicio)
                .HasColumnName("DTINICIOVIGENCIA")
                .HasColumnType("smalldatetime");

            builder.Property(e => e.IdCelula).HasColumnName("IDCELULA");

            builder.Property(e => e.IdContrato).HasColumnName("IDCONTRATO");

            builder.Property(e => e.IdServicoContratado).HasColumnName("IDSERVICOCONTRATADO");

            builder.Property(e => e.IdEscopoServico).HasColumnName("IDESCOPOSERVICO");

            builder.Property(e => e.IdEscopoServico).HasColumnName("IDESCOPOSERVICO");

            //builder.HasOne(d => d.Contrato)
            //    .WithMany(p => p.VinculoCelulaContratos)
            //    .HasForeignKey(d => d.IdContrato)
            //    .HasConstraintName("FK_CONTRATO_VINCULOCELULACONTRATO");

            //builder.HasOne(d => d.ServicoContratado)
            //    .WithMany(p => p.VinculoCelulaContratos)
            //    .HasForeignKey(d => d.IdServicoContratado)
            //    .HasConstraintName("FK_SERVICOCONTRATADO_VINCULOCELULACONTRATO");

            //builder.HasOne(p => p.EscopoServico)
            //    .WithMany(p => p.VinculoCelulaContratos)
            //    .HasForeignKey(p => p.IdEscopoServico);

            //builder.Property(x => x.DataAlteracao)
            //    .HasColumnName("DTALTERACAO")
            //    .IsRequired(false);

            //builder.Property(x => x.Usuario)
            //    .HasMaxLength(30)
            //    .IsRequired(true)
            //    .IsUnicode(false)
            //    .HasColumnName("LGUSUARIO");

            builder.Ignore(x => x.DataAlteracao);
            builder.Ignore(x => x.Usuario);
        }
    }
}
