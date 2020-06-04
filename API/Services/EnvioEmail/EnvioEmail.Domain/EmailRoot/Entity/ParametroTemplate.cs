using EnvioEmail.Domain.SharedRoot.Entity;
using System.Collections.Generic;

namespace EnvioEmail.Domain.EmailRoot.Entity
{
    public class ParametroTemplate : EntityBase
    {
        public int IdTemplate { get; set; }
        public string NomeParametro { get; set; }

        public virtual TemplateEmail Template { get; set; }
        public virtual IEnumerable<ValorParametroEmail> ValoresParametro { get; set; }
    }
}
