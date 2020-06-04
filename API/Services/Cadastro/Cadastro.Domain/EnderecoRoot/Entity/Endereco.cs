using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Domain.EnderecoRoot.Entity
{
    public class Endereco : EntityBase
    {
        public int? IdCidade { get; set; }
        public int? IdCliente { get; set; }
        public string SgAbrevLogradouro { get; set; }
        public string NmEndereco { get; set; }
        public string NrEndereco { get; set; }
        public string NmCompEndereco { get; set; }
        public string NmBairro { get; set; }
        public string NrCep { get; set; }      
        [NotMapped]
        public int IdProfissional { get; set; }
        [NotMapped]
        public string NomeCidade { get; set; }        
        public virtual ICollection<Empresa> Empresas { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual AbreviaturaLogradouro AbreviaturaLogradouro { get; set; }
    }
}
