using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ExtensaoContratoPrestadorMap : IEntityTypeConfiguration<ExtensaoContratoPrestador>
    {
        public void Configure(EntityTypeBuilder<ExtensaoContratoPrestador> builder)
        {
            builder.ToTable("TBLEXTENSAOCONTRATOPRESTADOR");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("IDEXTENSAOCONTRATOPRESTADOR")
                .IsRequired(true);


            builder.Property(e => e.DataInicio)
               .HasColumnName("DTINICIO")
               .IsRequired(false);

            builder.Property(e => e.DataFim)
               .HasColumnName("DTFIM");

            builder.Property(e => e.NomeAnexo)
              .HasColumnName("NOMEANEXO")
              .IsRequired(false); ;

            builder.Property(e => e.CaminhoContrato)
              .HasColumnName("VLCAMINHOCONTRATO")
              .IsRequired(false);

            builder.Property(e => e.Inativo)
              .HasColumnName("FLINATIVO")
              .IsRequired();

            builder.Property(e => e.Tipo)
                .HasColumnName("TIPO")
                .IsRequired();

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsRequired();

            builder.HasOne(e => e.ContratoPrestador)
                .WithMany(c => c.ExtensoesContratoPrestador)
                .HasForeignKey(x => x.IdContratoPrestador);
        }
    }
}
