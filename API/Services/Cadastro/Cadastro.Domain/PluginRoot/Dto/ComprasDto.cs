using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class ComprasDto
    {
        public int IdEmpresa { get; set; }
        public int IdFilial { get; set; }
        public int CelulaEmissao { get; set; }
        public DateTime DtEmissao { get; set; }
        public int HoraEmissao { get; set; }
        public string Origem { get; set; }
        public int CodigoTipoCompra { get; set; }
        public int SubCodigoTipoCompra { get; set; }
        public int IdTipoFornecedor { get; set; }
        public int IdProfissional { get; set; }
        public string NFFornec { get; set; }
        public int DtEmissaoNF { get; set; }
        public int Obs { get; set; }
        public int IdBancoPagto { get; set; }
        public string SiglaTpPagto { get; set; }
        public int NumBanco { get; set; }
        public string AgenciaBancaria { get; set; }
        public string ContaCorrente { get; set; }
        public DateTime DtPagto { get; set; }
        public decimal VrFicha { get; set; }
        public decimal Reembolso { get; set; }
        public string LoginExec { get; set; }
        public string StatusFicha { get; set; }
        public int Versao { get; set; }
        public int IdContaForn { get; set; }
        public int IdPagamento { get; set; }
        public int IDLan { get; set; }
        public decimal VrLiquido { get; set; }
        public decimal VrBruto { get; set; }
    }
}
