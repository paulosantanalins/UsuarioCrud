using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.DominioRoot.ItensDominio
{
    public class DomTipoContabil : Dominio
    {
        public IEnumerable<Celula> Celulas { get; set; }
        public IEnumerable<VinculoTipoCelulaTipoContabil> VinculosTipoCelulaTiposContabil { get; set; }
    }
}
