using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class VinculoUsuarioPerfilDto
    {
        public string Login { get; set; }
        public List<int> Perfis { get; set; }
    }
}
