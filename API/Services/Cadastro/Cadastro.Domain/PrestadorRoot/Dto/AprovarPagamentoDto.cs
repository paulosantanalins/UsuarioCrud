using System;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class AprovarPagamentoDto
    {
        public AprovarPagamentoDto()
        {
            SomenteVisualiza = false;
        }

        public int Id { get; set; }
        public int IdCelula { get; set; }
        public int IdPessoaAprovador { get; set; }
        public int IdPessoaVisualizador { get; set; }
        public int? IdCelulaSuperior { get; set; }
        public int? IdHoraMesPrestador { get; set; }
        public string Cnpj { get; set; }
        public string Nome { get; set; }        
        public string Contratacao { get; set; }
        public int? DiaPagamento { get; set; }
        public decimal? Horas { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorComDescontos { get; set; }
        public string Tipo { get; set; }
        public bool? Aprovar { get; set; }
        public string Aprovador { get; set; }
        public string Status { get; set; }
        public bool? Desabilita { get; set; }
        public string Motivo { get; set; }
        public string Situacao { get; set; }
        public string Periodo { get; set; }
        public string NomeOperadorHoras { get; set;}
        public string EmailResponsavel { get; set; }
        public string EmailVisualizador { get; set; }
        public bool PossuiNf { get; set; }
        public bool SomenteVisualiza { get; set; }

        public int? IdChaveOrigemIntRm { get; set; }

        public string NomeEfetuaouUltimaALteracao { get; set;}
    }
}
