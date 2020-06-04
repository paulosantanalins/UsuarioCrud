using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.ViewModels.GestaoRespasse
{
    public class SolicitarRepasseVM
    {
        public int Id { get; set; }
        public int? IdProfissional { get; set; }
        public DateTime DtRepasse { get; set; }
        public string DescProjeto { get; set; }
        public string DescMotivoNegacao { get; set; }
        public string DescJustificativa { get; set; }
        public int? QtdRepasse { get; set; }
        public decimal? VlUnitario { get; set; }
        public decimal? VlTotal { get; set; }
        public decimal? VlCustoProfissional { get; set; }
        public string FlStatus { get; set; }
        public DateTime? DtRepasseMae { get; set; }
        public int IdEpm { get; set; }
        public int IdMoeda { get; set; }
        public int IdServicoContratadoOrigem { get; set; }
        public int IdServicoContratadoDestino { get; set; }
        public int? IdRepasseMae { get; set; }
        public int? IdTipoDespesa { get; set; }
        public int FlProfissionaisAtivos { get; set; }
        public ProfissionalEacesso Profissional { get; set; }



        public int IdCelOrigem { get; set; }
        public int IdCelDestino { get; set; }
        public int IdClienteOrigem { get; set; }
        public int IdClienteDestino { get; set; }

        public int VezesRepitidas { get; set; }
        public int? NrParcela { get; set; }

        


    }
}
