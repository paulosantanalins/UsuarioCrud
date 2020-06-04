using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class VinculoUsuarioPerfilVM
    {
        public string Login { get; set; }
        public List<int> Perfis { get; set; }
    }
}
