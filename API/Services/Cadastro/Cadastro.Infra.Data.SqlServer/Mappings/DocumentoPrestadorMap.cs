using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class DocumentoPrestadorMap : IEntityTypeConfiguration<DocumentoPrestador>
    {
        public void Configure(EntityTypeBuilder<DocumentoPrestador> builder)
        {
            builder.ToTable("TBLDOCUMENTOPRESTADOR");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName("IDDOCUMENTOPRESTADOR")
                .IsRequired();

            builder.Property(e => e.CaminhoDocumento)
                .HasColumnName("VLCAMINHODOCUMENTO")
                .IsRequired();          

            builder.Property(e => e.NomeAnexo)
                .HasColumnName("NOMEANEXO");
            
            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired();

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsRequired();

            builder.Property(e => e.Inativo)
                .HasColumnName("FLINATIVO")
                .IsRequired();

            builder.Property(e => e.IdTipoDocumentoPrestador)
                .HasColumnName("IDTIPODOCUMENTOPRESTADOR");

            builder.HasOne(p => p.Prestador)
                .WithMany(p => p.DocumentosPrestador)
                .HasForeignKey(p => p.IdPrestador);
            
            builder.HasOne(p => p.TipoDocumentoPrestador)
                .WithMany(p => p.DocumentosPrestador)
                .HasForeignKey(p => p.IdTipoDocumentoPrestador);

            builder.Ignore(x => x.DescricaoTipoOutros);
        }
    }
}
