using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class HorasMesPrestadorMap : IEntityTypeConfiguration<HorasMesPrestador>
    {
        public void Configure(EntityTypeBuilder<HorasMesPrestador> builder)
        {
            builder.ToTable("TBLHORASMESPRESTADOR");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IDHORASMESPRESTADOR");

            builder.Property(p => p.IdPrestador)
                .HasColumnName("IDPRESTADOR");

            builder.Property(p => p.IdHorasMes)
                .HasColumnName("IDHORASMES");

            builder.Property(p => p.IdChaveOrigemIntRm)
                .HasColumnName("IDCHAVEORIGEMINTRM");

            builder.Property(p => p.Horas)
                .HasColumnName("VLHORAS");

            builder.Property(p => p.Extras)
                .HasColumnName("VLEXTRAS");

            builder.Property(p => p.Situacao)
                .HasColumnName("NMSITUACAO");

            builder.Property(p => p.SemPrestacaoServico)
                .HasColumnName("FLSEMPRESTACAOSERVICO");

            builder.Property(p => p.ObservacaoSemPrestacaoServico)
                .HasColumnName("OBSSEMPRESTACAOSERVICO")
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
              .HasColumnName("DTALTERACAO");

            builder.Property(x => x.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.HasOne(d => d.Prestador)
                .WithMany(p => p.HorasMesPrestador)
                .HasForeignKey(d => d.IdPrestador);

            builder.HasOne(d => d.HorasMes)
                .WithMany(p => p.HorasMesPrestador)
                .HasForeignKey(d => d.IdHorasMes);

            builder.HasMany(p => p.PrestadoresEnvioNf)
                .WithOne(p => p.HorasMesPrestador)
                .HasForeignKey(k => k.IdHorasMesPrestador);
            
        }
    }
}
