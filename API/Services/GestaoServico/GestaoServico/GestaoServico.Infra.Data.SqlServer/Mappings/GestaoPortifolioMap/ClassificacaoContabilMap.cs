using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GestaoServico.Infra.Data.SqlServer.Mappings.GestaoPortifolioMap
{
    public class ClassificacaoContabilMap : IEntityTypeConfiguration<ClassificacaoContabil>
    {
        public void Configure(EntityTypeBuilder<ClassificacaoContabil> builder)
        {
            builder.ToTable("TBLCLASSIFICACAOCONTABIL");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("IDCLASSIFICACAOCONTABIL");
            builder.Property(e => e.IdCategoriaContabil).HasColumnName("IDCATEGORIACONTABIL");

            builder.Property(x => x.DescClassificacaoContabil)
                .HasMaxLength(150)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("DESCCLASSIFICACAOCONTABIL");

            builder.Property(x => x.SgClassificacaoContabil)
                .HasMaxLength(10)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("SGCLASSIFICACAOCONTABIL");

            builder.Property(x => x.FlStatus)
                .IsRequired(true)
                .HasColumnName("FLSTATUS");

            builder.HasOne(d => d.CategoriaContabil)
                .WithMany(p => p.ClassificacoesContabil)
                .HasForeignKey(d => d.IdCategoriaContabil)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO")
                .IsRequired(false);

            builder.Property(x => x.Usuario)
                .HasMaxLength(30)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnName("LGUSUARIO");

        }
    }
}
