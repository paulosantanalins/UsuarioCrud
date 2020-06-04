using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.ViewModels
{
    public class ClienteLocalTrabalhoEacessoVM
    {
        public int IdLocalTrabalho { get; set; }
        public int IdLocal { get; set; }
        public int IdCliente { get; set; }
        public string NumeroTrabalho { get; set; }
        public string NomeTrabalho { get; set; }
        public string EnderecoTrabalho { get; set; }
        public string CepTrabalho { get; set; }
        public string ComplementoEnderecoTrabalho { get; set; }
        public string BairroTrabalho { get; set; }
        public string CidadeTrabalho { get; set; }
        public string EstadoTrabalho { get; set; }
        public string PaisTrabalho { get; set; }
        public string AbreviaturaLogradouroTrabalho { get; set; }
        public string SiglaEstadoTrabalho { get; set; }
    }
}
