using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class LogErroRMDto
    {
        public string DescricaoErro { get; set; }
        public string Tabela { get; set; }
        public DateTime DataErro { get; set; }
    }
}
