using ControleAcesso.Domain.ControleAcessoRoot.Entity;

namespace ControleAcesso.Domain.MonitoramentoRoot.Entity
{
    public class MonitoramentoBack : EntityBase
    {
        public string TipoLog { get; set; }
        public string Origem { get; set; }
        public string DetalheLog { get; set; }
        public string StackTrace { get; set; }
    }
}
