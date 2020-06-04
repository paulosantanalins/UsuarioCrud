using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Dto
{
    public class GridServicoMigradoDTO : DtoBase
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public int IdServicoEacesso { get; set; }
        public string NmCliente { get; set; }
        public string NmServicoEacesso { get; set; }
        public string NmPortfolio { get; set; }
        public string NmEscopo { get; set; }
        public string DescStatus { get; set; }
    }
}
