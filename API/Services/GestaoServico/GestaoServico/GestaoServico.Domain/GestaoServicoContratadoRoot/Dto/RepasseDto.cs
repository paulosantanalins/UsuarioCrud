using System;
using System.Collections.Generic;
using System.Text;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Dto
{
    public class RepasseDto
    {
        public int IdProfissional { get; set; }
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
        public ProfissionalEacesso Profissional { get; set; }
    }
}
