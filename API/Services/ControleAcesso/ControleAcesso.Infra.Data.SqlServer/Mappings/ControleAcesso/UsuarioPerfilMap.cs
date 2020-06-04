using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class UsuarioPerfilMap : IEntityTypeConfiguration<UsuarioPerfil>
    {
        public void Configure(EntityTypeBuilder<UsuarioPerfil> builder)
        {
            builder.ToTable("tblUsuarioPerfil");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idUsuarioPerfil")
                .IsRequired();

            builder.Property(p => p.IdPerfil)
                .HasColumnName("idPerfil")
                .IsRequired();

            builder.Property(p => p.LgUsuario)
                .HasColumnName("lgUsuarioLogado")
                .HasColumnType("varchar(30)")
                .IsRequired();

            builder.HasOne(d => d.Perfil)
                .WithMany(p => p.UsuarioPerfis)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull);

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
