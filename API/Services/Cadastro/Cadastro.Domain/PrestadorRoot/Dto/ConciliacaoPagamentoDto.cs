using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ConciliacaoPagamentoDto
    {
        public int IdCelula { get; set; }
        public string Nome { get; set; }
        public string Empresa { get; set; }
        public string Diretoria {get; set;}
        public string EmpresaGrupo { get; set; }
        public string Contratacao { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string NrCnpj { get; set; }
        public string ContaCaixa { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }       
        public string Conta { get; set; }
        public decimal ValorStfcorp { get; set; }
        public decimal ValorStfcorpComDescontos { get; set; }        
        public decimal ValorRm { get; set; }
        public decimal ValorRmComDesconto { get; set; }
        public bool Divergente { get; set; }
        public int? IdEmpresaGrupo { get; set; }
        public int? IdChaveOrigemIntRm { get; set; }
        public string StatusRm { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int DiaPagamento { get; set; }

        public bool Conciliado { get; set; }
        public bool DadosFinanceiro { get; set; }
        public bool CentroCusto { get; set; }

        public bool Fechado { get; set; }

        public string BancoEmpresa { get; set; }
        public string CodigoBancoEmpresa { get; set; }
        public string ContaEmpresa { get; set; }

        public List<EmpresaPrestador> EmpresasPrestador { get; set; }

        public ICollection<ClienteServicoPrestador> ClientesServicosPrestador;
    }
}
