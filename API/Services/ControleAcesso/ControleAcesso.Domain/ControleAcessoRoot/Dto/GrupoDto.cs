using System;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class GrupoDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public string Descricao { get; set; }
        
        public bool Ativo { get; set; }
    }
}
