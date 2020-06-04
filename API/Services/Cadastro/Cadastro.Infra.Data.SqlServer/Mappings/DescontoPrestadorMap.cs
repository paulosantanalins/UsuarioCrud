using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class DescontoPrestadorMap : IEntityTypeConfiguration<DescontoPrestador>
    {
        public void Configure(EntityTypeBuilder<DescontoPrestador> builder)
        {
            builder.HasKey(e => e.Id);


            builder.ToTable("TBLHORASMESPRESTADORDESCONTO");

            builder.Property(e => e.Id)
               .HasColumnName("IDHORASMESPRESTADORDESCONTO");

            builder.Property(e => e.IdHorasMesPrestador)
                .HasColumnName("IDHORASMESPRESTADOR");

            builder.Property(e => e.IdDesconto)
                .HasColumnName("IDDESCONTO");

            builder.Property(e => e.ValorDesconto)
                .HasColumnName("VLDESCONTO");

            builder.Property(e => e.DescricaoDesconto)
                .HasColumnName("DESCDESCONTO");
            
            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(d => d.Desconto)
                .WithMany(p => p.DescontosPrestador)
                .HasForeignKey(d => d.IdDesconto);

            builder.HasOne(d => d.HorasMesPrestador)
                .WithMany(p => p.DescontosPrestador)
                .HasForeignKey(d => d.IdHorasMesPrestador);
        }
    }
}
