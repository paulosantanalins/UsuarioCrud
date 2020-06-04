using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.DominioRoot.Entity
{
    public class Dominio : EntityBase
    {
        public string ValorTipoDominio { get; set; }
        public int IdValor { get; set; }
        public string DescricaoValor { get; set; }
        public bool Ativo { get; set; }
    }
}
