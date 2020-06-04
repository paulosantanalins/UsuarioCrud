using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{


    public class ServicoCelulaDTO
    {
        public int IdCelula { get; set; }
        public String NomeFantasia { get; set; }
        public int IdCliente { get; set; }
        public List<ServicosPendentesClienteDTO> servicos { get; set; }


    }
    public class ServicosPendentesClienteDTO
    {
        public int IdServico { get; set; }
        public String Nome { get; set; }

    }
}
