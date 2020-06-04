using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoServicoContratado
{
    public class ServicoContratadoVM
    {
        public int? Id { get; set; }
        public int? IdCelula { get; set; }
        public int? IdCelulaComercial { get; set; }
        public int? IdCliente { get; set; }
        public string DescCliente { get; set; }
        public int? IdContrato { get; set; }
        public int? NrContrato { get; set; }
        public int? IdPortifolioServico { get; set; }
        public string DescPortfolio { get; set; }
        public decimal? VlMarkup { get; set; }
        public DateTime? DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
        public string Escopo { get; set; }
        public string FormaFaturamento { get; set; }
        public string NmTipoReembolso { get; set; }
        public decimal? VlRentabilidade{ get; set; }
        public bool? FlReembolso { get; set; }
        public bool? FlDespesasReembolsaveis { get; set; }
        public bool? FlHorasExtrasReembosaveis { get; set; }
        public bool? FlFaturaRecorrente { get; set; }
        public bool? FlReoneracao { get; set; }
        public decimal? VlKM { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdFilial { get; set; }
        public string IdProdutoRM { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public int IdEscopoServico { get; set; }

        public int? IdMoedaContrato { get; set; }
        public DateTime? DataInicialContrato { get; set; }
        public DateTime? DataFinalContrato { get; set; }

        public string DescTipoCelula { get; set; }
        public string DescEscopo { get; set; }
        public string CelulasComerciaisResponsaveis { get; set; }
    }
}
