using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.DominioRoot.ItensDominio
{
    public class DomStatusRepasse : Dominio
    {
        public IEnumerable<LogRepasse> LogsRepasse { get; set; }
    }
}
