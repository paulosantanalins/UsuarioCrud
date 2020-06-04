using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoServicoContratado
{
    public class GridServicoMigradoVM
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public string NmCliente { get; set; }
        public string NmServicoEacesso { get; set; }
        public string NmPortfolio { get; set; }
        public string NmEscopo { get; set; }
        public string DescStatus { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
