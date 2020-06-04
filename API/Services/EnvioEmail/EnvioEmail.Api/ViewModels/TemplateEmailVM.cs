using System;
using System.Collections.Generic;

namespace EnvioEmail.Api.ViewModels
{
    public class TemplateEmailVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public bool FlagCorpoAlterado { get; set; }
        public bool FlagFixo { get; set; }        
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public IEnumerable<ParametroTemplateVM> Parametros { get; set; }
    }
}
