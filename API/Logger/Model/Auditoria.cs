using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Model
{
    public class Auditoria
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Tabela { get; set; }
        public string IdsAlterados { get; set; }
        public string ValoresAntigos { get; set; }
        public string ValoresNovos { get; set; }
    }
}
