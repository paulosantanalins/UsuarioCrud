using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels.ControleAcesso
{
    public class FuncionalidadeVM
    {
        public int Id { get; set; }
        public string NmFuncionalidade { get; set; }
        public string DescFuncionalidade { get; set; }
        public bool FlAtivo { get; set; }

    }
}
