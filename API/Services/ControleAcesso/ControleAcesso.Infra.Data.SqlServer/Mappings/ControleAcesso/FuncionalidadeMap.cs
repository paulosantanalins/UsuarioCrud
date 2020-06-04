using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class FuncionalidadeMap : IEntityTypeConfiguration<Funcionalidade>
    {
        public void Configure(EntityTypeBuilder<Funcionalidade> builder)
        {
            builder.ToTable("tblFuncionalidade");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idFuncionalidade")
                .IsRequired();

            builder.Property(p => p.NmFuncionalidade)
                .HasColumnName("nmFuncionalidade")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.DescFuncionalidade)
                .HasColumnName("descFuncionalidade")
                .HasColumnType("varchar(300)");

            builder.Property(p => p.FlAtivo)
                .HasColumnName("flAtivo")
                .IsRequired();


            builder.Ignore(x => x.DataAlteracao);
            builder.Ignore(x => x.Usuario);
        }
    }
}
