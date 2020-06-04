using GestaoServico.Domain.GestaoFilialRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoFilialMap
{
    public class EmpresaMap : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLEMPRESA");

            builder.Property(e => e.Id).HasColumnName("IDEMPRESA");

            builder.Property(e => e.NmEmpresa)
                .IsRequired()
                .HasColumnName("NMEMPRESA")
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.NmRazaoSocial)
                .IsRequired()
                .HasColumnName("NMRAZAOSOCIAL")
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Ignore(x => x.DataAlteracao).Ignore(x => x.Usuario);

            //builder.Property(x => x.DataAlteracao)
            //    .HasColumnName("DTALTERACAO")
            //    .IsRequired(false);

            //builder.Property(x => x.Usuario)
            //    .HasMaxLength(30)
            //    .IsRequired(true)
            //    .IsUnicode(false)
            //    .HasColumnName("LGUSUARIO");

        }
    }
}