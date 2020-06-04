using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomTipoOcorrencia : Dominio
    {        
        public IEnumerable<ObservacaoPrestador> ObservacaoPrestador { get; set; }
    }
}
