using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels.ControleAcesso
{
    public class BroadcastVM
    {
        public int Id { get; set; }
        public BroadcastItemVM BroadcastItem { get; set; }
        public int IdBroadcastItem { get; set; }
        public bool Excluido { get; set; }
        public bool Lido { get; set; }
        public string LgUsuarioVinculado { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
