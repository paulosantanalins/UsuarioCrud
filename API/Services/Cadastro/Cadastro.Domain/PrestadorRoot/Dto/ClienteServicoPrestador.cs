using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ClienteServicoEacesso
    {
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }

        public int IdServico { get; set; }

        public string Cliente { get; set; }

        public string Servico { get; set; }

        public string Custo { get; set; }
    }
}
