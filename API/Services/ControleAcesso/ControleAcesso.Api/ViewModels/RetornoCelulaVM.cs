using System.Collections.Generic;

namespace ControleAcesso.Api.ViewModels
{
    public class RetornoCelulasVM
    {
        public List<CelulaVM> Dados { get; set; }
        public string Notifications { get; set; }
        public bool Success { get; set; }
    }
}
