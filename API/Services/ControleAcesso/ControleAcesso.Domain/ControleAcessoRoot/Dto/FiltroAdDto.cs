using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class FiltroAdDto
    {
        public string Login { get; set; }
        public int? Celula { get; set; }
        public int Total { get; set; }
    }
}
