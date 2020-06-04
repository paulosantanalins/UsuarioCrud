using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto
{
    public class ValoresRelatorioRentabilidadeDto
    {
        public ValoresRelatorioRentabilidadeDto()
        {
            ValoresSalario = new List<ValoresFicha>();
            ValoresGeral = new List<ValoresFicha>();
        }
        public string Descricao { get; set; }
        public decimal VlFat { get; set; }
        public decimal VlFatIr { get; set; }

        public decimal VlAjustFat { get; set; }
        public decimal VlFatAjustTotal { get; set; }
        public decimal VlMarkUp { get; set; }
        public decimal VlMarkUpIr { get; set; }
        public decimal VlDespGeral { get; set; }
        public decimal VlDespSal { get; set; }
        public decimal VlDespCel { get; set; }
        public decimal VlTotalDesp { get; set; }
        public decimal VlRepPag { get; set; }
        public decimal VlRepPagImp { get; set; }
        public decimal VlRepRec { get; set; }
        public decimal VlLucro { get; set; }
        public decimal VlLucroPercent { get; set; }

        public decimal VlDespGeralPercent { get; set; }
        public decimal VlDespSalPercent { get; set; }
        public decimal VlDespCelPercent { get; set; }
        public decimal VlTotalDespPercent { get; set; }

        public int IdServico { get; set; }
        public int IdCliente { get; set; }
        public int IdCelula { get; set; }
        public int Nivel { get; set; }
        public string Tipo { get; set; }
        public List<ValoresFicha> ValoresGeral { get; set; }
        public List<ValoresFicha> ValoresSalario { get; set; }
    }

    public class ValoresFicha
    {
        public decimal Valor { get; set; }
        public int IdLan { get; set; }
    }
}
