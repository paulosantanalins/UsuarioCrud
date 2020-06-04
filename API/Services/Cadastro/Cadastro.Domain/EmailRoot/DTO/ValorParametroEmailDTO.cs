using System;

namespace Cadastro.Domain.EmailRoot.DTO
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
