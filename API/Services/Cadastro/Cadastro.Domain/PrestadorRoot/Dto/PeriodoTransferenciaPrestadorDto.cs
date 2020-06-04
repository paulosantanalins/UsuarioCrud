using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class PeriodoTransferenciaPrestadorDto
    {
        public string NomePeriodo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
