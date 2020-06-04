using EnvioEmail.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvioEmail.Domain.EmailRoot.Entity
{
    public class TemplateEmail : EntityBase
    {
        public string Nome { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public bool FlagFixo { get; set; }
        [NotMapped]
        public bool FlagCorpoAlterado { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public virtual IEnumerable<Email> Emails { get; set; }
        public virtual IEnumerable<ParametroTemplate> Parametros { get; set; }
    }
}
