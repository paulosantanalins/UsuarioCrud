using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class ValoresContratoPrestadorModel
    {
        public int Id { get; set; }
        public int IdPrestador { get; set; }
        public string TipoContrato { get; set; }
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public int IdTipoContrato { get; set; }
        public int QuantidadeNova { get; set; }
        public decimal ValorNovo { get; set; }
        public decimal Porcentagem { get; set; }
        public DateTime DataReajuste { get; set; }

        public int IdCelula { get; set; }
        public int Situacao { get; set; }
    }
}
