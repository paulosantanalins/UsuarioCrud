using Cadastro.Domain.PrestadorRoot.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Infra.Data.SqlServer.Mappings
{
    class SocioEmpresaPrestadorMap : IEntityTypeConfiguration<SocioEmpresaPrestador>
    {
        public void Configure(EntityTypeBuilder<SocioEmpresaPrestador> builder)
        {
            builder.ToTable("TBLSOCIOEMPRESAPRESTADOR");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("IDSOCIOEMPRESAPRESTADOR");
            builder.Property(e => e.NomeSocio).HasColumnName("NOMESOCIO");
            builder.Property(e => e.CpfSocio).HasColumnName("CPFSOCIO");
            builder.Property(e => e.RgSocio).HasColumnName("RGSOCIO");
            builder.Property(e => e.Profissao).HasColumnName("PROFISSAO");
            builder.Property(e => e.TipoPessoa).HasColumnName("TIPOPESSOA");
            builder.Property(e => e.IdNacionalidade).HasColumnName("IDNACIONALIDADE");
            builder.Property(e => e.IdEstadoCivil).HasColumnName("IDESTADOCIVIL");
            builder.Property(e => e.Participacao).HasColumnName("PARTICIPACAO");
            builder.Property(e => e.IdEAcesso).HasColumnName("IDEACESSO");
            builder.Property(e => e.IdEmpresa).HasColumnName("IDEMPRESA");
            builder.Property(e => e.IdEmpresaEacesso).HasColumnName("IDEMPRESAEACESSO");
            builder.Property(e => e.Usuario).HasColumnName("LGUSUARIO");
            builder.Property(e => e.DataAlteracao).HasColumnName("DTALTERACAO");

            builder.HasOne(x => x.Empresa).WithMany(x => x.SociosEmpresaPrestador).HasForeignKey(x => x.IdEmpresa);
        }
    }
}
