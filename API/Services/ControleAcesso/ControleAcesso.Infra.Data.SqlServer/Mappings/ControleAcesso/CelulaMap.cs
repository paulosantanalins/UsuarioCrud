using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAcesso.Infra.Data.SqlServer.Mappings.ControleAcesso
{
    public class CelulaMap : IEntityTypeConfiguration<Celula>
    {
        public void Configure(EntityTypeBuilder<Celula> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("TBLCELULA");

            builder.Property(e => e.Id).HasColumnName("IDCELULA");

            builder.Property(e => e.DescCelula)
                .IsRequired()
                .HasColumnName("DESCCELULA")
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

            builder.Property(e => e.IdCelulaSuperior)
                .HasColumnName("IDCELULASUPERIOR");

            builder.Property(e => e.IdGrupo)
                .HasColumnName("IDGRUPO")
                .IsRequired(false);

            builder.Property(e => e.IdTipoCelula)
                .HasColumnName("IDTIPOCELULA")
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
               .HasColumnName("DTULTIMAALTERACAO")
               .IsRequired(false);

            builder.Property(e => e.IdPessoaResponsavel)
               .HasColumnName("IDPESSOARESPONSAVEL");

            builder.Property(e => e.IdTipoHierarquia)
                .HasColumnName("IDTIPOHIERARQUIA")
                .IsRequired(false);

            builder.Property(e => e.IdPais)
                .HasColumnName("IDPAIS")
                .IsRequired(false);

            builder.Property(e => e.IdMoeda)
                .HasColumnName("IDMOEDA")
                .IsRequired(false);

            builder.Property(e => e.IdTipoContabil)
                .HasColumnName("IDTIPOCONTABIL")
                .IsRequired(false);

            builder.Property(e => e.IdTipoServicoDelivery)
                .HasColumnName("IDTIPOSERVICODELIVERY")
                .IsRequired(false);

            builder.Property(e => e.IdEmpresaGrupo)
                .HasColumnName("IDEMPRESAGRUPO")
                .IsRequired(false);

            builder.Property(e => e.DataAlteracao)
               .HasColumnName("DTULTIMAALTERACAO")
               .IsRequired(false);

            builder.Property(e => e.Usuario)
               .HasColumnName("LGUSUARIO")
               .IsRequired(true);

            builder.HasOne(d => d.CelulaSuperior)
                .WithMany(p => p.CelulasSubordinadas)
                .HasForeignKey(d => d.IdCelulaSuperior)
                .HasConstraintName("FK_CELULA");

            builder.HasOne(d => d.Grupo)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdGrupo)
                .HasConstraintName("FK_GRUPO_CELULA");

            builder.HasOne(d => d.Pessoa)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdPessoaResponsavel);

            builder.HasOne(d => d.TipoCelula)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdTipoCelula);

            builder.HasOne(d => d.TipoHierarquia)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdTipoHierarquia);

            builder.HasOne(d => d.TipoContabil)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdTipoContabil);

            builder.HasOne(d => d.TipoServicoDelivery)
                .WithMany(p => p.Celulas)
                .HasForeignKey(d => d.IdTipoServicoDelivery);

            builder.Ignore(x => x.IdEacesso);
        }
    }
}
