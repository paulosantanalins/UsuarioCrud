using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.ViewModels
{
    public class AprovarPagamentoVM
    {
        public int Id { get; set; }

        public int? IdHoraMesPrestador { get; set; }

        public int IdHorasMes { get; set; }        

        public string Status { get; set; }

        public string Motivo { get; set; }
    }
}
