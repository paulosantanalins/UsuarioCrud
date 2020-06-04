using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class Empresa : EntityBase
    {
        public Empresa()
        {
            Endereco = new Endereco();
            EmpresasPrestador = new HashSet<EmpresaPrestador>();
        }
        public int IdEndereco { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DataVigencia { get; set; }
        public string InscricaoEstadual { get; set; }        
        public string Atuacao { get; set; }
        public string Socios { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public int? IdEmpresaRm { get; set; }

        public virtual Endereco Endereco { get; set; }
        public virtual ICollection<EmpresaPrestador> EmpresasPrestador { get; set; }
        public virtual ICollection<SocioEmpresaPrestador> SociosEmpresaPrestador { get; set; }


    }
}
