using Account.Domain.ProjetoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Account.Infra.Data.SqlServer.Mappings.SistemaRootMap
{
    public class SistemaMap : IEntityTypeConfiguration<Sistema>
    {
        public void Configure(EntityTypeBuilder<Sistema> builder)
        {
            builder.ToTable("Sistema");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(p => p.Titulo)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(p => p.Descricao)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.Link)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(p => p.Ordem)
                .IsRequired();

            //builder.Property(p => p.Imagem)
            //    .HasColumnName("IMAGEM")
            //    .HasColumnType("varchar(50)")
            //    .IsRequired(false);

            builder.Property(x => x.DtAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.LgUsuario)
                .HasMaxLength(30)
                .IsRequired(false)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");
        }
    }
}
