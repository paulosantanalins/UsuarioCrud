
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.DTO
{
    public class GridEscopoDTO
    {
        public int Id { get; set; }
        public string NmEscopoServico { get; set; }
        public bool FlAtivo { get; set; }
        public string NmPortifolioServico { get; set; }

        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
