using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class PrestadorMap : IEntityTypeConfiguration<Prestador>
    {
        public void Configure(EntityTypeBuilder<Prestador> builder)
        {
            builder.ToTable("TBLPRESTADOR");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("IDPRESTADOR");

            builder.Property(e => e.IdEmpresaGrupo)
                .HasColumnName("IDEMPRESAGRUPO");

            builder.Property(e => e.IdCelula)
                .HasColumnName("IDCELULA");

            builder.Property(e => e.IdFilial)
                .HasColumnName("IDFILIAL");

            builder.Property(e => e.DataInicio)
                .HasColumnName("DTINICIO");

            builder.Property(e => e.ProdutoRm)
                .HasColumnName("NRPRODUTORM")
                .IsUnicode(false);

            builder.Property(e => e.DataValidadeContrato)
                .HasColumnName("DTVALIDADECONTRATO");

            builder.Property(e => e.DataDesligamento)
                .HasColumnName("DTDESLIGAMENTO");
        
            builder.Property(e => e.IdCargo)
                .HasColumnName("IDCARGO");

            builder.Property(e => e.IdContratacao)
                .HasColumnName("IDCONTRATACAO");

            builder.Property(e => e.IdSituacao)
                .HasColumnName("IDSITUACAO");

            builder.Property(e => e.IdDiaPagamento)
                .HasColumnName("IDDIAPAGAMENTO");

            builder.Property(e => e.IdAreaFormacao)
                .HasColumnName("IDAREA");

            builder.Property(e => e.CodEacessoLegado)
                .HasColumnName("CDEACESSOLEGADO");

            builder.Property(e => e.IdTipoRemuneracao)
                .HasColumnName("IDTIPOREMUNERACAO");

            builder.Property(e => e.DataUltimaAlteracaoEacesso)
                .HasColumnName("DTULTIMAALTERACAO");

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO");

            builder.Property(e => e.IdProdutoRm)
                .HasColumnName("IDPRODUTORM");

            builder.Property(e => e.IdRepresentanteRmTRPR)
                .HasColumnName("IDREPRESENTANTERMTRPR");

            builder.Property(e => e.EmailParceiro)
                .HasColumnName("NMEMAILPARCEIRO")
                .IsUnicode(false);

            builder.Property(e => e.IdPessoa)
                .HasColumnName("IDPESSOA");

            builder.Property(e => e.IdPessoaAlteracao)
                .HasColumnName("IDPESSOAALTERACAO");

            builder.Property(e => e.DataInicioCLT)
                .HasColumnName("DTINICIOCLT");

            builder.Property(e => e.PagarPelaMatriz)
                .HasColumnName("FLPAGARPELAMATRIZ");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(d => d.Celula)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdCelula);

            builder.HasOne(d => d.Cargo)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdCargo);

            builder.HasOne(d => d.Contratacao)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdContratacao);

            builder.HasOne(d => d.SituacaoPrestador)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdSituacao);

            builder.HasOne(d => d.DiaPagamento)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdDiaPagamento);

            builder.HasOne(d => d.AreaFormacao)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdAreaFormacao);

            builder.HasOne(d => d.TipoRemuneracao)
                .WithMany(p => p.Prestadores)
                .HasForeignKey(d => d.IdTipoRemuneracao);

            builder.HasMany(x => x.ContratosPrestador)
                .WithOne(p => p.Prestador);

            builder.HasMany(x => x.DocumentosPrestador)
                .WithOne(p => p.Prestador);
        }
    }
}
