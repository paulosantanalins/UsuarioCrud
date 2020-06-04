using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Domain.EmailRoot
{
    public class ValorParametroEmailDTO
    {
        public int Id { get; set; }
        public int EmailId { get; set; }
        public DateTime DtCadastro { get; set; }
        public string ParametroNome { get; set; }
        public string ParametroValor { get; set; }
    }
}
