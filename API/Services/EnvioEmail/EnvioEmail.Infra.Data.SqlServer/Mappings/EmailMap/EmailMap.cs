using EnvioEmail.Domain.EmailRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EnvioEmail.Infra.Data.SqlServer.Mappings.EmailMap
{
    public class EmailMap : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> entity)
        {

            entity.HasKey(e => e.Id);

            entity.ToTable("TBLEMAIL");

            entity.Property(e => e.Id)
                .HasColumnName("IDEMAIL");

            entity.Property(e => e.DtCadastro)
                .HasColumnName("DTCADASTRO")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.Assunto)
                .HasColumnName("DESCASSUNTO")
                .IsUnicode(false);

            entity.Property(e => e.Corpo)
                .HasColumnName("DESCCORPOEMAIL")
                .IsUnicode(false);

            entity.Property(e => e.Erro)
                .HasColumnName("DESCERRO")
                .IsUnicode(false);

            entity.Property(e => e.Status)
                .HasColumnName("DESCSTATUSEMAIL");

            entity.Property(e => e.DtEnvio)
                .HasColumnName("DTENVIO")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.DtParaEnvio)
                .HasColumnName("DTPARAENVIO")
                .HasColumnType("smalldatetime");

            entity.Property(e => e.IdTemplate)
                .HasColumnName("IDTEMPLATE");

            entity.Property(e => e.Para)
                .HasColumnName("NMDESTINATARIO")
                .IsUnicode(false);

            entity.Property(e => e.ComCopia)
                .HasColumnName("NMDESTINATARIOCOPIA")
                .IsUnicode(false);

            entity.Property(e => e.ComCopiaOculta)
                .HasColumnName("NMDESTINATARIOCOPIAOCULTA")
                .IsUnicode(false);

            entity.Property(e => e.RemetenteNome)
                .HasColumnName("NMREMETENTE")
                .IsUnicode(false);

            entity.Property(e => e.TentativasComErro)
                .HasColumnName("NRTENTATIVAENVIOEMAIL");

            entity.Property(e => e.RemetenteEmail)
                .HasColumnName("NMEMAILREMETENTE")
                .IsUnicode(false)
                .IsRequired(false);

            entity.HasOne(d => d.Template)
                .WithMany(p => p.Emails)
                .HasForeignKey(d => d.IdTemplate);
            
        }
    }
}
