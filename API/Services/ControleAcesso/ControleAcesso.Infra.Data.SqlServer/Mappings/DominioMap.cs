using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.DominioRoot.ItensDominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings
{
    class DominioMap : IEntityTypeConfiguration<Dominio>
    {
        public void Configure(EntityTypeBuilder<Dominio> builder)
        {
            builder.ToTable("TBLDOMINIO");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDDOMINIO")
                .IsRequired(true);

            builder.Property(p => p.ValorTipoDominio)
                .HasColumnName("VLTIPODOMINIO")
                .IsRequired(true);

            builder.Property(p => p.IdValor)
                .HasColumnName("IDVALOR")
                .IsRequired(true);

            builder.Property(p => p.DescricaoValor)
                .HasColumnName("VLDOMINIO")
                .IsRequired(true);

            builder.Property(p => p.Ativo)
                .HasColumnName("FLATIVO")
                .IsRequired(true);

            CreateDiscriminators(builder);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }

        private void CreateDiscriminators(EntityTypeBuilder<Dominio> builder)
        {
            builder.HasDiscriminator(x => x.ValorTipoDominio)
                .HasValue<DomTipoHierarquia>("TIPO_HIERARQUIA")
                .HasValue<DomTipoContabil>("TIPO_CONTABIL") 
                .HasValue<DomTipoServicoDelivery>("TIPO_SERVICO_DELIVERY"); 
        }
    }
}
