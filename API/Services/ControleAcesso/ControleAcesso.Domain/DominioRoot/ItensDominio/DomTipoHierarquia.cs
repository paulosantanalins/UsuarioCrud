using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.DominioRoot.ItensDominio
{
    public class DomTipoHierarquia : Dominio
    {
        public IEnumerable<Celula> Celulas { get; set; }
    }
}
