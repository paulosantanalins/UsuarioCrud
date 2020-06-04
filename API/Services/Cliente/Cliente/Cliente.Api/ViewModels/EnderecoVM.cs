using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.ViewModels
{
    public class EnderecoVM
    {
        public int Id { get; set; }
        public int IdCidade { get; set; }
        public int IdCliente { get; set; }
        public string SgAbrevLogradouro { get; set; }
        public string NmEndereco { get; set; }
        public string NrEndereco { get; set; }
        public string NmCompEndereco { get; set; }
        public string NmBairro { get; set; }
        public string NrCep { get; set; }

        public virtual ClienteVM Cliente { get; set; }
        public virtual CidadeVM Cidade { get; set; }
    }
}
