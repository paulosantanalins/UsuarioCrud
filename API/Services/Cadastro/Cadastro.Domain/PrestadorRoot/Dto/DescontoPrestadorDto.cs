using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class DescontoPrestadorDto
    {
        public int Id { get; set; }
        public int IdHorasMesPrestador { get; set; }
        public int IdCelula { get; set; }
        public int IdDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public string Observacao { get; set; }
        public string NomePrestador { get; set; }
        public string TipoDesconto { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Usuario { get; set; }
        public bool EnviadoParaRM { get; set; }

    }
}
