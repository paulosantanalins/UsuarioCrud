using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class ComboDescontoPrestadorVM
    {
        public int Id { get; set; }
        public string Descricao { get; set; }        
        public bool PodeReceberDesconto { get; set; }        
    }
}
