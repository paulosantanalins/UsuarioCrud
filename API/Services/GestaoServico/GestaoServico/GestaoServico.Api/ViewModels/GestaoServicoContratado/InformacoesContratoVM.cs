using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoServicoContratado
{
    public class InformacoesContratoVM
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime? DtFinalizacao { get; set; }
        public int IdMoeda { get; set; }
        public int IdCelulaComercial { get; set; }
    }
}
