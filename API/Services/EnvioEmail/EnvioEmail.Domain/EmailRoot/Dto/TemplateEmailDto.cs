using EnvioEmail.Domain.EmailRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnvioEmail.Domain.EmailRoot.Dto
{
    public class TemplateEmailDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public bool FlagFixo { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Usuario { get; set; }
        public virtual IEnumerable<Email> Emails { get; set; }
        public virtual IEnumerable<ParametroTemplateDto> Parametros { get; set; }
    }
}
