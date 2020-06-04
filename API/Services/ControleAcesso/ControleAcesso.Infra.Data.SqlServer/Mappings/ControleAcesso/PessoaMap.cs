using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class PessoaMap : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLPESSOA");

            builder.Property(e => e.Id).HasColumnName("IDPESSOA");

            builder.Property(e => e.Nome)
                .HasColumnName("NMPESSOA");

            builder.Property(e => e.Email)
                .HasColumnName("NMEMAIL");

            builder.Property(e => e.Matricula)
               .HasColumnName("NRMATRICULA");


            builder.Property(e => e.IdEacesso)
                .HasColumnName("CDEACESSOLEGADO")
                .IsRequired(false);

            builder.Ignore(x => x.DataAlteracao)
                    .Ignore(x => x.Usuario);
        }
    }
}
