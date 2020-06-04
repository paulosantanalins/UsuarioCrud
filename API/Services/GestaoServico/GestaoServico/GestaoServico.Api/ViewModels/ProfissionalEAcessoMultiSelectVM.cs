using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels
{
    public class ProfissionalEAcessoMultiSelectVM
    {
        public int Id { get; set; }
        public int IdSecundario { get; set; }
        public string Nome { get; set; }
        public int Situacao { get; set; }
        public bool Inativo { get; set; }
    }
}
