using System;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class RepasseDto
    {
        public int IdProfissional { get; set; }
        public int IdCliente1 { get; set; }
        public int IdServico1 { get; set; }
        public int IdEmpresa { get; set; }
        public int IdFilial { get; set; }
        public int IdFicha { get; set; }
        public DateTime DataRepasse { get; set; }
        public string CodigoCelOrigem { get; set; }
        public string CodigoCelDestino { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public string DescProj { get; set; }
        public string CodigoTipoDespesa { get; set; }
        public int QuantidadeHoras { get; set; }
        public decimal ValorDespUnit { get; set; }
        public decimal ValorDespesa { get; set; }
        public int MyPrVlDespesa { get; set; }
        public int StatusAprov { get; set; }
    }
}
