using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class PessoaMap : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLPESSOA");

            builder.Property(e => e.Id)
                .HasColumnName("IDPESSOA");

            builder.Property(e => e.Nome)
                .HasColumnName("NMPESSOA");

            builder.Property(e => e.Cpf)
                .HasColumnName("NRCPF");

            builder.Property(e => e.Rg)
                 .HasColumnName("NRRG");

            builder.Property(e => e.DtNascimento)
                .HasColumnName("DTNASCIMENTO")
                .IsRequired(false);

            builder.Property(e => e.NomeDoPai)
                .HasColumnName("NMPAI");

            builder.Property(e => e.NomeDaMae)
                .HasColumnName("NMMAE");

            builder.Property(e => e.Email)
                .HasColumnName("NMEMAIL");

            builder.Property(e => e.EmailInterno)
                .HasColumnName("NMEMAILINTERNO");

            builder.Property(e => e.Matricula)
                .HasColumnName("NRMATRICULA");

            builder.Property(e => e.IdNacionalidade)
                .HasColumnName("IDNACIONALIDADE");

            builder.Property(e => e.IdEscolaridade)
                .HasColumnName("IDESCOLARIDADE");

            builder.Property(e => e.IdExtensao)
                .HasColumnName("IDEXTENSAO");

            builder.Property(e => e.IdGraduacao)
                .HasColumnName("IDGRADUACAO");

            builder.Property(e => e.IdEstadoCivil)
                .HasColumnName("IDESTADOCIVIL");

            builder.Property(e => e.IdSexo)
                .HasColumnName("IDSEXO")
                .IsRequired(false);

            builder.Property(e => e.IdEndereco)
                .HasColumnName("IDENDERECO")
                .IsRequired(false);

            builder.HasOne(d => d.Endereco)
               .WithMany(p => p.Pessoas)
               .HasForeignKey(d => d.IdEndereco);

            builder.Property(e => e.IdTelefone)
                .HasColumnName("IDTELEFONE")
                .IsRequired(false);
            

            builder.HasOne(d => d.Telefone)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdTelefone);

            builder.Property(e => e.CodEacessoLegado)
                .HasColumnName("CDEACESSOLEGADO");

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTALTERACAO");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(d => d.Nacionalidade)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdNacionalidade);

            builder.HasOne(d => d.Escolaridade)
             .WithMany(p => p.Pessoas)
             .HasForeignKey(d => d.IdEscolaridade);

            builder.HasOne(d => d.Extensao)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdExtensao);

            builder.HasOne(d => d.Graduacao)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdGraduacao);

            builder.HasOne(d => d.EstadoCivil)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdEstadoCivil);

            builder.HasOne(d => d.Sexo)
                .WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdSexo);

            builder.HasOne(x => x.Prestador)
               .WithOne(p => p.Pessoa)
               .HasForeignKey<Prestador>(x => x.IdPessoa);
        }
    }
}
