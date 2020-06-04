using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class VinculoTipoCelulaTipoContabilVM
    {
        public int Id { get; set; }
        public TipoCelulaVM tipocelula { get; set; }
        public TipoCelulaVM tipoContabil { get; set; }

    }
}
