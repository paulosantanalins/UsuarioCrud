using System;
using System.Collections.Generic;

namespace Cadastro.Domain.EmailRoot.DTO
{
    public class EmailDTO
    {
        public int Id { get; set; }
        public DateTime? DtCadastro { get; set; }

        public int? IdTemplate { get; set; }
        public string RemetenteNome { get; set; }
        public string Para { get; set; }
        public string ComCopia { get; set; }
        public string ComCopiaOculta { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public DateTime? DtParaEnvio { get; set; }
        public DateTime? DtEnvio { get; set; }
        public string Erro { get; set; }
        public string Status { get; set; }
        public int? TentativasComErro { get; set; }

        public IEnumerable<ValorParametroEmailDTO> ValoresParametro { get; set; }
    }
}
