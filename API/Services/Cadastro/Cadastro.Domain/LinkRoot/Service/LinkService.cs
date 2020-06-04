using Cadastro.Domain.LinkRoot.Repository;
using Cadastro.Domain.LinkRoot.Service.Interfaces;
using System.Linq;

namespace Cadastro.Domain.LinkRoot.Service
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository _linkRepository;

        public LinkService(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public string ObterLinkPortal()
        {
            var result = _linkRepository.Buscar(x => x.Nome == "Portal").FirstOrDefault();
            if (result != null)
            {
                return result.Url;
            }
            else
            {
                return "";
            }
        }
    }
}
