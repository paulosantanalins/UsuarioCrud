using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels.ControleAcesso
{
    public class BroadcastItemVM
    {
        public int Id { get; set; }
        public ICollection<BroadcastVM> Broadcasts { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Usuario { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
