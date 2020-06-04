using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class DadosFinanceiroVM
    {
        public string FormaPagamento { get; set; }
        public string TipoConta { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string NumeroConta { get; set; }
    }
}
