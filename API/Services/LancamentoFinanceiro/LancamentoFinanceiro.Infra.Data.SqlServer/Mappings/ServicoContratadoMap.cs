using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class ServicoContratadoMap : IEntityTypeConfiguration<ServicoContratado>
    {
        public void Configure(EntityTypeBuilder<ServicoContratado> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLSERVICOCONTRATADO");

            builder.Property(e => e.Id)
                .HasColumnName("IDSERVICOCONTRATADO");

            builder.Property(e => e.IdContrato)
                .HasColumnName("IDCONTRATO");

            builder.Property(e => e.IdEmpresa)
                .HasColumnName("IDEMPRESA");

            builder.Property(e => e.DtInicial)
                .HasColumnName("DTINICIOVIGENCIA")
                .HasColumnType("date");

            builder.Property(e => e.DtFinal)
                .HasColumnName("DTFIMVIGENCIA")
                .HasColumnType("date");

            builder.Property(e => e.VlKM)
                .HasColumnName("VLKM")
                .HasColumnType("decimal(11, 2)");

            builder.Property(e => e.FlReembolso)
                .HasColumnName("FLREEMBOLSO");

            builder.Property(e => e.FlHorasExtrasReembosaveis)
                .HasColumnName("FLHORASEXTRASREEMBOLSAVEIS");

            builder.Property(e => e.IdProdutoRM)
                .HasColumnName("NRPRODUTORM");

            builder.Property(e => e.QtdExtraReembolso)
                .HasColumnName("QTDEXTRAREEMBOLSO");

            builder.Property(e => e.NmTipoReembolso)
                .HasColumnName("NMTIPOREEMBOLSO");

            builder.Property(e => e.VlRentabilidade)
                .HasColumnName("VLRENTABILIDADEPREVISTA");

            builder.Property(e => e.FlFaturaRecorrente)
                .HasColumnName("FLFATURARECORRENTE");

            builder.Property(e => e.FormaFaturamento)
                .HasColumnName("DESCFORMAFATURAMENTO");

            builder.Property(e => e.FlReoneracao)
                .HasColumnName("FLREONERACAO");

            builder.Property(e => e.IdProdutoRM)
                .HasColumnName("NRPRODUTORM");

            builder.Property(e => e.FlHorasExtrasReembosaveis)
                .HasColumnName("FLHORASEXTRASREEMBOLSAVEIS");

            builder.Property(e => e.IdFilial)
                .HasColumnName("IDFILIAL");

            builder.Property(e => e.IdEscopoServico)
                .HasColumnName("IDESCOPOSERVICO");

            builder.Property(e => e.DescTipoCelula)
                .HasColumnName("DESCTIPOCELULA");

            builder.Property(e => e.IdCelula)
                .HasColumnName("IdCelula");

            builder.Property(e => e.DescricaoServicoContratado)
                .HasColumnName("DESCRICAOSERVICOCONTRATADO");

            builder.Property(e => e.IdGrupoDelivery)
                .HasColumnName("IdGrupoDelivery");

            builder.Property(e => e.IdFrenteNegocio)
                .HasColumnName("IdFrenteNegocio");

            builder.Property(e => e.IdServicoContratadoOrigem)
                .HasColumnName("IDSERVICOCONTRATADOORIGEM");

            builder.HasOne(d => d.ServicoContratadoOrigem)
                .WithMany(p => p.ServicosContratadosMigracao)
                .HasForeignKey(d => d.IdServicoContratadoOrigem);

            builder.HasOne(d => d.EscopoServico)
                .WithMany(p => p.ServicoContratados)
                .HasForeignKey(d => d.IdEscopoServico);

            builder.HasOne(d => d.Contrato)
                .WithMany(p => p.ServicoContratados)
                .HasForeignKey(d => d.IdContrato);

            builder.Property(x => x.DtAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.LgUsuario)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");
        }
    }
}
