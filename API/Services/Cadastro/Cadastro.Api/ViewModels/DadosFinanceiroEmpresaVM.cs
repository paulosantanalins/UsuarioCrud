using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class DadosFinanceiroEmpresaVM
    {        
        public string Empresa { get; set; }
        public string Descricao { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }        
    }
}
