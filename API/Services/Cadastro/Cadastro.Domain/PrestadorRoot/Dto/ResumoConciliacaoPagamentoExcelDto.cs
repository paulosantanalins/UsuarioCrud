namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ResumoConciliacaoPagamentoExcelDto
    {      
        public string Celula { get; set; }        
        public string DescricaoDiretoria { get; set; }
        public string IdEmpresaGrupo { get; set; }
        public string DescriçaoEmpresaGrupo { get; set; }
        public string Empresa { get; set; }
        public string CNPJ { get; set; }
        public string Prestador { get; set; }
        public string ValorBrutoStfcorp { get; set; }
        public string ValorBrutoRm { get; set; }
        public string ValorLiquidoRm { get; set; }
        public string TipoContratacao { get; set; }
        public string DiaPagamento { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string ContaCaixa { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Conciliado { get; set; }
        public string Fechado { get; set; }
    }
}
