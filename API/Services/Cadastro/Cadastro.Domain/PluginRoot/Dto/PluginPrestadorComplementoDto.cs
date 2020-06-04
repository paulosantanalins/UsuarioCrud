using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PluginRoot.Dto
{
    public class PluginPrestadorComplementoDto
    {
        public string StatusInt { get; set; }
        public string OperacaoInt { get; set; }
        public DateTime DataInsercaoInt { get; set; }
        public string ChaveOrigemInt { get; set; }
        public string CodColigada { get; set; }
        public string CodRpr { get; set; }
        public string CcrCodFornec { get; set; }
        public string CcrNomeFornec { get; set; }
        public string CcrCidOrigem { get; set; }
    }
}
