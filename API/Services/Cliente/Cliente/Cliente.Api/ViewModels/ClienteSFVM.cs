using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.ViewModels
{
    public class ClienteSFVM
    {
        public string cnpj { get; set; }
        public string nomeFantasia { get; set; }
        public string razaoSocial { get; set; }
        public string abrevLogradouro { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string compEndereco { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string pais { get; set; }
        public string idSalesForce { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
    }
}
