using System.Collections.Generic;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class FiltroAdDtoSeguranca
    {
        public List<string> Celulas { get; set; }
        public string Token { get; set; }
    }
}
