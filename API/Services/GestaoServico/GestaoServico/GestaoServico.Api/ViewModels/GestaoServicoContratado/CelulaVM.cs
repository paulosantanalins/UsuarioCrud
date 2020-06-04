using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoServicoContratado
{
    public class CelulaVM
    {
        public int Id { get; set; }
        public bool Inativa { get; set; }
        public string DescCelula { get; set; }
        public int IdMoeda { get; set; }
        public decimal Markup2012 { get; set; }
        public string DescricaoHierarquica { get; set; }
        public int? IdCelulaSup { get; set; }
        public bool RepasseParaMesmaCelula { get; set; }
    }
}
