using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class PrestadorDto
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public int IdHoraMesPrestador { get; set; }
        public string Nome { get; set; }
        public string Funcao { get; set; }
        public string Contratacao { get; set; }
        public string CPF { get; set; }
        public string Status { get; set; }

        public string Remuneracao { get; set; }

        public decimal? ValorHora { get; set; }
        public string ValorMes { get; set; }    
        public int? DiaPagamento { get; set; }
        public string Periodo { get; set; }
        public decimal? Horas { get; set; }
        public decimal? Extras { get; set; }
        public string Situacao { get; set; }
        public string TipoAprovacao { get; set; }

        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDesligamento { get; set; }

        public decimal Total { get; set; }

        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public string ObservacaoSemPrestacaoServico { get; set; }

        public string Email { get; set; }

        public string EmailInterno { get; set; }
    }
}
