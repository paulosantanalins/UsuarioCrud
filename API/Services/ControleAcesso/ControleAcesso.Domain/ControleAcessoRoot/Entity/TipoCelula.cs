using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class TipoCelula : EntityBase
    {
        public string Descricao { get; set; }

        public IEnumerable<Celula> Celulas { get; set; }

        public VinculoTipoCelulaTipoContabil TipoCelulaTipoContabil { get; set; }
    }
}
