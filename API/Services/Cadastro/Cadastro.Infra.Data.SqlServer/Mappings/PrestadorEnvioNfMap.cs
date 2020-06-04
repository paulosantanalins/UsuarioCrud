using Cadastro.Domain.NotaFiscalRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    public class PrestadorEnvioNfMap : IEntityTypeConfiguration<PrestadorEnvioNf>
    {
        public void Configure(EntityTypeBuilder<PrestadorEnvioNf> builder)
        {
            builder.ToTable("TBLPRESTADORENVIONF");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDPRESTADORENVIONF");

            builder.HasOne(d => d.HorasMesPrestador).WithMany(p => p.PrestadoresEnvioNf);

            builder.Property(e => e.Token)
                    .HasColumnName("VLTOKEN");

            builder.Property(e => e.CaminhoNf)
                    .HasColumnName("VLCAMINHONF");

            builder.Property(u => u.Usuario)
                .HasColumnName("LGUSUARIO")
                .IsUnicode(false);

            builder.Property(x => x.DataAlteracao)
                .HasColumnName("DTALTERACAO");
        }
    }
}
