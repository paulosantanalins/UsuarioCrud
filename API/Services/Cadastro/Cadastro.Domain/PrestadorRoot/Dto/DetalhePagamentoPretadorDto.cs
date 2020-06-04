using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class DetalhePagamentoPretadorDto
    {
        public string Tipo { get; set; }

        public decimal? Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal Total { get; set; }

        public string Observacao { get; set; }

    }
}
