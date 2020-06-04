using System;

namespace ControleAcesso.Domain.MonitoramentoRoot.DTO
{
    public class MonitoramentoBackDto
    {
        public int Id { get; set; }
        public string TipoLog { get; set; }
        public string Origem { get; set; }
        public string DetalheLog { get; set; }
        public string StackTrace { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
