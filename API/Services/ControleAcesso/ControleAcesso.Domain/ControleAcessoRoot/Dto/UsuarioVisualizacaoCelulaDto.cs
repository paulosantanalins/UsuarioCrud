using System;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class UsuarioVisualizacaoCelulaDto
    {
        public int Id { get; set; }
        public string LgUsuario { get; set; }
        public string NomeCompleto { get; set; }
        public string Usuario { get; set; }   
        public int IdCelula { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
