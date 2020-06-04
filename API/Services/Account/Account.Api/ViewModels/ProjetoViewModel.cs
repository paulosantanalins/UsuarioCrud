using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Api.ViewModels
{
    public class ProjetoViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Link { get; set; }
        public string LgUsuario { get; set; }
        public int Ordem { get; set; }
        public DateTime? DtAlteracao { get; set; }
        //public string Imagem { get; set; }
    }
}
