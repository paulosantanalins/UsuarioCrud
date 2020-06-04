using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Infra.Data.SqlServer.Mappings
{
    public class ProfissionaisMap : IEntityTypeConfiguration<Profissionais>
    {
        public void Configure(EntityTypeBuilder<Profissionais> builder)
        {
            builder.ToTable("tblProfissionais");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("Id");

            builder.Property(p => p.CPF)
                .HasColumnName("CPF");
            builder.Property(p => p.Nome)
                .HasColumnName("Nome");
            builder.Property(p => p.Celula)
                .HasColumnName("Celula");
            builder.Ignore(e => e.DataAlteracao)
                   .Ignore(e => e.Usuario);
        }
    }
}
