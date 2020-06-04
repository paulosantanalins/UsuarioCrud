using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Entity
{
    public class ClienteContrato : EntityBase
    {
        public int IdCliente { get; set; }
        public int IdContrato { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}
