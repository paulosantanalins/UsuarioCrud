using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class VinculoPerfilFuncionalidadeMap : IEntityTypeConfiguration<VinculoPerfilFuncionalidade>
    {
        public void Configure(EntityTypeBuilder<VinculoPerfilFuncionalidade> builder)
        {
            builder.ToTable("tblVinculoPerfilFuncionalidade");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idVinculoPerfilFuncionalidade")
                .IsRequired();

            builder.Property(p => p.IdPerfil)
                .HasColumnName("idPerfil")
                .IsRequired();

            builder.Property(p => p.IdFuncionalidade)
                .HasColumnName("idFuncionalidade")
                .IsRequired();

            builder.HasOne(d => d.Perfil)
                .WithMany(p => p.VinculoPerfilFuncionalidades)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Funcionalidade)
                .WithMany(p => p.VinculoPerfilFuncionalidades)
                .HasForeignKey(d => d.IdFuncionalidade)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Ignore(x => x.DataAlteracao);
            builder.Ignore(x => x.Usuario);
        }
    }
}
