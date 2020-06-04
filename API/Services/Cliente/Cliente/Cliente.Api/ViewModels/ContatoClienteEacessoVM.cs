using Cliente.Domain.ClienteRoot.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.ViewModels
{
    public class ContatoClienteEacessoVM
    {
        public int IdContato { get; set; }
        public int IdCliente { get; set; }
        public string NomeContato { get; set; }
        public string TipoContato { get; set; }
        public string DepartamentoContato { get; set; }
        public string CargoContato { get; set; }
        public string AbreviaturaLogradouroContato { get; set; }
        public string EnderecoContato { get; set; }
        public string CepContato { get; set; }
        public string ComplementoEnderecoContato { get; set; }
        public string NumeroEnderecoContato { get; set; }
        public string BairroContato { get; set; }
        public string ObservacaoContato { get; set; }
        public string EmailContato { get; set; }
        public string CidadeContato { get; set; }
        public string EstadoContato { get; set; }
        public List<TelefoneContatoClienteVM> Telefones { get; set; } = new List<TelefoneContatoClienteVM>();
    }
}
