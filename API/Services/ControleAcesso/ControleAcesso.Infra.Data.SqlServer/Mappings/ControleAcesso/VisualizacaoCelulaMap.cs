using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class VisualizacaoCelulaMap : IEntityTypeConfiguration<VisualizacaoCelula>
    {
        public void Configure(EntityTypeBuilder<VisualizacaoCelula> builder)
        {
            builder.ToTable("TBLVISUALIZACAOCELULA");

            builder.HasKey(p => new { p.Id });

            builder.Property(p => p.Id)
                .HasColumnName("idVisualizacaoCelula")
                .IsRequired();

            builder.Property(p => p.IdCelula)
                .HasColumnName("idCelula")
                .IsRequired(false);

            builder.Ignore(x => x.TodasAsCelulasSempre);
            builder.Ignore(x => x.IdCelulaUsuarioVinculado);
            
            builder.Property(x => x.TodasAsCelulasSempreMenosAPropria)     
                 .HasColumnName("FLBLOQUEARPROPRIACELULA");

            builder.Property(x => x.LgUsuario)
               .HasMaxLength(30)
               .IsRequired(true)
               .IsUnicode(false)
               .HasColumnName("LGUSUARIOVINCULADO");

            builder.Property(x => x.DataAlteracao)
                 .HasColumnName("DTALTERACAO")
                 .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(d => d.Celula)
                .WithMany(p => p.VisualizacoesCelula)
                .HasForeignKey(d => d.IdCelula)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
