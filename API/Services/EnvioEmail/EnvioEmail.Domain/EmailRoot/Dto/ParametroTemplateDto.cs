using EnvioEmail.Domain.EmailRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnvioEmail.Domain.EmailRoot.Dto
{
    public class ParametroTemplateDto
    {
        public int Id { get; set; }
        public int IdTemplate { get; set; }
        public string NomeParametro { get; set; }

        public virtual IEnumerable<ValorParametroEmail> ValoresParametro { get; set; }
    }
}
