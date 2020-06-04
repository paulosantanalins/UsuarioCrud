using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.ItensDominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
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

            builder.Property(x => x.DataAlteracao)
              .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);
            CreateDiscriminators(builder);
        }

        private void CreateDiscriminators(EntityTypeBuilder<Dominio> builder)
        {
            builder.HasDiscriminator(x => x.ValorTipoDominio)
                .HasValue<DomNacionalidade>("NACIONALIDADE")
                .HasValue<DomCargo>("CARGO")
                .HasValue<DomContratacao>("CONTRATACAO")
                .HasValue<DomEscolaridade>("ESCOLARIDADE")
                .HasValue<DomExtensao>("EXTENSAO")
                .HasValue<DomGraduacao>("GRADUACAO")
                .HasValue<DomEstadoCivil>("ESTADOCIVIL")
                .HasValue<DomSexo>("SEXO")
                .HasValue<DomSituacaoPrestador>("SITUACAO_PRESTADOR")
                .HasValue<DomDiaPagamento>("DIA_PAGAMENTO")
                .HasValue<DomAreaFormacao>("AREA_FORMACAO")
                .HasValue<DomTipoRemuneracao>("TIPO_REMUNERACAO")
                .HasValue<DomDesconto>("DESCONTO")
                .HasValue<DomBeneficio>("BENEFICIO")
                .HasValue<DomTipoOcorrencia>("OCORRENCIA_OBS")
                .HasValue<DomTipoDocumentoPrestador>("TIPO_DOCUMENTO_PRESTADOR");
        }
    }
}
