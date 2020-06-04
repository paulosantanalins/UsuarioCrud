using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class RelatorioPrestadoresAtivosDto
    {
        public string Diretoria { get; set; }
        public string Celula { get; set; }
        public string Nome { get; set; }
        public string Admissao { get; set; }
        public string Cargo { get; set; }
        public string CPF { get; set; }
        public string Sexo { get; set; }
        public string Cliente { get; set; }
        public string NomeFilial { get; set; }

        public string Empresa { get; set; }
        public string Servico { get; set; }
        public string Custo { get; set; }
        public string Contratacao { get; set; }      
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
        public string TipoRemuneracao { get; set; }
        public string Valor { get; set; }
        public string DiaPagamento { get; set; }
        public string DataDesligamento { get; set; }
        public int? IdEmpresaGrupo { get; set; }
        //public int IdCelula { get; set; }
        //public int IdCliente { get; set; }
        //public int IdServico { get; set; }

    }
}
