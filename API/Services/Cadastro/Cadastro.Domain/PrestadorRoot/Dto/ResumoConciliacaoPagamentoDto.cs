namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ResumoConciliacaoPagamentoDto
    {
        public string Coligada { get; set; }
        public string Banco { get; set; }
        public decimal? ValorTotalLiquido { get; set; }
        public decimal? ValorTotalBruto { get; set; }
        public string NrCnpj { get; set; }
        public int? IdEmpresaGrupo { get; set; }
    }
}
