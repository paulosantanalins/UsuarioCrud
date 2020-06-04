using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class TipoCelulaMap : IEntityTypeConfiguration<TipoCelula>
    {
        public void Configure(EntityTypeBuilder<TipoCelula> builder)
        {
            builder.ToTable("TBLTIPOCELULA");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDTIPOCELULA");

            builder.Property(e => e.Descricao)
                .HasColumnName("DESCTIPOCELULA")
                .IsUnicode(false)
                .IsRequired(true);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
    }
}
