using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{

    public class EmpresaMap : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLEMPRESA");

            builder.Property(e => e.Id).HasColumnName("IDEMPRESA");

            builder.Property(e => e.IdEndereco).HasColumnName("IDENDERECO");

            builder.Property(p => p.Cnpj)
                .HasColumnName("NRCNPJ");

            builder.Property(p => p.RazaoSocial)
                .HasColumnName("NMRAZAOSOCIAL");

            builder.Property(p => p.DataVigencia)
                .HasColumnName("DTVIGENCIA");

            builder.Property(p => p.InscricaoEstadual)
                .HasColumnName("NRINSCESTADUAL");

            builder.Property(p => p.Atuacao)
                .HasColumnName("DESCATUACAO");

            builder.Property(p => p.Socios)
                .HasColumnName("DESCSOCIOS");

            builder.Property(p => p.Observacao)
                .HasColumnName("DESCOBSERVACAO");

            builder.Property(e => e.Ativo)
                .HasColumnName("FLATIVO");

            builder.Property(p => p.IdEmpresaRm)
                .HasColumnName("IDEMPRESARM");

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(p => p.Endereco)
                .WithMany(x => x.Empresas)
                .HasForeignKey(x => x.IdEndereco)
                .IsRequired(true);

            builder.HasMany(p => p.SociosEmpresaPrestador)
                .WithOne(x => x.Empresa);
        }
    }
}