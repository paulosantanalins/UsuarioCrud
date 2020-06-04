using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class FormasPagamentosDto
    {
        public string DescricaoPagamento { get; set; }
        public string SiglaFormaPagamento { get; set; }
        public string ERPExterno { get; set; }
    }
}
