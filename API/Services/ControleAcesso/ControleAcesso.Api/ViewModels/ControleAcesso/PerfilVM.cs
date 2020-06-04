using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels.ControleAcesso
{
    public class PerfilVM
    {
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int Id { get; set; }
        public string NmPerfil { get; set; }
        public string NmModulo { get; set; }
        public bool FlAtivo { get; set; }
        public List<string> Funcionalidades { get; set; }
        public List<string> FuncionalidadesKeys { get; set; }
        public List<VinculoPerfilFuncionalidadeVM> VinculoPerfilFuncionalidades { get; set; }
    }
}
