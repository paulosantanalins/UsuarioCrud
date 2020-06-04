using Cliente.Domain.ClienteRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Infra.Data.SqlServer.Mappings
{
    public class GrupoClienteMap : IEntityTypeConfiguration<GrupoCliente>
    {
        public void Configure(EntityTypeBuilder<GrupoCliente> builder)
        {
            builder.ToTable("tblGrupoCliente");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idGrupoCliente")
                .IsRequired(true);

            builder.Property(p => p.DescGrupoCliente)
                .HasColumnName("descGrupoCliente")
                .IsRequired(true);

            builder.Property(p => p.FlStatus)
                .HasColumnName("FlStatus")
                .IsRequired(true);

            builder.HasMany(p => p.Clientes)
                .WithOne(x => x.GrupoCliente)
                .HasForeignKey(x => x.IdGrupoCliente)
                .IsRequired(false);

            builder.HasIndex(e => e.IdClienteMae)
                    .HasName("UN_GRUPOCLIENTE_IDCLIENTEMAE ")
                    .IsUnique();

            builder.Property(e => e.IdClienteMae).HasColumnName("IDCLIENTEMAE");

            builder.Ignore(e => e.DataAlteracao);

            builder.Ignore(e => e.Usuario);
        }
    }
}
