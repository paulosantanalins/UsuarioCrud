using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.Dto
{
    public class ClassificacaoContabilDto
    {
        public int Id { get; set; }
        public string DescClassificacaoContabil { get; set; }
        public string SgClassificacaoContabil { get; set; }
        public bool FlStatus { get; set; }
        public int? IdCategoriaContabil { get; set; }
        public string DescCategoria { get; set; }
        public string SgCategoria { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
