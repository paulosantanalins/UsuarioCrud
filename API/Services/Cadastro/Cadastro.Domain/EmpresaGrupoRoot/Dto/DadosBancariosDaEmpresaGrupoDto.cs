using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.EmpresaGrupoRoot.Dto
{
    public class DadosBancariosDaEmpresaGrupoDto
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string Descricao { get; set; }        
        public string CodCxa { get; set; }
        public string Banco { get; set; }
        public string DigitoDoBanco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string DigitoDaConta { get; set; }




        //public int Id { get; set; }
        //public string Descricao { get; set; }
        //public string RazaoSocial { get; set; }
        //public string CodCxa { get; set; }        
        //public string NomeBanco { get; set; }
        //public string NumBanco { get; set; }
        //public string NumAgencia { get; set; }
        //public string NroConta { get; set; }
        //public string DigConta { get; set; }

    }
}
