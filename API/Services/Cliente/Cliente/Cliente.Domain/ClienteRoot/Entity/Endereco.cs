using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Entity
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
        //public string Referencia { get; set; }

        public virtual ClienteET Cliente { get; set; }
        public virtual Cidade Cidade { get; set; }
    }
}
