using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.Dto
{
    public class PortfolioServicoDto
    {
        public int Id { get; set; }
        public string NmServico { get; set; }
        public string DescServico { get; set; }
        public bool FlStatus { get; set; }
        public int? IdCategoria { get; set; }
        public string sgCategoria { get; set; }
        public int? IdTipoServico { get; set; }
        public int? IdClassificacaoContabil { get; set; }
        public string DescClassificacaoContabil { get; set; }
        public int? IdDelivery { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public string DescTipoServico { get; set; }

    }
}

