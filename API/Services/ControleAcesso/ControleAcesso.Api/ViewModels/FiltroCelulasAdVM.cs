using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class FiltroCelulasAdVM
    {
        public string[] Logins { get; set; }
        public int[] Celulas { get; set; }
        public int Total { get; set; }
    }
}
