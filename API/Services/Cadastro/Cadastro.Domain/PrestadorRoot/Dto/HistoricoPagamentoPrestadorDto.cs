using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class HistoricoPagamentoPrestadorDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Login { get; set; }
        public DateTime? Data { get; set; }
        public string Motivo { get; set; }
    }
}
