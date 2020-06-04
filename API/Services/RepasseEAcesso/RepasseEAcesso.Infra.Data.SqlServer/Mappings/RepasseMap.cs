using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class RepasseMap : IEntityTypeConfiguration<Repasse>
    {
        public void Configure(EntityTypeBuilder<Repasse> builder)
        {
            builder.ToTable("EFATURAMENTO_REPASSE");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("IDREPASSE");

            builder.Property(p => p.IdCelulaOrigem)
                .HasColumnName("CD_CEL_ORIGEM");
            builder.Property(p => p.IdClienteOrigem)
                .HasColumnName("IDCLIENTE1");
            builder.Property(p => p.IdServicoOrigem)
                .HasColumnName("IDSERVICO1");
            builder.Property(p => p.IdCelulaDestino)
                .HasColumnName("CD_CEL_DESTINO");
            builder.Property(p => p.IdClienteDestino)
                .HasColumnName("IDCLIENTE");
            builder.Property(p => p.IdServicoDestino)
                .HasColumnName("IDSERVICO");
            builder.Property(p => p.DataRepasse)
                .HasColumnName("DT_REPASSE");
            builder.Property(p => p.IdProfissional)
                .HasColumnName("IDPROFISSIONAL");
            builder.Property(p => p.ValorCustoProfissional)
                .HasColumnName("Vl_Custo_Prof");
            builder.Property(p => p.ValorUnitario)
                .HasColumnName("vl_desp_unit");
            builder.Property(p => p.QuantidadeItens)
                .HasColumnName("qt_horas");
            builder.Property(p => p.ValorTotal)
                .HasColumnName("vl_despesa");
            builder.Property(p => p.IdMoeda)
                .HasColumnName("idMoedaPagto");
            builder.Property(p => p.DescricaoProjeto)
                .HasColumnName("DescProj");
            builder.Property(p => p.MotivoNegacao)
                .HasColumnName("MotivoNeg");
            builder.Property(p => p.Justificativa)
                .HasColumnName("Justificativa");
            builder.Property(p => p.Status)
                .HasColumnName("status_aprov");
            builder.Property(p => p.IdRepasseMae)
                .HasColumnName("IdRepasseMae");        
            builder.Property(p => p.IdOrigem)
              .HasColumnName("IdOrigem");

            //builder.Property(p => p.IdEpm)
            //    .HasColumnName("IdEmpresa");          
            //builder.Property(p => p.RepasseInterno)
            //    .HasColumnName("FlagDescAjuste");
            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);
        }
    }
}
