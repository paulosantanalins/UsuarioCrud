using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class EmpresaGrupoDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
    }
}
