using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class EmpresaPrestadorMap : IEntityTypeConfiguration<EmpresaPrestador>
    {
        public void Configure(EntityTypeBuilder<EmpresaPrestador> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLEMPRESAPRESTADOR");

            builder.Property(e => e.Id).HasColumnName("IDEMPRESAPRESTADOR");

            builder.Property(e => e.IdEmpresa).HasColumnName("IDEMPRESA");

            builder.Property(e => e.IdPrestador).HasColumnName("IDPRESTADOR");
            
            builder.HasOne(d => d.Empresa)
                 .WithMany(p => p.EmpresasPrestador)
                 .HasForeignKey(d => d.IdEmpresa);

            builder.HasOne(d => d.Prestador)
                .WithMany(p => p.EmpresasPrestador)
                .HasForeignKey(d => d.IdPrestador);

            builder.Ignore(x => x.DataAlteracao)
                   .Ignore(x => x.Usuario);

        }
    }
}
