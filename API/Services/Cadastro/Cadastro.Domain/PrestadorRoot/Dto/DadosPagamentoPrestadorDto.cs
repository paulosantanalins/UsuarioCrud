using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class DadosPagamentoPrestadorDto
    {
        public DateTime? dataInicio { get; set; }
        public DateTime? dataDesligamento { get; set; }
        public ICollection<DetalhePagamentoPretadorDto> detalhesPagamentoPrestador { get; set; }
        public ICollection<HistoricoPagamentoPrestadorDto> historicosPagamentoPrestador { get; set; }
    }
}
