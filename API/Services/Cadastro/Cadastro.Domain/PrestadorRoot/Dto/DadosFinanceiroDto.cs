using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class DadosFinanceiroDto
    {
        public string FormaPagamento { get; set; }
        public int TipoConta { get; set; }
        public int? Banco { get; set; }
        public string Agencia { get; set; }
        public string NumeroConta { get; set; }
        public string NomeBanco { get; set; }
    }
}
