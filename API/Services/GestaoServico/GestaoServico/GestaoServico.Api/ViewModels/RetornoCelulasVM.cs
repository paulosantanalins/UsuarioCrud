using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using System.Collections.Generic;

namespace GestaoServico.Api.ViewModels
{
    public class RetornoCelulasVM
    {
        public List<CelulaVM> Dados { get; set; }
        public string Notifications { get; set; }
        public bool Success { get; set; }
    }
}
