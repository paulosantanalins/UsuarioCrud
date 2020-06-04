using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class VisualizacaoCelulaVM
    {
        public VisualizacaoCelulaVM()
        {
            Celulas = new List<CelulaVM>();
            Logins = new List<string>();
        }
        public int? Id { get; set; }
        public List<string> Logins { get; set; }
        public List<CelulaVM> Celulas { get; set; }
        public bool TodasAsCelulasSempre { get; set; }
        public bool TodasAsCelulasSempreMenosAPropria { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
    }
}
