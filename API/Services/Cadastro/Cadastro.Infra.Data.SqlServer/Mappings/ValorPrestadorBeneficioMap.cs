using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class ValorPrestadorBeneficioMap : IEntityTypeConfiguration<ValorPrestadorBeneficio>
    {
        public void Configure(EntityTypeBuilder<ValorPrestadorBeneficio> builder)
        {
            builder.ToTable("TBLVALORPRESTADORBENEFICIO");

            builder.HasKey(x => x.Id);            

            builder.Property(x => x.Id)
                .HasColumnName("IDVALORPRESTADORBENEFICIO");

            builder.Property(x => x.IdValorPrestador)
                .HasColumnName("IDVALORPRESTADOR");

            builder.Property(x => x.IdBeneficio)
                .HasColumnName("IDBENEFICIO")
                ;

            builder.Property(x => x.ValorBeneficio)
                .HasColumnName("VLBENEFICIO")
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
              .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
               .HasMaxLength(30)
               .IsRequired(true)
               .IsUnicode(false)
               .HasColumnName("LGUSUARIO");

            builder.HasOne(x => x.ValorPrestador)
                .WithMany(p => p.ValoresPrestadorBeneficio)
                .HasForeignKey(x => x.IdValorPrestador);

            builder.HasOne(x => x.Beneficio)
               .WithMany(p => p.ValoresPrestadorBeneficioDom)
               .HasForeignKey(x => x.IdBeneficio);
        }
    }
}
