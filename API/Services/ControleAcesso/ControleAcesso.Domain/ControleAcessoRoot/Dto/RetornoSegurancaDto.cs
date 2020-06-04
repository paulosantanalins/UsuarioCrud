using System.Collections.Generic;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class RetornoSegurancaDto
    {
        public RetornoSegurancaDto()
        {
            Dados = new List<UsuarioSegurancaDto>();
        }

        public List<UsuarioSegurancaDto> Dados { get; set; }
        public string Notifications { get; set; }
        public bool Success { get; set; }
    }
}
