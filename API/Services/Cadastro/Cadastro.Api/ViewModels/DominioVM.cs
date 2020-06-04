using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class DominioVM
    {
        public int Id { get; set; }
        public string ValorTipoDominio { get; set; }
        public int IdValor { get; set; }
        public string DescricaoValor { get; set; }
        public bool Ativo { get; set; }
    }
}
