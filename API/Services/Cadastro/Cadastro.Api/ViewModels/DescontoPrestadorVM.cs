using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class DescontoPrestadorVM
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public decimal ValorDesconto { get; set; }
        public string Observacao { get; set; }
        public string TipoDesconto { get; set; }
        public int IdDesconto { get; set; }
        public int IdHorasMesPrestador { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
