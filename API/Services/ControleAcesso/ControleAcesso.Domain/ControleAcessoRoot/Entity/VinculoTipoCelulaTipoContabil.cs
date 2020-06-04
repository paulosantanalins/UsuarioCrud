using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.DominioRoot.ItensDominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class VinculoTipoCelulaTipoContabil:EntityBase
    {
        public virtual TipoCelula Tipocelula { get; set; }
        public virtual DomTipoContabil TipoContabil { get; set; }
        public int IdTipoCelula { get; set; }
        public int IdTipoContabil { get; set; }
    }
}
