using Cadastro.Domain.CelulaRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Microsoft.Extensions.Options;
using Utils.Connections;

namespace Cadastro.Domain.CelulaRoot.Service
{
    public class GrupoService : IGrupoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IGrupoRepository _grupoRepository;
        public GrupoService(
            IUnitOfWork unitOfWork,
            IGrupoRepository grupoRepository,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
            _grupoRepository = grupoRepository;
        }

    }
}
