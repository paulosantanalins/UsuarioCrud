using System.Collections.Generic;
using Cadastro.Domain.PessoaRoot.Dto;

namespace Cadastro.Domain.PessoaRoot.Service.Interfaces
{
    public interface IViewProfissionaisService
    {
        IEnumerable<ViewProfissionalGeral> ViewProfissionalGeralEacesso();
        IEnumerable<ViewProfissionaisDesligados> ViewProfissionaisDesligados30Dias();
        IEnumerable<ViewAdimitidosFuturos> ViewAdimitidosFuturosNatCorp();
        IEnumerable<ViewProfissionaisDesligados> ViewInativadosNatCorp();
        IEnumerable<ViewDemissaoFuturas> ViewDemissaoFuturas();
    }
}
