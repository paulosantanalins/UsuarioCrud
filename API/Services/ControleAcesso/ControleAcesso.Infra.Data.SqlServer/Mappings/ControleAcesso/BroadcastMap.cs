using ControleAcesso.Domain.BroadcastRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class BroadcastMap : IEntityTypeConfiguration<Broadcast>
    {

        public void Configure(EntityTypeBuilder<Broadcast> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLBROADCAST");

            builder.Property(e => e.Id)
                .HasColumnName("IDBROADCAST");

            builder.Property(e => e.Excluido)
                .HasColumnName("FLEXCLUIDO")
                .IsRequired(true);

            builder.Property(e => e.Lido)
                .HasColumnName("FLLIDO")
                 .IsRequired(true); ;

            builder.Property(e => e.LgUsuarioVinculado)
                .HasColumnName("LGUSUARIOVINCULADO")
                .IsRequired(true);

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsRequired(false);

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(e => e.IdBroadcastItem)
                .HasColumnName("IDBROADCASTITEM")
                .IsRequired(true);

            builder.Property(e => e.DataCriacao)
                .HasColumnName("DTCRIACAO")
                .IsRequired();

            builder.HasOne(x => x.BroadcastItem)
                   .WithMany(x => x.Broadcasts)
                   .HasForeignKey(x => x.IdBroadcastItem);
        }
    }
}
