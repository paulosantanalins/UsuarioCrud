using System;
using System.Collections.Generic;

namespace GestaoServico.Domain.OperacaoMigradaRoot.DTO
{
    public class ServicoMigracaoDTO
    {
        public int IdServico { get; set; }
        public string DescricaoServico { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public decimal? MarkUp { get; set; }
        public string DescricaoContrato { get; set; }
        public DateTime? DtInicioContrato { get; set; }
        public DateTime? DtFimContrato { get; set; }
        public string DescricaoMoeda { get; set; } //Sempre Real
        public string DescricaoFormaFaturamento { get; set; }
        public decimal? RentabilidadePrevista { get; set; }
        public string DescricaoTipoReembolso { get; set; }
        public decimal? ValorKmRodado { get; set; }
        public int? IdColigada { get; set; }
        public string DescricaoColigada { get; set; }
        public int? IdFilial { get; set; }
        public string DescricaoFilial { get; set; } 
        public string CodProdutoRm { get; set; }
        public string DescricaoProdutoRm { get; set; } //nao to trazendo
        public bool? IsDespesasReembolsaveis { get; set; }
        public bool? IsHeReembolsaveis { get; set; }
        public bool? IsFaturaRecorrente { get; set; }
        public bool? IsReeoneracao { get; set; }

        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }

        public bool Principal { get; set; }
        public int IdEscopo { get; set; }
        public int IdPortfolio { get; set; }
        public int IdTipoCelula { get; set; }
        public string DescricaoOperacao { get; set; }
        public int? IdGrupoDelivery { get; set; }
        public int? IdFrenteNegocio { get; set; }

        public List<int> IdsServicosAgrupados { get; set; }
        public List<int> CelulasServicosComerciaisAgrupados { get; set; }
        public List<int> IdsClientes { get; set; }
    }
}
