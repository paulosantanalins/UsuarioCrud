using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class PerfilDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string NmPerfil { get; set; }
        public string NmModulo { get; set; }
        public bool FlAtivo { get; set; }
        public List<string> Funcionalidades { get; set; }
        public List<string> FuncionalidadesKeys { get; set; }
    }
}
