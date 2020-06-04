using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class LogCampoCelulaDto
    {
        public string Campo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNovo { get; set; }
        public string Usuario { get; set; }
        public DateTime? DtAlteracao { get; set; }
    }
}
