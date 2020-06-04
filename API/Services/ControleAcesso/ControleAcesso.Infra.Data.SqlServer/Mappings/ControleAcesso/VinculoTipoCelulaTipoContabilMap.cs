using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.DominioRoot.ItensDominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class VinculoTipoCelulaTipoContabilMap : IEntityTypeConfiguration<VinculoTipoCelulaTipoContabil>
    {
        public void Configure(EntityTypeBuilder<VinculoTipoCelulaTipoContabil> builder)
        {
            builder.ToTable("TBLTIPOCELULATIPOCONTABIL");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDTIPOCELULATIPOCONTABIL")
                .IsRequired();

            builder.Property(p => p.IdTipoCelula)
                  .HasColumnName("IDTIPOCELULA")
                  .IsRequired();

            builder.Property(p => p.IdTipoContabil)
              .HasColumnName("IDTIPOCONTABIL")
              .IsRequired();


            builder.HasOne(d => d.Tipocelula)
                .WithOne(p => p.TipoCelulaTipoContabil)
                .HasForeignKey<VinculoTipoCelulaTipoContabil>(b => b.IdTipoCelula);


            builder.HasOne(d => d.TipoContabil)
                .WithMany(p => p.VinculosTipoCelulaTiposContabil)
                .HasForeignKey(d => d.IdTipoContabil);


            builder.Ignore(x=>x.Usuario)
                 .Ignore(x=>x.DataAlteracao);

        }
    }
}
