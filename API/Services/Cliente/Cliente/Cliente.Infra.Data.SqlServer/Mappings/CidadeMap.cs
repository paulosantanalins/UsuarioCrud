using Cliente.Domain.ClienteRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Infra.Data.SqlServer.Mappings
{
    public class CidadeMap : IEntityTypeConfiguration<Cidade>
    {
        public void Configure(EntityTypeBuilder<Cidade> builder)
        {
            builder.ToTable("tblCidade");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idCidade")
                .IsRequired(true);

            builder.Property(p => p.IdEstado)
                .HasColumnName("idEstado")
                .IsRequired(true);

            builder.Property(p => p.NmCidade)
                .HasColumnName("nmCidade")
                .IsRequired(true);

            builder.HasMany(p => p.Enderecos)
                .WithOne(x => x.Cidade)
                .HasForeignKey(x => x.IdCidade)
                .IsRequired(true);

            builder.Ignore(e => e.DataAlteracao);

            builder.Ignore(e => e.Usuario);
        }
    }
}
