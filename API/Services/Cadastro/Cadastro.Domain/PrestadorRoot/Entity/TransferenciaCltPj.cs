using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class TransferenciaCltPj : EntityBase
    {
        public int IdEacessoLegado { get; set; }
        public int IdPrestadorTransferido { get; set; }
        public string NomePrestador { get; set; }

        public virtual Prestador PrestadorTransferido { get; set; }
    }
}
