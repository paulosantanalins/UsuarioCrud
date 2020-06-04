using System;

namespace ControleAcesso.Api.ViewModels
{
    public class GrupoVM
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public string Descricao { get; set; }
        
        public bool Ativo { get; set; }
    }
}
