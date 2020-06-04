using System;

namespace Cadastro.Api.ViewModels
{
    public class ClienteServicoPrestadorVM
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public int IdLocalTrabalho { get; set; }
        public string DescricaoTrabalho { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataPrevisaoTermino { get; set; }
        public decimal ValorCusto { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal ValorRepasse { get; set; }
        public bool Ativo { get; set; }

        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
