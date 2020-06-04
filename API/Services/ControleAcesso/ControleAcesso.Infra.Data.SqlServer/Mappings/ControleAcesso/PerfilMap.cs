using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class PerfilMap : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.ToTable("tblPerfil");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idPerfil")
                .IsRequired();

            builder.Property(p => p.NmPerfil)
                .HasColumnName("nmPerfil")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.NmModulo)
                .HasColumnName("nmModulo")
                .HasColumnType("varchar(60)")
                .IsRequired();

            builder.Property(p => p.FlAtivo)
                .HasColumnName("flAtivo")
                .IsRequired();

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
