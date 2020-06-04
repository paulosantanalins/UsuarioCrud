using Cadastro.Domain.CelulaRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class CelulaMap : IEntityTypeConfiguration<Celula>
    {
        public void Configure(EntityTypeBuilder<Celula> builder)
        {
            builder.ToTable("TBLCELULA");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDCELULA");

            builder.Property(e => e.Descricao)
                .HasColumnName("DESCCELULA")
                .IsUnicode(false)
                .IsRequired(true);

            builder.Property(e => e.IdCelulaSuperior)
                .HasColumnName("IDCELULASUPERIOR")
                .IsRequired(false);

            builder.Property(e => e.IdGrupo)
                .HasColumnName("IDGRUPO")
                .IsRequired(false);

            builder.Property(e => e.IdTipoCelula)
                .HasColumnName("IDTIPOCELULA")
                .IsRequired(false);

            builder.Property(e => e.IdPessoaResponsavel)
                .HasColumnName("IDPESSOARESPONSAVEL")
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasColumnName("STATUS")
                .IsRequired();

            builder.Property(e => e.NomeResponsavel)
                .HasColumnName("NMRESPONSAVEL")
                .IsUnicode(false)
                .IsRequired(true);

            builder.Property(e => e.EmailResponsavel)
                .HasColumnName("NMEMAILRESPONSAVEL")
                .IsUnicode(false)
                .IsRequired(true);

            builder.Property(e => e.FlHabilitarRepasseMesmaCelula)
                .HasColumnName("FLHABILITARREPASSEMESMACELULA");

            builder.Property(e => e.FlHabilitarRepasseEpm)
                .HasColumnName("FLHABILITARREPASSEEPM");

            builder.Property(e => e.DataAlteracao)
                .HasColumnName("DTULTIMAALTERACAO");

            builder.Property(e => e.Usuario)
                .HasColumnName("LGUSUARIO");

            builder.HasOne(d => d.Grupo)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdGrupo);

            builder.HasOne(d => d.TipoCelula)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdTipoCelula);

            builder.HasOne(d => d.CelulaSuperior)
                .WithMany(p => p.CelulasSuperiores)
                .HasForeignKey(d => d.IdCelulaSuperior);

            builder.HasOne(d => d.Pessoa)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdPessoaResponsavel);
        }
    }
}
