using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Options;
using Utils.Connections;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class InativacaoPrestadorService : IInativacaoPrestadorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInativacaoPrestadorRepository _inativacaoPrestadorRepository;        
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public InativacaoPrestadorService(IUnitOfWork unitOfWork,
            IInativacaoPrestadorRepository inativacaoPrestadorRepository,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _unitOfWork = unitOfWork;
            _inativacaoPrestadorRepository = inativacaoPrestadorRepository;            
            _connectionStrings = connectionStrings;
        }
        
    }
}

