using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ContratoPrestadorMap : IEntityTypeConfiguration<ContratoPrestador>
    {
        public void Configure(EntityTypeBuilder<ContratoPrestador> builder)
        {
            builder.ToTable("TBLCONTRATOPRESTADOR");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName("IDCONTRATOPRESTADOR")
                .IsRequired();
   
            builder.Property(e => e.NomeAnexo)
                .HasColumnName("NOMEANEXO");

            builder.Property(e => e.DataInicio)
               .HasColumnName("DTINICIO")
               .IsRequired();

            builder.Property(e => e.DataFim)
               .HasColumnName("DTFIM");

            builder.Property(e => e.Tipo)
                .HasColumnName("TIPO")
                .IsRequired();

            builder.Property(e => e.Inativo)
                .HasColumnName("FLINATIVO")
                .IsRequired();

            builder.Property(e => e.CaminhoContrato)
               .HasColumnName("VLCAMINHOCONTRATO");

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsRequired();          

            builder.HasOne(e => e.Prestador)
                .WithMany(c => c.ContratosPrestador)
                .HasForeignKey(x => x.IdPrestador);

            builder.HasMany(x => x.ExtensoesContratoPrestador)
                .WithOne(x => x.ContratoPrestador);
        }
    }
}
