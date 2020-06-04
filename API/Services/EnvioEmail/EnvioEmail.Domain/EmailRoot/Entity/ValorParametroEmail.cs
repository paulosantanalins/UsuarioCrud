using EnvioEmail.Domain.SharedRoot.Entity;
using System;

namespace EnvioEmail.Domain.EmailRoot.Entity
{
    public class ValorParametroEmail : EntityBase
    {
        public int EmailId { get; set; }
        public DateTime DtCadastro { get; set; }
        public string ParametroNome { get; set; }
        public string ParametroValor { get; set; }

        public virtual Email Email { get; set; }
        public virtual ParametroTemplate Parametro { get; set; }
    }
}
