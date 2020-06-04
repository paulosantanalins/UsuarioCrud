using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<ClienteET>
    {
        public void Configure(EntityTypeBuilder<ClienteET> builder)
        {
            builder.ToTable("tblCliente");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("idCliente")
                .IsRequired();

            builder.Property(p => p.IdGrupoCliente)
                .HasColumnName("idGrupoCliente")
                .IsRequired(false);

            builder.Property(p => p.IdEAcesso)
                .HasColumnName("idEAcesso");

            builder.Property(p => p.NrCnpj)
                .HasColumnName("nrCnpj")
                .IsRequired();

            builder.Property(p => p.NmFantasia)
                .HasColumnName("nmFantasia")
                .IsRequired(false);

            builder.Property(p => p.NmRazaoSocial)
                .HasColumnName("nmRazaoSocial")
                .IsRequired();

            builder.Property(p => p.IdSalesforce)
                .HasColumnName("idSalesforce")
                .IsRequired();

            builder.Property(p => p.FlTipoHierarquia)
                .HasColumnName("FlTipoHierarquia")
                .HasColumnType("char(1)")
                .IsRequired();

            builder.Property(p => p.FlStatus)
                .HasColumnName("flStatus")
                .HasColumnType("char(1)")
                .IsRequired();

            builder.Property(p => p.NrTelefone)
                .HasColumnName("NrTelefone")
                .IsRequired(false);

            builder.Property(p => p.NrTelefone2)
                .HasColumnName("NrTelefone2")
                .IsRequired(false);

            builder.Property(p => p.NrFax)
                .HasColumnName("NrFax")
                .IsRequired(false);

            builder.Property(p => p.NmSite)
                .HasColumnName("NmSite")
                .IsRequired(false);

            builder.Property(p => p.NmEmail)
                .HasColumnName("NmEmail")
                .IsRequired(false);

            builder.Property(p => p.NrInscricaoEstadual)
                .HasColumnName("NrInscricaoEstadual")
                .IsRequired(false);

            builder.Property(p => p.NrInscricaoMunicipal)
                .HasColumnName("NrInscricaoMunicipal")
                .IsRequired(false);

            builder.HasMany(p => p.Enderecos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.IdCliente)
                .IsRequired();


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
