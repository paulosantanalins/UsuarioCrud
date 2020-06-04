using System;

namespace GestaoServico.Domain.Dto
{
    public class ServicoEAcessoDTO
    {
        public int IdServico { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public string DescricaoServicoContratado { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
